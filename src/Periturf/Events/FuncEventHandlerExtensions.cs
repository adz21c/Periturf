//
//   Copyright 2021 Adam Burton (adz21c@gmail.com)
//   
//   Licensed under the Apache License, Version 2.0 (the "License")
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//

using Periturf.Events;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class FuncEventHandlerExtensions
    {
        /// <summary>
        /// Adds a dynamic event hander.
        /// </summary>
        /// <typeparam name="TEventData">The type of the event data.</typeparam>
        /// <param name="configurator">The configurator.</param>
        /// <param name="handler">The handler.</param>
        /// <exception cref="ArgumentNullException">
        /// configurator
        /// or
        /// handler
        /// </exception>
        public static void Handle<TEventData>(this IEventConfigurator<TEventData> configurator, Func<IEventContext<TEventData>, CancellationToken, Task> handler)
        {
            if (configurator is null)
                throw new ArgumentNullException(nameof(configurator));

            configurator.AddHandlerSpecification(new FuncEventHandlerSpecification<TEventData>(handler ?? throw new ArgumentNullException(nameof(handler))));
        }
    }
}
