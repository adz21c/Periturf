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

namespace Periturf.Events
{
    public abstract class EventSpecification<TEventData> : IEventConfigurator<TEventData>
        where TEventData : class
    {
        private readonly List<IEventHandlerSpecification<TEventData>> _handlerSpecifications = new List<IEventHandlerSpecification<TEventData>>();
        private readonly IEventHandlerFactory _eventHandlerFactory;

        protected EventSpecification(IEventHandlerFactory eventHandlerFactory)
        {
            _eventHandlerFactory = eventHandlerFactory ?? throw new ArgumentNullException(nameof(eventHandlerFactory));
        }

        public IReadOnlyList<IEventHandlerSpecification<TEventData>> HandlerSpecifications => _handlerSpecifications;

        public void AddHandlerSpecification(IEventHandlerSpecification<TEventData> spec)
        {
            _handlerSpecifications.Add(spec ?? throw new ArgumentNullException(nameof(spec)));
        }

        protected IEventHandler<TEventData> CreateHandler()
        {
            return _eventHandlerFactory.Create(_handlerSpecifications);
        }
    }
}
