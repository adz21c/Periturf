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
using System.Threading.Tasks;

namespace Periturf.Events
{
    /// <summary>
    /// Implementation of <see cref="IEventConfigurator{TEventData}"/>.
    /// </summary>
    /// <typeparam name="TEventData">The type of the event data.</typeparam>
    /// <seealso cref="Periturf.Events.IEventConfigurator{TEventData}" />
    public class EventSpecification<TEventData> : IEventConfigurator<TEventData>
        where TEventData : class
    {
        private readonly List<Func<IEventContext<TEventData>, Task>> _actions = new List<Func<IEventContext<TEventData>, Task>>();

        /// <summary>
        /// Gets the actions.
        /// </summary>
        /// <value>
        /// The actions.
        /// </value>
        public IReadOnlyList<Func<IEventContext<TEventData>, Task>> Actions => _actions;

        /// <summary>
        /// The action to be executed in response to an event. Can be executed multiple times to define multiple actions.
        /// </summary>
        /// <param name="response">The response.</param>
        /// <exception cref="System.ArgumentNullException">reaction</exception>
        public void React(Func<IEventContext<TEventData>, Task> reaction)
        {
            _actions.Add(reaction ?? throw new ArgumentNullException(nameof(reaction)));
        }
    }
}
