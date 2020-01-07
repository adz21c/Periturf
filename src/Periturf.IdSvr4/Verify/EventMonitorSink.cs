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
using System.Collections.Concurrent;
using System.Threading.Tasks;
using IdentityServer4.Events;
using IdentityServer4.Services;

namespace Periturf.IdSvr4.Verify
{
    class EventMonitorSink : IEventMonitorSink
    {
        private readonly IEventSink? _innerSink;
        private readonly ConcurrentDictionary<Type, ConcurrentDictionary<Guid, IEventOccurredConditionEvaluator>> _evaluators = new ConcurrentDictionary<Type, ConcurrentDictionary<Guid, IEventOccurredConditionEvaluator>>();

        public EventMonitorSink(IEventSink? innerSink = null)
        {
            _innerSink = innerSink;
        }

        void IEventMonitorSink.AddEvaluator(Type eventType, IEventOccurredConditionEvaluator evaluator)
        {
            var eventTypeEvaluators = _evaluators.GetOrAdd(eventType, x => new ConcurrentDictionary<Guid, IEventOccurredConditionEvaluator>());
            eventTypeEvaluators.TryAdd(evaluator.Id, evaluator);
        }

        void IEventMonitorSink.RemoveEvaluator(Type eventType, IEventOccurredConditionEvaluator evaluator)
        {
            var eventTypeEvaluators = _evaluators.GetOrAdd(eventType, x => new ConcurrentDictionary<Guid, IEventOccurredConditionEvaluator>());
            eventTypeEvaluators.TryRemove(evaluator.Id, out var _);
        }

        async Task IEventSink.PersistAsync(Event evt)
        {
            await (_innerSink?.PersistAsync(evt) ?? Task.CompletedTask);

            var eventType = evt.GetType();
            var eventTypeEvaluators = _evaluators.GetOrAdd(eventType, x => new ConcurrentDictionary<Guid, IEventOccurredConditionEvaluator>());

            foreach (var evaluator in eventTypeEvaluators.Values)
                await evaluator.CheckEventAsync(evt);
        }
    }

}
