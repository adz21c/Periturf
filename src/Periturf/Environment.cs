//
//   Copyright 2021 Adam Burton (adz21c@gmail.com)
//   
//   Licensed under the Apache License, Version 2.0 (the "License")
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//

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
    public partial class Environment
    {
        private readonly List<IHost> _hosts = new List<IHost>();
        private readonly Dictionary<string, IComponent> _components = new Dictionary<string, IComponent>();
        private readonly EventHandlerFactory _eventHandlerFactory;
        private readonly TimeSpan _defaultVerifyInactivityTimeout;

        private Environment(TimeSpan defaultVerifyInactivityTimeout)
        {
            _eventHandlerFactory = new EventHandlerFactory(this);
            _defaultVerifyInactivityTimeout = defaultVerifyInactivityTimeout;
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
            Task StartHost(IHost host)
            {
                try
                {
                    return host.StartAsync(ct);
                }
                catch (Exception ex)
                {
                    return Task.FromException(ex);
                }
            }

            var startingHosts = _hosts
                .Select(StartHost)
                .ToList();

            try
            {
                await Task.WhenAll(startingHosts);
            }
            catch
            {
                var hostDetails = startingHosts
                    .Where(x => x.IsFaulted)
                    .Select(x => new HostExceptionDetails(
                        x.Exception?.InnerExceptions?.ToArray() ?? Array.Empty<Exception>()))
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
            Task StopHost(IHost host)
            {
                try
                {
                    return host.StopAsync(ct);
                }
                catch (Exception ex)
                {
                    return Task.FromException(ex);
                }
            }

            var stoppingHosts = _hosts
                .Select(StopHost)
                .ToList();

            try
            {
                await Task.WhenAll(stoppingHosts);
            }
            catch
            {
                var hostDetails = stoppingHosts
                    .Where(x => x.IsFaulted)
                    .Select(x => new HostExceptionDetails(
                        x.Exception?.InnerExceptions?.ToArray() ?? Array.Empty<Exception>()))
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

            var spec = new EnvironmentSpecification();
            config?.Invoke(spec);
            var hosts = spec.Build();
            
            var env = new Environment(spec.VerifyInactivityTimeout);
            env._hosts.AddRange(hosts);
            foreach (var component in env._hosts.SelectMany(x => x.Components))
                env._components.Add(component.Key, component.Value);

            return env;
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

                return component.CreateConfigurationSpecification<TSpecification>(_environment._eventHandlerFactory);
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

        class EventHandlerFactory : IEventHandlerFactory
        {
            private readonly Environment _env;

            public EventHandlerFactory(Environment env)
            {
                _env = env;
            }

            public IEventHandler<TEventData> Create<TEventData>(IEnumerable<IEventHandlerSpecification<TEventData>> eventHandlerSpecifications)
            {
                return new EventHandler<TEventData>(_env, eventHandlerSpecifications.Select(x => x.Build()).ToList());
            }
        }

        class EventHandler<TEventData> : IEventHandler<TEventData>
        {
            private readonly Environment _env;
            private readonly List<Func<IEventContext<TEventData>, CancellationToken, Task>> _handlers;

            public EventHandler(Environment env, List<Func<IEventContext<TEventData>, CancellationToken, Task>> handlers)
            {
                _env = env;
                _handlers = handlers;
            }

            public async Task ExecuteHandlersAsync(TEventData eventData, CancellationToken ct)
            {
                var eventContext = new EventContext<TEventData>(_env, eventData);
                await Task.WhenAll(_handlers.Select(x => x(eventContext, ct)));
            }
        }

        class EventContext<TEventData> : IEventContext<TEventData>
        {
            private readonly Environment _env;

            public EventContext(Environment env, TEventData eventData)
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

        #region Verify

        public IVerifier Verify(Action<IVerificationContext> config)
        {
            var context = new VerificationContext(_components);
            config(context);
            return context;
        }

        private class VerificationContext : IVerificationContext, IEventConfigurator, IVerifier
        {
            private readonly Dictionary<string, IComponent> _components;
            private readonly List<IEventSpecification> _eventSpecifications = new List<IEventSpecification>();

            public VerificationContext(Dictionary<string, IComponent> components)
            {
                _components = components;
            }

            public void Event(Func<IEventConfigurator, IEventSpecification> config)
            {
                var spec = config(this);
                _eventSpecifications.Add(spec);
            }

            public async Task StartAsync(CancellationToken ct = default)
            {
                foreach (var spec in _eventSpecifications)
                    await spec.BuildAsync(ct);
            }

            TBuilder IEventConfigurator.GetEventBuilder<TBuilder>(string componentName)
            {
                if (_components.TryGetValue(componentName, out var component))
                    return (TBuilder)component.CreateEventBuilder();
                
                throw new ComponentLocationFailedException(componentName);
            }
        }



        #endregion
    }
}
