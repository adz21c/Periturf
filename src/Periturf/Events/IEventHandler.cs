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
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Events
{
    /// <summary>
    /// Event handlers take an event and execute all the handlers.
    /// </summary>
    /// <typeparam name="TEventData">The type of the event data.</typeparam>
    public interface IEventHandler<in TEventData>
    {
        /// <summary>
        /// Executes the handlers asynchronously.
        /// </summary>
        /// <param name="eventData">The event data.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns></returns>
        Task ExecuteHandlersAsync(TEventData eventData, CancellationToken ct);
    }
}