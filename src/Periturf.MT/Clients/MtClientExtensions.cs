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
using Periturf.Events;
using Periturf.MT.Clients;

namespace Periturf
{
    /// <summary>
    /// 
    /// </summary>
    public static class MtClientExtensions
    {
        /// <summary>
        /// Creates a MassTransit client for the component within the environment.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="componentName">Name of the component for which the client belongs.</param>
        /// <returns></returns>
        public static IMTClient MTClient(this Environment environment, string componentName = "MTBus")
        {
            return (IMTClient)environment.CreateComponentClient(componentName);
        }

        /// <summary>
        /// Creates a MassTransit client for the component within the environment.
        /// </summary>
        /// <typeparam name="TEventData">The type of the event data.</typeparam>
        /// <param name="context">The context.</param>
        /// <param name="componentName">Name of the component for which the client belongs.</param>
        /// <returns></returns>
        public static IMTClient MTClient<TEventData>(this IEventResponseContext<TEventData> context, string componentName = "MTBus")
            where TEventData : class
        {
            return (IMTClient)context.CreateComponentClient(componentName);
        }
    }
}
