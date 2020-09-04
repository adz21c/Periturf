using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Periturf.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Periturf.Web.Setup
{
    class WebComponentSetupSpecification : IWebComponentSetupConfigurator
    {
        public void ConfigureBuilder(Action<IWebHostBuilder> config)
        {
            BuilderConfig = config;
        }

        public Action<IWebHostBuilder>? BuilderConfig { get; private set; }

        public WebComponent Apply(IWebHostBuilder builder)
        {
            BuilderConfig?.Invoke(builder);

            var component = new WebComponent();
            builder.Configure(app =>
            {
                app.Use(async (context, next) =>
                {
                    await component.ProcessAsync(context);
                    await next();
                });
            });
            
            return component;
        }
    }
}
