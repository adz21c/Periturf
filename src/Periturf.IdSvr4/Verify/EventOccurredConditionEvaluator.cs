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

        private readonly object _feedManagerLock = new object();
        private ConditionInstanceFeedManager<TEvent>? _feedManager;

        public EventOccurredConditionSpecification(IEventMonitorSink eventMonitorSink, Func<TEvent, bool> condition)
        {
            _eventMonitorSink = eventMonitorSink;
            _condition = condition;
        }

        public string Description => typeof(TEvent).Name;

        public Task<IComponentConditionEvaluator> BuildAsync(IConditionInstanceTimeSpanFactory timespanFactory, CancellationToken ct = default)
        {
            if (_feedManager == null)
            {
                lock (_feedManagerLock)
                {
                    if (_feedManager == null)
                        _feedManager = new ConditionInstanceFeedManager<TEvent>(_eventMonitorSink, _condition, timespanFactory);
                }
            }

            return Task.FromResult<IComponentConditionEvaluator>(
                new LifetimeManager(
                    _feedManager.CreateFeed(),
                    _feedManager));
        }

        class LifetimeManager : IComponentConditionEvaluator
        {
            private readonly ConditionInstanceFeeder _feed;
            private readonly ConditionInstanceFeedManager<TEvent> _feedManager;

            public LifetimeManager(ConditionInstanceFeeder feed, ConditionInstanceFeedManager<TEvent> feedManager)
            {
                _feed = feed;
                _feedManager = feedManager;
            }

            public ValueTask DisposeAsync()
            {
                _feedManager.RemoveFeed(_feed);
                _feed.Complete();
                return new ValueTask();
            }

            public IAsyncEnumerable<ConditionInstance> GetInstancesAsync(CancellationToken ect = default)
            {
                return _feed.GetInstancesAsync(ect);
            }
        }
    }
}
