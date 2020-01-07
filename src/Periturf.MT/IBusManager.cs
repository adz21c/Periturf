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
using System.Threading.Tasks;
using MassTransit;
using Periturf.Configuration;
using Periturf.Events;
using Periturf.MT.Configuration;
using Periturf.MT.Verify;
using Periturf.Verify;

namespace Periturf.MT
{
    /// <summary>
    /// Represents a bus configured for a specific MassTransit transport
    /// </summary>
    public interface IBusManager
    {
        /// <summary>
        /// Gets the bus control.
        /// </summary>
        /// <value>
        /// The bus control.
        /// </value>
        IBusControl? BusControl { get; }

        /// <summary>
        /// Initial bus creation and configuration
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <param name="eventResponseContextFactory">The event response action context factory.</param>
        void Setup(IMtSpecification specification, IEventResponseContextFactory eventResponseContextFactory);

        /// <summary>
        /// Applies temporary configuration to the bus.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <param name="eventResponseContextFactory">The event response action context factory.</param>
        /// <returns>
        /// Disposable configuration handle.
        /// </returns>
        Task<IConfigurationHandle> ApplyConfigurationAsync(IMtSpecification specification, IEventResponseContextFactory eventResponseContextFactory);

        /// <summary>
        /// Applies verification setup on the bus.
        /// </summary>
        /// <param name="timeSpanFactory">The time span factory.</param>
        /// <param name="specification">The specification.</param>
        /// <returns></returns>
        Task<IVerificationHandle> ApplyVerificationAsync(IConditionInstanceTimeSpanFactory timeSpanFactory, IMtVerifySpecification specification);
    }
}