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
using Periturf.Configuration;
using Periturf.IdSvr4.Configuration;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Periturf
{
    /// <summary>
    /// 
    /// </summary>
    public static class ConfigurationContextExtensions
    {
        /// <summary>
        /// Defines test configuration for an IdentityServer4 component.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="config">The configuration.</param>
        [ExcludeFromCodeCoverage]
        public static void IdSvr4(this IConfigurationContext builder, Action<IIdSvr4Configurator> config)
        {
            builder.IdSvr4("IdSvr4", config);
        }

        /// <summary>
        /// Defines test configuration for an IdentityServer4 component.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="name">The component name.</param>
        /// <param name="config">The configuration.</param>
        public static void IdSvr4(this IConfigurationContext builder, string name, Action<IIdSvr4Configurator> config)
        {
            var spec = builder.CreateComponentConfigSpecification<IdSvr4Specification>(name);
            config(spec);
            builder.AddSpecification(spec);
        }
    }
}
