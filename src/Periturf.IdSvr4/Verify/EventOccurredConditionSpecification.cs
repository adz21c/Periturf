﻿/*
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
    class EventOccurredConditionSpecification<TEvent> : IComponentConditionSpecification
        where TEvent : Event
    {
        private readonly IEventMonitorSink _eventMonitorSink;
        private readonly Func<TEvent, bool> _condition;

        public EventOccurredConditionSpecification(IEventMonitorSink eventMonitorSink, Func<TEvent, bool> condition)
        {
            _eventMonitorSink = eventMonitorSink;
            _condition = condition;
        }

        public string Description => throw new NotImplementedException();

        public Task<IComponentConditionEvaluator> BuildAsync(CancellationToken ct = default)
        {
            var evaluator = new EventOccurredConditionEvaluator<TEvent>(_condition);
            _eventMonitorSink.AddEvaluator(typeof(TEvent), evaluator);
            return Task.FromResult<IComponentConditionEvaluator>(evaluator);
        }
    }
}