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
using Periturf.MT.Setup;
using Periturf.Setup;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Periturf
{
    /// <summary>
    /// 
    /// </summary>
    public static class SetupContextExtensions
    {
        /// <summary>
        /// Configures an MassTransit Host and Component.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="config">The configuration.</param>
        [ExcludeFromCodeCoverage]
        public static void MTBus(this ISetupContext context, Action<IBusConfigurator> config)
        {
            context.MTBus("MTBus", config);
        }

        /// <summary>
        /// Configures an MassTransit Host and Component.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="hostName">Name of the host.</param>
        /// <param name="config">The configuration.</param>
        /// <exception cref="ArgumentNullException">hostName
        /// or
        /// config</exception>
        public static void MTBus(this ISetupContext context, string hostName, Action<IBusConfigurator> config)
        {
            if (string.IsNullOrWhiteSpace(hostName))
                throw new ArgumentNullException(nameof(hostName));

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var busSpec = new BusSpecification(hostName, context.EventResponseContextFactory);
            config(busSpec);
            context.Host(hostName, busSpec.Build());
        }
    }
}
