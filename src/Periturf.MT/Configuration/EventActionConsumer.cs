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
using Periturf.MT.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Periturf.MT.Configuration
{
    class EventActionConsumer<TMessage> : IConsumer<TMessage> where TMessage : class
    {
        private readonly string _componentName;
        private readonly IEventResponseContextFactory _eventResponseContextFactory;
        private readonly IReadOnlyCollection<Func<IMessageReceivedContext<TMessage>, bool>> _predicates;
        private readonly IReadOnlyCollection<Func<IEventResponseContext<IMessageReceivedContext<TMessage>>, Task>> _actions;

        public EventActionConsumer(
            string componentName,
            IEventResponseContextFactory eventResponseContextFactory,
            IReadOnlyCollection<Func<IMessageReceivedContext<TMessage>, bool>> predicates,
            IReadOnlyCollection<Func<IEventResponseContext<IMessageReceivedContext<TMessage>>, Task>> actions)
        {
            _componentName = componentName;
            _eventResponseContextFactory = eventResponseContextFactory;
            _predicates = predicates;
            _actions = actions;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var adapter = new ConsumeContextToMessageReceivedContextAdapter<TMessage>(context);
            if (_predicates.Any() && !_predicates.Any(x => x(adapter)))
                return;

            var eventResponseContext = new ConsumerEventResponseContext<TMessage>(
                _componentName,
                context,
                _eventResponseContextFactory.Create<IMessageReceivedContext<TMessage>>(adapter));
            foreach (var action in _actions)
                await action(eventResponseContext).ConfigureAwait(false);
        }
    }
}