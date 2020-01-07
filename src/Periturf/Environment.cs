/*
 *     Copyright 2019 Adam Burton (adz21c@gmail.com)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Periturf.Clients;
using Periturf.Components;
using Periturf.Configuration;
using Periturf.Events;
using Periturf.Setup;
using Periturf.Verify;

namespace Periturf
{
    /// <summary>
    /// The environment which manages the assignment and removal of configuration to components.
    /// </summary>
    public class Environment
    {
        private readonly Dictionary<string, IHost> _hosts = new Dictionary<string, IHost>();
        private readonly Dictionary<string, IComponent> _components = new Dictionary<string, IComponent>();
        private readonly EventResponseContextFactory _eventResponseContextFactory;
        private TimeSpan _defaultExpectationTimeout = TimeSpan.FromMilliseconds(5000);
        private bool _defaultExpectationShortCircuit = false;

        private Environment()
        {
            _eventResponseContextFactory = new EventResponseContextFactory(this);
        }

        /// <summary>
        /// Starts all hosts in the environment.
        /// </summary>
        /// <param name="ct">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="EnvironmentStartException"></exception>
        public async Task StartAsync(CancellationToken ct = default)
        {
            // For symplicity, lets not fail fast :-/
            Task StartHost(KeyValuePair<string, IHost> host)
            {
                try
                {
                    return host.Value.StartAsync(ct);
                }
                catch (Exception ex)
                {
                    return Task.FromException(ex);
                }
            }

            var startingHosts = _hosts
                .Select(x => new { Name = x.Key, Task = StartHost(x) })
                .ToList();

            try
            {
                await Task.WhenAll(startingHosts.Select(x => x.Task));
            }
            catch
            {
                var hostDetails = startingHosts
                    .Where(x => x.Task.IsFaulted)
                    .Select(x => new HostExceptionDetails(
                        x.Name,
                        x.Task.Exception.InnerExceptions.First()))
                    .ToArray();

                throw new EnvironmentStartException(hostDetails);
            }
        }

        /// <summary>
        /// Stops all hosts in the environment.
        /// </summary>
        /// <param name="ct">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="EnvironmentStopException"></exception>
        public async Task StopAsync(CancellationToken ct = default)
        {
            // For symplicity, lets not fail fast :-/
            Task StopHost(KeyValuePair<string, IHost> host)
            {
                try
                {
                    return host.Value.StopAsync(ct);
                }
                catch (Exception ex)
                {
                    return Task.FromException(ex);
                }
            }

            var stoppingHosts = _hosts
                .Select(x => new { Name = x.Key, Task = StopHost(x) })
                .ToList();

            try
            {
                await Task.WhenAll(stoppingHosts.Select(x => x.Task));
            }
            catch
            {
                var hostDetails = stoppingHosts
                    .Where(x => x.Task.IsFaulted)
                    .Select(x => new HostExceptionDetails(
                        x.Name,
                        x.Task.Exception.InnerExceptions.First()))
                    .ToArray();

                throw new EnvironmentStopException(hostDetails);
            }
        }

        #region Setup

        /// <summary>
        /// Creates and configures the hosts and components within an environment.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        public static Environment Setup(Action<ISetupContext> config)
        {
            var env = new Environment();

            var configurator = new SetupContext(env);
            config(configurator);

            return env;
        }

        class SetupContext : ISetupContext
        {
            private readonly Environment _env;

            public SetupContext(Environment env)
            {
                _env = env;
            }

            public IEventResponseContextFactory EventResponseContextFactory => _env._eventResponseContextFactory;

            public void DefaultExpectationTimeout(TimeSpan timeout)
            {
                if (timeout <= TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException(nameof(timeout));

                _env._defaultExpectationTimeout = timeout;
            }

            public void DefaultExpectationShortCircuit(bool shortCircuit)
            {
                _env._defaultExpectationShortCircuit = shortCircuit;
            }

            public void Host(string name, IHost host)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentNullException(nameof(name));

                if (host == null)
                    throw new ArgumentNullException(nameof(host));

                if (_env._hosts.ContainsKey(name))
                    throw new DuplicateHostNameException(name);

                _env._hosts.Add(name, host);
                foreach (var comp in host.Components)
                {
                    if (_env._components.ContainsKey(comp.Key))
                        throw new DuplicateComponentNameException(comp.Key);

                    _env._components.Add(comp.Key, comp.Value);
                }
            }
        }

        #endregion

        #region Configure

        /// <summary>
        /// Configures expectation into the environment.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <param name="ct"></param>
        /// <returns>The unique identifier for the expectation configuration.</returns>
        /// <exception cref="ConfigurationApplicationException"></exception>
        public async Task<IConfigurationHandle> ConfigureAsync(Action<IConfigurationContext> config, CancellationToken ct = default)
        {
            // Gather configuration
            var context = new ConfigurationContext(this);
            config(context);

            return await context.ApplyAsync(ct);
        }

        class ConfigurationContext : IConfigurationContext
        {
            private readonly Environment _environment;
            private readonly List<IConfigurationSpecification> _specifications = new List<IConfigurationSpecification>();

            public ConfigurationContext(Environment environment)
            {
                _environment = environment;
            }

            public TSpecification CreateComponentConfigSpecification<TSpecification>(string componentName) where TSpecification : IConfigurationSpecification
            {
                if (string.IsNullOrWhiteSpace(componentName))
                    throw new ArgumentNullException(nameof(componentName));

                if (!_environment._components.TryGetValue(componentName, out var component))
                    throw new ComponentLocationFailedException(componentName);

                return component.CreateConfigurationSpecification<TSpecification>(_environment._eventResponseContextFactory);
            }

            public void AddSpecification(IConfigurationSpecification specification)
            {
                _specifications.Add(specification ?? throw new ArgumentNullException(nameof(specification)));
            }

            public async Task<IConfigurationHandle> ApplyAsync(CancellationToken ct)
            {
                var specTasks = _specifications.Select(x => x.ApplyAsync(ct)).ToList();
                await Task.WhenAll(specTasks);
                return new ConfigurationHandle(specTasks.Select(x => x.Result));
            }
        }

        #endregion

        #region Verify

        /// <summary>
        /// Defines a verifier to establish if expectations are met.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns></returns>
        public async Task<IVerifier> VerifyAsync(Action<IVerificationContext> builder, CancellationToken ct = default)
        {
            var context = new VerificationContext(this);
            builder(context);

            return await context.BuildAsync(ct);
        }

        class VerificationContext : IVerificationContext
        {
            private readonly List<(IComponentConditionSpecification ComponentSpec, ExpectationSpecification ExpectationSpec)> _specs = new List<(IComponentConditionSpecification, ExpectationSpecification)>();
            private readonly Environment _env;
            private TimeSpan? _expectationTimeout;
            private bool? _shortCircuit;
            
            public VerificationContext(Environment env)
            {
                _env = env;
            }

            public void Expect(IComponentConditionSpecification conditionSpecification, Action<IExpectationConfigurator> config)
            {
                var expecationSpec = new ExpectationSpecification();
                config(expecationSpec);

                _specs.Add(
                    (
                        conditionSpecification ?? throw new ArgumentNullException(nameof(conditionSpecification)),
                        expecationSpec
                    ));
            }

            public TComponentConditionBuilder GetComponentConditionBuilder<TComponentConditionBuilder>(string componentName) where TComponentConditionBuilder : IComponentConditionBuilder
            {
                if (!_env._components.TryGetValue(componentName, out var component))
                    throw new ComponentLocationFailedException(componentName);

                return component.CreateConditionBuilder<TComponentConditionBuilder>();
            }

            public void Timeout(TimeSpan timeout)
            {
                if (timeout <= TimeSpan.Zero)
                    throw new ArgumentOutOfRangeException(nameof(timeout));

                _expectationTimeout = timeout;
            }

            public void ShortCircuit(bool? shortCircuit)
            {
                _shortCircuit = shortCircuit;
            }

            public async Task<Verifier> BuildAsync(CancellationToken ct)
            {
                // use the longest defined timeout
                var verifierTimeout = _specs
                    .Select(x => x.ExpectationSpec.Timeout ?? TimeSpan.Zero)
                    .Concat(new[] { _expectationTimeout ?? _env._defaultExpectationTimeout })
                    .Max();

                var timespanFactory = new ConditionInstanceTimeSpanFactory(DateTime.Now);
                
                var expectations = _specs.Select(async x =>
                {
                    var componentConditionEvaluator = await x.ComponentSpec.BuildAsync(timespanFactory, ct);
                    return x.ExpectationSpec.Build(verifierTimeout, componentConditionEvaluator, x.ComponentSpec.Description);
                }).ToList();

                await Task.WhenAll(expectations);

                return new Verifier(
                    expectations.Select(x => x.Result).ToList(),
                    _shortCircuit ?? _env._defaultExpectationShortCircuit);
            }
        }

        #endregion

        #region Client

        /// <summary>
        /// Creates a component client.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">componentName</exception>
        /// <exception cref="ComponentLocationFailedException"></exception>
        public IComponentClient CreateComponentClient(string componentName)
        {
            if (string.IsNullOrWhiteSpace(componentName))
                throw new ArgumentNullException(nameof(componentName));

            if (!_components.TryGetValue(componentName, out var component))
                throw new ComponentLocationFailedException(componentName);

            return component.CreateClient();
        }

        #endregion

        #region Events

        class EventResponseContextFactory : IEventResponseContextFactory
        {
            private readonly Environment _env;

            public EventResponseContextFactory(Environment env)
            {
                _env = env;
            }

            public IEventResponseContext<TEventData> Create<TEventData>(TEventData eventData) where TEventData : class
            {
                return new EventResponseContext<TEventData>(_env, eventData);
            }
        }

        class EventResponseContext<TEventData> : IEventResponseContext<TEventData> where TEventData : class
        {
            private readonly Environment _env;

            public EventResponseContext(Environment env, TEventData eventData)
            {
                _env = env;
                Data = eventData;
            }

            public TEventData Data { get; }

            public IComponentClient CreateComponentClient(string componentName)
            {
                return _env.CreateComponentClient(componentName);
            }
        }

        #endregion
    }
}
