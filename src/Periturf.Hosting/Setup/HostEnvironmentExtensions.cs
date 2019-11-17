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
using Periturf.Hosting.Setup;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Periturf
{
    /// <summary>
    /// 
    /// </summary>
    public static class HostEnvironmentExtensions
    {
        /// <summary>
        /// Setup a generic host using default name.
        /// </summary>
        /// <param name="setupConfigurator">The setup configurator.</param>
        /// <param name="config">The configuration.</param>
        [ExcludeFromCodeCoverage]
        public static void GenericHost(this ISetupConfigurator setupConfigurator, Action<IPeriturfHostBuilder> config)
        {
            setupConfigurator.GenericHost("GenericHost", config);
        }

        /// <summary>
        /// Setup a generic host.
        /// </summary>
        /// <param name="setupConfigurator">The setup configurator.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="config">The configuration.</param>
        public static void GenericHost(this ISetupConfigurator setupConfigurator, string hostName, Action<IPeriturfHostBuilder> config)
        {
            var builder = new PeriturfHostBuilder(Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder());
            config(builder);
            setupConfigurator.Host(hostName, new HostAdapter(builder.Build(), builder.Components));
        }
    }
}
