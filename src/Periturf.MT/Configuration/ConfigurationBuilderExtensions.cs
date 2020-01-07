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
using Periturf.MT.Configuration;
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
        /// Defines test configuration for a MassTransit component.
        /// </summary>
        /// <param name="context">The builder.</param>
        /// <param name="config">The configuration.</param>
        [ExcludeFromCodeCoverage]
        public static void MTBus(this IConfigurationContext context, Action<IMtConfigurator> config)
        {
            context.MTBus("MTBus", config);
        }

        /// <summary>
        /// Defines test configuration for a MassTransit component.
        /// </summary>
        /// <param name="context">The builder.</param>
        /// <param name="name">The component name.</param>
        /// <param name="config">The configuration.</param>
        public static void MTBus(this IConfigurationContext context, string name, Action<IMtConfigurator> config)
        {
            var spec = context.CreateComponentConfigSpecification<MtConfigurationSpecification>(name);
            config(spec.MtSpec);
            context.AddSpecification(spec);
        }
    }
}
