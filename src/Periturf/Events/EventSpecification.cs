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

using System;
using System.Collections.Generic;

namespace Periturf.Events
{
    /// <summary>
    /// Base event specification that implements all the <see cref="IEventHandler{TEventData}"/> orchestration.
    /// </summary>
    /// <typeparam name="TEventData">The type of the event data.</typeparam>
    /// <seealso cref="Periturf.Events.IEventConfigurator{TEventData}" />
    public abstract class EventSpecification<TEventData> : IEventConfigurator<TEventData>
        where TEventData : class
    {
        private readonly List<IEventHandlerSpecification<TEventData>> _handlerSpecifications = new List<IEventHandlerSpecification<TEventData>>();
        private readonly IEventHandlerFactory _eventHandlerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSpecification{TEventData}"/> class.
        /// </summary>
        /// <param name="eventHandlerFactory">The event handler factory.</param>
        /// <exception cref="ArgumentNullException">eventHandlerFactory</exception>
        protected EventSpecification(IEventHandlerFactory eventHandlerFactory)
        {
            _eventHandlerFactory = eventHandlerFactory ?? throw new ArgumentNullException(nameof(eventHandlerFactory));
        }

        /// <summary>
        /// Gets the handler specifications.
        /// </summary>
        /// <value>
        /// The handler specifications.
        /// </value>
        public IReadOnlyList<IEventHandlerSpecification<TEventData>> HandlerSpecifications => _handlerSpecifications;

        /// <summary>
        /// Adds an event handler specification.
        /// </summary>
        /// <param name="spec">The spec.</param>
        /// <exception cref="ArgumentNullException">spec</exception>
        public void AddHandlerSpecification(IEventHandlerSpecification<TEventData> spec)
        {
            _handlerSpecifications.Add(spec ?? throw new ArgumentNullException(nameof(spec)));
        }

        /// <summary>
        /// Creates the <see cref="IEventHandler{TEventData}"/>.
        /// </summary>
        /// <returns></returns>
        protected IEventHandler<TEventData> CreateHandler()
        {
            return _eventHandlerFactory.Create(_handlerSpecifications);
        }
    }
}
