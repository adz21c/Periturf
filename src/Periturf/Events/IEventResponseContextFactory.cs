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
namespace Periturf.Events
{
    /// <summary>
    /// Creates event response contexts.
    /// </summary>
    public interface IEventResponseContextFactory
    {
        /// <summary>
        /// Creates an event response context.
        /// </summary>
        /// <typeparam name="TEventData">The type of the event data.</typeparam>
        /// <param name="eventData">The event data.</param>
        /// <returns></returns>
        IEventResponseContext<TEventData> Create<TEventData>(TEventData eventData) where TEventData : class;
    }
}
