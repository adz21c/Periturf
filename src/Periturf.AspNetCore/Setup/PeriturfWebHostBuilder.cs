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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Periturf.Components;

namespace Periturf.AspNetCore.Setup
{
    class PeriturfWebHostBuilder : IPeriturfWebHostBuilder
    {
        private readonly IWebHostBuilder _builder;

        public PeriturfWebHostBuilder(IWebHostBuilder builder)
        {
            _builder = builder;
        }

        public Dictionary<string, IComponent> Components { get; } = new Dictionary<string, IComponent>();

        public void AddComponent(string componentName, IComponent component)
        {
            if (string.IsNullOrWhiteSpace(componentName))
                throw new ArgumentNullException(nameof(componentName));

            if (component == null)
                throw new ArgumentNullException(nameof(component));

            Components.Add(componentName, component);
        }

        public IWebHost Build()
        {
            return _builder.Build();
        }

        [ExcludeFromCodeCoverage]   // Passthrough code
        public IWebHostBuilder ConfigureAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            return _builder.ConfigureAppConfiguration(configureDelegate);
        }

        [ExcludeFromCodeCoverage]   // Passthrough code
        public IWebHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            return _builder.ConfigureServices(configureServices);
        }

        [ExcludeFromCodeCoverage]   // Passthrough code
        public IWebHostBuilder ConfigureServices(Action<WebHostBuilderContext, IServiceCollection> configureServices)
        {
            return _builder.ConfigureServices(configureServices);
        }

        [ExcludeFromCodeCoverage]   // Passthrough code
        public string GetSetting(string key)
        {
            return _builder.GetSetting(key);
        }

        [ExcludeFromCodeCoverage]   // Passthrough code
        public IWebHostBuilder UseSetting(string key, string value)
        {
            return _builder.UseSetting(key, value);
        }
    }
}
