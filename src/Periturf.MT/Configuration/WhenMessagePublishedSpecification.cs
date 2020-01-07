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
using MassTransit;
using Periturf.Events;
using Periturf.MT.Events;

namespace Periturf.MT.Configuration
{
    class WhenMessagePublishedSpecification<TReceivedMessage> : EventResponseSpecification<IMessageReceivedContext<TReceivedMessage>>, IWhenMessagePublishedSpecification, IWhenMessagePublishedConfigurator<TReceivedMessage> where TReceivedMessage : class
    {
        private readonly string _componentName;

        public WhenMessagePublishedSpecification(string componentName)
        {
            _componentName = componentName;
        }

        Type IWhenMessagePublishedSpecification.MessageType => typeof(TReceivedMessage);

        void IWhenMessagePublishedSpecification.Configure(IReceiveEndpointConfigurator configurator, IEventResponseContextFactory eventResponseContextFactory)
        {
            if (configurator == null)
                throw new ArgumentNullException(nameof(configurator));
            
            if (eventResponseContextFactory == null)
                throw new ArgumentNullException(nameof(eventResponseContextFactory));

            configurator.Consumer(() => new EventActionConsumer<TReceivedMessage>(
                _componentName,
                eventResponseContextFactory,
                Predicates,
                Actions));
        }
    }
}
