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
using Periturf.Components;
using Periturf.Configuration;
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

        private Environment()
        { }

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
        public static Environment Setup(Action<ISetupConfigurator> config)
        {
            var env = new Environment();

            var configurator = new SetupConfigurator(env);
            config(configurator);

            return env;
        }

        class SetupConfigurator : ISetupConfigurator
        {
            private readonly Environment _env;

            public SetupConfigurator(Environment env)
            {
                _env = env;
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
        public async Task<Guid> ConfigureAsync(Action<IConfiugrationBuilder> config, CancellationToken ct = default)
        {
            var id = Guid.NewGuid();

            Task ConfigureComponent(IComponentConfigurator configurator)
            {
                try
                {
                    return configurator.RegisterConfigurationAsync(id, ct);
                }
                catch (Exception ex)
                {
                    return Task.FromException(ex);
                }
            }

            // Gather configuration
            var builder = new ConfigurationBuilder(this);
            config(builder);
            var configurators = builder.GetConfigurators();

            // Apply configuration
            var configuringComponents = configurators
                .Select(x => new { Name = x.Key, Task = ConfigureComponent(x.Value) })
                .ToList();

            try
            {
                await Task.WhenAll(configuringComponents.Select(x => x.Task));

                return id;
            }
            catch
            {
                var componentDetails = configuringComponents
                    .Where(x => x.Task.IsFaulted)
                    .Select(x => new ComponentExceptionDetails(
                        x.Name,
                        x.Task.Exception.InnerExceptions.First()))
                    .ToArray();

                throw new ConfigurationApplicationException(componentDetails);
            }
        }

        /// <summary>
        /// Removes the specified expectation configuration from the environment.
        /// </summary>
        /// <param name="configId">The configuration identifier.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ConfigurationRemovalException"></exception>
        public async Task RemoveConfigurationAsync(Guid configId, CancellationToken ct = default)
        {
            Task RemoveConfiguration(IComponent component)
            {
                try
                {
                    return component.UnregisterConfigurationAsync(configId, ct);
                }
                catch (Exception ex)
                {
                    return Task.FromException(ex);
                }
            }

            var configuringComponents = _components
                .Select(x => new { Name = x.Key, Task = RemoveConfiguration(x.Value) })
                .ToList();

            try
            {
                await Task.WhenAll(configuringComponents.Select(x => x.Task));
            }
            catch
            {
                var componentDetails = configuringComponents
                    .Where(x => x.Task.IsFaulted)
                    .Select(x => new ComponentExceptionDetails(
                        x.Name,
                        x.Task.Exception.InnerExceptions.First()))
                    .ToArray();

                throw new ConfigurationRemovalException(configId, componentDetails);
            }
        }

        class ConfigurationBuilder : IConfiugrationBuilder
        {
            private readonly Dictionary<string, IComponentConfigurator> _configurators = new Dictionary<string, IComponentConfigurator>();
            private readonly Environment _environment;

            public ConfigurationBuilder(Environment environment)
            {
                _environment = environment;
            }

            public void AddComponentConfigurator<T>(string componentName, Func<T, IComponentConfigurator> config)
                where T : IComponent
            {
                if (string.IsNullOrWhiteSpace(componentName))
                    throw new ArgumentNullException(nameof(componentName));

                var component = (T)_environment._components[componentName];
                var componentConfigurator = config(component);
                _configurators.Add(componentName, componentConfigurator);
            }

            public Dictionary<string, IComponentConfigurator> GetConfigurators() => _configurators;
        }

        #endregion

        #region Verify

        /// <summary>
        /// Registers listeners for conditions and returns a <see cref="IVerifier" /> to evaluate if the condition has happened since creation.
        /// </summary>
        /// <param name="verifierBuilder">Specifies the conditions for the verifier.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns>
        ///   <see cref="IVerifier" /> to evaluate if the condition has happened since creation
        /// </returns>
        public async Task<IVerifier> VerifyAsync(Func<IConditionContext, IConditionSpecification> verifierBuilder, CancellationToken ct = default)
        {
            var conditionContext = new ConditionContext(this);
            var condition = verifierBuilder(conditionContext);

            var verifyId = Guid.NewGuid();
            var erasePlan = new ErasePlan();
            var evaluator = await condition.BuildEvaluatorAsync(verifyId, erasePlan, ct);

            return new Verifier(evaluator, erasePlan);
        }

        class ConditionContext : IConditionContext
        {
            private readonly Environment _env;

            public ConditionContext(Environment env)
            {
                _env = env;
            }

            public TComponentConditionBuilder GetComponentConditionBuilder<TComponentConditionBuilder>(string componentName) where TComponentConditionBuilder : IComponentConditionBuilder
            {
                if (!_env._components.TryGetValue(componentName, out var component))
                    throw new ComponentLocationFailedException(componentName);

                return component.CreateConditionBuilder<TComponentConditionBuilder>();
            }
        }

        class Verifier : IVerifier
        {
            private readonly IConditionEvaluator _evaluator;
            private readonly ErasePlan _erasePlan;

            public Verifier(IConditionEvaluator evaluator, ErasePlan erasePlan)
            {
                _evaluator = evaluator;
                _erasePlan = erasePlan;
            }

            public async Task VerifyAndThrowAsync(CancellationToken ct = default)
            {
                var result = await _evaluator.EvaluateAsync(ct);
                if (!result)
                    throw new VerificationFailedException();
            }

            public Task CleanUpAsync(CancellationToken ct = default)
            {
                return _erasePlan.ExecuteCleanUpAsync(ct);
            }
        }

        class ErasePlan : IConditionErasePlan
        {
            private readonly List<IConditionEraser> _erasers = new List<IConditionEraser>();

            public void AddEraser(IConditionEraser eraser)
            {
                _erasers.Add(eraser ?? throw new ArgumentNullException(nameof(eraser)));
            }

            public async Task ExecuteCleanUpAsync(CancellationToken ct = default)
            {
                Task Erase(IConditionEraser eraser)
                {
                    try
                    {
                        return eraser.EraseAsync(ct);
                    }
                    catch (Exception ex)
                    {
                        return Task.FromException(ex);
                    }
                }

                var erasers = _erasers.Select(Erase).ToList();

                try
                {
                    await Task.WhenAll(erasers);
                }
                catch
                {
                    throw new VerificationCleanUpFailedException();
                }
            }
        }

        #endregion
    }
}
