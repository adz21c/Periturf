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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Events;
using Periturf.Verify;
using Periturf.Verify.ComponentConditions;

namespace Periturf.IdSvr4.Verify
{
    class EventOccurredConditionSpecification<TEvent> : ComponentMonitorSpecification, IEventOccurredConditionEvaluator
        where TEvent : Event
    {
        private readonly IEventMonitorSink _eventMonitorSink;
        private readonly Func<TEvent, bool> _condition;
        private IConditionInstanceTimeSpanFactory? _timeSpanFactory;

        public EventOccurredConditionSpecification(IEventMonitorSink eventMonitorSink, Func<TEvent, bool> condition) : base()
        {
            _eventMonitorSink = eventMonitorSink;
            _condition = condition;
            Description = typeof(TEvent).Name;
        }

        protected override Task StartMonitorAsync(IConditionInstanceTimeSpanFactory timeSpanFactory, CancellationToken ct)
        {
            _timeSpanFactory = timeSpanFactory;
            _eventMonitorSink.AddEvaluator(typeof(TEvent), this);
            return Task.CompletedTask;
        }

        protected override Task StopMonitorAsync(CancellationToken ct)
        {
            _eventMonitorSink.RemoveEvaluator(typeof(TEvent), this);
            return Task.CompletedTask;
        }

        Guid IEventOccurredConditionEvaluator.Id { get; } = Guid.NewGuid();

        async Task IEventOccurredConditionEvaluator.CheckEventAsync(Event @event, CancellationToken ct)
        {
            Debug.Assert(_timeSpanFactory != null);
            
            if (@event is TEvent typedEvent && _condition(typedEvent))
                await ConditionInstanceHandler.HandleInstanceAsync(
                    new ConditionInstance(
                        _timeSpanFactory.Create(typedEvent.TimeStamp),
                        "Something"),
                    ct).ConfigureAwait(false);
        }
    }
}
