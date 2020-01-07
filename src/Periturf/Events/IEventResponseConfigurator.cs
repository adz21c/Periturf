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
using System.Threading.Tasks;

namespace Periturf.Events
{
    /// <summary>
    /// Defines actions to be executed on identification of an event.
    /// </summary>
    /// <typeparam name="TEventData">The type of the event data.</typeparam>
    public interface IEventResponseConfigurator<TEventData> where TEventData : class
    {
        /// <summary>
        /// The action to be executed in response to an event. Can be executed multiple times to define multiple actions.
        /// </summary>
        /// <param name="response">The response.</param>
        void Response(Func<IEventResponseContext<TEventData>, Task> response);
    }
}
