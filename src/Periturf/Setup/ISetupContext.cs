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
using Periturf.Components;
using Periturf.Events;
using System;

namespace Periturf.Setup
{
    /// <summary>
    /// Gathers configuration for environment setup.
    /// </summary>
    public interface ISetupContext
    {
        /// <summary>
        /// Gets the event response action context factory.
        /// </summary>
        /// <value>
        /// The event response action context factory.
        /// </value>
        IEventResponseContextFactory EventResponseContextFactory { get; }

        /// <summary>
        /// Sets the default expectation short circuit state.
        /// </summary>
        /// <param name="shortCircuit">if set to <c>true</c> [short circuit].</param>
        void DefaultExpectationShortCircuit(bool shortCircuit);

        /// <summary>
        /// Sets the default expectation timeout.
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        void DefaultExpectationTimeout(TimeSpan timeout);

        /// <summary>
        /// Adds a host to the environment.
        /// </summary>
        /// <param name="name">Unique identifier for the host.</param>
        /// <param name="host">The host.</param>
        void Host(string name, IHost host);
    }
}
