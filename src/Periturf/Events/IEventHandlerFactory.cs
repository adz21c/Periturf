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

using System.Collections.Generic;

namespace Periturf.Events
{
    /// <summary>
    /// Creates instances of <see cref="IEventHandler{TEventData}"/>
    /// </summary>
    public interface IEventHandlerFactory
    {
        /// <summary>
        /// Creates an event handler from the provided specifications.
        /// </summary>
        /// <typeparam name="TEventData">The type of the event data.</typeparam>
        /// <param name="eventHandlerSpecifications">The event handler specifications.</param>
        /// <returns></returns>
        IEventHandler<TEventData> Create<TEventData>(IEnumerable<IEventHandlerSpecification<TEventData>> eventHandlerSpecifications);
    }
}
