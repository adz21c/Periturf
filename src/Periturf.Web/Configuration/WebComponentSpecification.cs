using Periturf.Configuration;
using Periturf.Events;
using Periturf.Web.Configuration.Requests;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Web.Configuration
{
    class WebComponentSpecification : IWebComponentConfigurator, IConfigurationSpecification
    {
        private readonly List<WebConfiguration> _configurations;
        private readonly IEventHandlerFactory _eventHandlerFactory;

        public WebComponentSpecification(List<WebConfiguration> configurations, IEventHandlerFactory eventHandlerFactory)
        {
            _configurations = configurations;
            _eventHandlerFactory = eventHandlerFactory;
        }

        public void OnRequest(Action<IWebRequestEventConfigurator> config)
        {
            if (config == null)
                return;

            var spec = new WebRequestEventSpecification(_eventHandlerFactory);
            config(spec);
            WebRequestSpecifications.Add(spec);
        }

        public List<WebRequestEventSpecification> WebRequestSpecifications { get; } = new List<WebRequestEventSpecification>();

        public Task<IConfigurationHandle> ApplyAsync(CancellationToken ct = default)
        {
            var newConfig = WebRequestSpecifications.Select(x => x.Build()).ToList();
            _configurations.AddRange(newConfig);
            
            return Task.FromResult<IConfigurationHandle>(new ConfigurationHandle(newConfig, _configurations));
        }

        class ConfigurationHandle : IConfigurationHandle
        {
            private readonly List<WebConfiguration> _newConfig;
            private readonly List<WebConfiguration> _configurations;

            public ConfigurationHandle(List<WebConfiguration> newConfig, List<WebConfiguration> configurations)
            {
                _newConfig = newConfig;
                _configurations = configurations;
            }

            public ValueTask DisposeAsync()
            {
                _configurations.RemoveAll(x => _newConfig.Contains(x));
                return new ValueTask();
            }
        }
    }
}
