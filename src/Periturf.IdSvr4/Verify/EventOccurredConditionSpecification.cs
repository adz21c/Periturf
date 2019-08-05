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
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Events;
using Periturf.Verify;

namespace Periturf.IdSvr4.Verify
{
    class EventOccurredConditionSpecification<TEvent> : IConditionSpecification, IConditionEraser
        where TEvent : Event
    {
        private readonly IEventMonitorSink _eventMonitorSink;
        private readonly Func<TEvent, bool> _condition;
        private IEventOccurredConditionEvaluator _evaluator;

        public EventOccurredConditionSpecification(IEventMonitorSink eventMonitorSink, Func<TEvent, bool> condition)
        {
            _eventMonitorSink = eventMonitorSink;
            _condition = condition;
        }

        Task<IConditionEvaluator> IConditionSpecification.BuildEvaluatorAsync(Guid verifierId, IConditionErasePlan erasePlan, CancellationToken ct)
        {
            _evaluator = new EventOccurredConditionEvaluator<TEvent>(_condition);
            _eventMonitorSink.AddEvaluator(typeof(TEvent), _evaluator);
            erasePlan.AddEraser(this);
            return Task.FromResult<IConditionEvaluator>(_evaluator);
        }

        Task IConditionEraser.EraseAsync(CancellationToken ct)
        {
            _eventMonitorSink.RemoveEvaluator(typeof(TEvent), _evaluator);
            return Task.CompletedTask;
        }
    }
}