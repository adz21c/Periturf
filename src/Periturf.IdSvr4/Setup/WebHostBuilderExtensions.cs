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
using IdentityModel.Client;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Periturf.IdSvr4;
using Periturf.IdSvr4.Clients;
using Periturf.IdSvr4.Configuration;
using Periturf.IdSvr4.Verify;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;

namespace Periturf
{
    /// <summary>
    /// 
    /// </summary>
    public static class WebHostBuilderExtensions
    {
        /// <summary>
        /// Adds and configures an IdentityServer4 component to the ASP.NET Core Web Host.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="config">The configuration.</param>
        [ExcludeFromCodeCoverage]
        public static void IdSvr4(this IPeriturfWebHostBuilder builder, Action<IdSvr4SetupConfigurator> config = null)
        {
            builder.IdSvr4("IdSvr4", config);
        }

        /// <summary>
        /// Adds and configures an IdentityServer4 component to the ASP.NET Core Web Host.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="name">The host name.</param>
        /// <param name="config">The configuration.</param>
        public static void IdSvr4(this IPeriturfWebHostBuilder builder, string name, Action<IdSvr4SetupConfigurator> config = null)
        {
            var configurator = new IdSvr4SetupConfigurator();
            config?.Invoke(configurator);

            var appPath = new PathString((name.StartsWith("/") ? string.Empty : "/") + name);

            var appConfig = configurator.AppConfigCallback ?? ((IApplicationBuilder app) => app.UseIdentityServer());
            builder.Configure(app => app.Map(appPath, appConfig));

            var urls = builder.GetSetting("urls");
            var url = urls.Split(';')
                .Select(x => x.Replace("*", "localhost"))
                .Select(x => new
                {
                    Url = x,
                    Rank = x.Contains("localhost") ? 0 : 1
                })
                .OrderBy(x => x.Rank)
                .Select(x => x.Url)
                .First();
            var uri = new Uri(new Uri(url), appPath.ToUriComponent());

            var store = new Store();
            var eventMonitorSink = new EventMonitorSink();

            var httpClient = new HttpClient() { BaseAddress = uri };
            var client = new ComponentClient(
                httpClient,
                new DiscoveryCache(uri.AbsoluteUri.ToLower(), httpClient));

            builder.ConfigureServices(services =>
            {
                services
                    .AddSingleton<IClientStore, Store>(sp => store)
                    .AddSingleton<IResourceStore, Store>(sp => store)
                    .AddSingleton<IEventSink, EventMonitorSink>(sp => eventMonitorSink);

                var identityServiceBuilder = services
                    .AddIdentityServer(i =>
                    {
                        i.Events.RaiseErrorEvents = true;
                        i.Events.RaiseFailureEvents = true;
                        i.Events.RaiseInformationEvents = true;
                        i.Events.RaiseSuccessEvents = true;
                    })
                    .AddDeveloperSigningCredential();

                configurator.ServicesCallback?.Invoke(identityServiceBuilder);
            });

            var component = new IdSvr4Component(store, eventMonitorSink, client);
            builder.AddComponent(name, component);
        }
    }
}
