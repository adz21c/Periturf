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
using MassTransit;
using Periturf.Events;
using System;

namespace Periturf.MT.Configuration
{
    /// <summary>
    /// Specification of what to do when a specific type of mesage, matching a criteria, is received.
    /// </summary>
    public interface IWhenMessagePublishedSpecification
    {
        /// <summary>
        /// Gets the type of the message.
        /// </summary>
        /// <value>
        /// The type of the message.
        /// </value>
        Type MessageType { get; }

        /// <summary>
        /// Configures the specified receieve endpoint.
        /// </summary>
        /// <param name="configurator">The receive endpoint configurator.</param>
        /// <param name="eventResponseContextFactory">The event response action context factory.</param>
        void Configure(IReceiveEndpointConfigurator configurator, IEventResponseContextFactory eventResponseContextFactory);
    }
}
