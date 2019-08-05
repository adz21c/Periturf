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
using Periturf.IdSvr4;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Periturf
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConfigurationBuilderExtensions
    {
        /// <summary>
        /// Defines test configuration for an IdentityServer4 component.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="config">The configuration.</param>
        [ExcludeFromCodeCoverage]
        public static void IdSvr4(this IConfiugrationBuilder builder, Action<IdSvr4Configurator> config)
        {
            builder.IdSvr4("IdSvr4", config);
        }

        /// <summary>
        /// Defines test configuration for an IdentityServer4 component.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="name">The component name.</param>
        /// <param name="config">The configuration.</param>
        public static void IdSvr4(this IConfiugrationBuilder builder, string name, Action<IdSvr4Configurator> config)
        {
            builder.AddComponentConfigurator<IdSvr4Component>(name, component =>
            {
                var configurator = new IdSvr4Configurator();
                config(configurator);

                var configRegistration = configurator.Build();
                return new ComponentConfigurator(component, configRegistration);
            });
        }
    }
}
