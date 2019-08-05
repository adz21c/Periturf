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
using IdentityServer4.Events;
using IdentityServer4.Services;
using Periturf.Verify;
using System;

namespace Periturf
{

    /// <summary>
    /// Builds IdentityServer4 specific conditions for a component.
    /// </summary>
    /// <seealso cref="Periturf.Verify.IComponentConditionBuilder" />
    public interface IIdSvr4ConditionBuilder : IComponentConditionBuilder
    {
        /// <summary>
        /// Hooks into the IdentityServer4 <see cref="IEventService"/> to identity if an event has occurred.
        /// </summary>
        /// <typeparam name="TEvent">The type of the event.</typeparam>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        IConditionSpecification EventOccurred<TEvent>(Func<TEvent, bool> condition)
            where TEvent : Event;
    }
}