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
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Periturf.Components;

namespace Periturf.Hosting.Setup
{
    class PeriturfHostBuilder : IPeriturfHostBuilder
    {
        private readonly IHostBuilder _builder;

        public PeriturfHostBuilder(IHostBuilder builder)
        {
            _builder = builder;
        }

        public Dictionary<string, IComponent> Components { get; } = new Dictionary<string, IComponent>();

        [ExcludeFromCodeCoverage]   // Passthrough code
        public IDictionary<object, object> Properties => _builder.Properties;

        public void AddComponent(string componentName, IComponent component)
        {
            if (string.IsNullOrWhiteSpace(componentName))
                throw new ArgumentNullException(nameof(componentName));

            if (component == null)
                throw new ArgumentNullException(nameof(component));

            Components.Add(componentName, component);
        }

        public Microsoft.Extensions.Hosting.IHost Build()
        {
            return _builder.Build();
        }

        [ExcludeFromCodeCoverage]   // Passthrough code
        public IHostBuilder ConfigureAppConfiguration(Action<HostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            return _builder.ConfigureAppConfiguration(configureDelegate);
        }

        [ExcludeFromCodeCoverage]   // Passthrough code
        public IHostBuilder ConfigureContainer<TContainerBuilder>(Action<HostBuilderContext, TContainerBuilder> configureDelegate)
        {
            return _builder.ConfigureContainer(configureDelegate);
        }

        [ExcludeFromCodeCoverage]   // Passthrough code
        public IHostBuilder ConfigureHostConfiguration(Action<IConfigurationBuilder> configureDelegate)
        {
            return _builder.ConfigureHostConfiguration(configureDelegate);
        }

        [ExcludeFromCodeCoverage]   // Passthrough code
        public IHostBuilder ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureDelegate)
        {
            return _builder.ConfigureServices(configureDelegate);
        }

        [ExcludeFromCodeCoverage]   // Passthrough code
        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(IServiceProviderFactory<TContainerBuilder> factory)
        {
            return _builder.UseServiceProviderFactory(factory);
        }

        [ExcludeFromCodeCoverage]   // Passthrough code
        public IHostBuilder UseServiceProviderFactory<TContainerBuilder>(Func<HostBuilderContext, IServiceProviderFactory<TContainerBuilder>> factory)
        {
            return _builder.UseServiceProviderFactory(factory);
        }
    }
}
