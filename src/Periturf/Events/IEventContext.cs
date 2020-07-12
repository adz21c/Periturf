/*
 *     Copyright 2020 Adam Burton (adz21c@gmail.com)
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
using Periturf.Clients;

namespace Periturf.Events
{
    /// <summary>
    /// The context within which an action responding to an event executes.
    /// </summary>
    /// <typeparam name="TEventData">The type of the event data.</typeparam>
    public interface IEventContext<out TEventData>
    {
        /// <summary>
        /// The event data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        TEventData Data { get; }

        /// <summary>
        /// Creates the component client.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        /// <returns></returns>
        IComponentClient CreateComponentClient(string componentName);
    }
}