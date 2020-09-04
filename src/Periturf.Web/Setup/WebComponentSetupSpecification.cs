using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Periturf.Components;
using Periturf.Hosting.Setup;
using System;

namespace Periturf.Web.Setup
{
    class WebComponentSetupSpecification : IGenericHostComponentSpecification, IWebComponentSetupConfigurator
    {
        public WebComponentSetupSpecification(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public Action<IWebHostBuilder>? ConfigreBuilderAction { get; private set; }

        public void ConfigureBuilder(Action<IWebHostBuilder> config)
        {
            ConfigreBuilderAction = config;
        }

        public IComponent Apply(IHostBuilder hostBuilder)
        {
            var component = new WebComponent();
            hostBuilder.ConfigureWebHostDefaults(w =>
            {
                ConfigreBuilderAction?.Invoke(w);
                w.Configure(app =>
                {
                    app.Use(async (context, next) =>
                    {
                        await component.ProcessAsync(context);
                        await next();
                    });
                });
            });
            
            return component;
        }
    }
}
