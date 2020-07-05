using Microsoft.AspNetCore.Http;
using Periturf.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Web.Configuration
{
    class WebComponentSpecification : IWebComponentConfigurator, IConfigurationSpecification
    {
        private readonly List<WebConfiguration> _configurations;

        public WebComponentSpecification(List<WebConfiguration> configurations)
        {
            _configurations = configurations;
        }

        public void OnRequest(Action<IWebRequestEventConfigurator> config)
        {
            if (config == null)
                return;

            var spec = new WebRequestEventSpecification();
            config(spec);
            WebRequestSpecifications.Add(spec);
        }

        public List<WebRequestEventSpecification> WebRequestSpecifications { get; } = new List<WebRequestEventSpecification>();

        public Task<IConfigurationHandle> ApplyAsync(CancellationToken ct = default)
        {
            var newConfig = WebRequestSpecifications.Select(x => new WebConfiguration(
                new List<Func<HttpRequest, bool>> { x => true },
                x.ResponseSpecification?.BuildFactory() ?? (x => (Task<HttpResponse>)null),
                new List<Func<HttpRequest, Task>>())).ToList();
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
