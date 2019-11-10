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
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Events;

namespace Periturf.IdSvr4.Verify
{
    class ConditionInstanceFeedManager<TEvent> : IEventOccurredConditionEvaluator
    {
        private readonly List<ConditionInstanceFeeder> _feeders = new List<ConditionInstanceFeeder>();
        private readonly List<ConditionInstance> _history = new List<ConditionInstance>();
        private readonly Func<TEvent, bool> _condition;
        private readonly IEventMonitorSink _sink;

        public ConditionInstanceFeedManager(IEventMonitorSink sink, Func<TEvent, bool> condition)
        {
            _sink = sink;
            _condition = condition;
        }

        public Guid Id { get; } = new Guid();

        public ConditionInstanceFeeder CreateFeed()
        {
            if (!_feeders.Any())
                _sink.AddEvaluator(typeof(TEvent), this);

            var feeder = new ConditionInstanceFeeder();
            _feeders.Add(feeder);
            return feeder;
        }

        public void RemoveFeed(ConditionInstanceFeeder feed)
        {
            _feeders.Remove(feed);
            if (!_feeders.Any())
            {
                _sink.RemoveEvaluator(typeof(TEvent), this);
                _history.Clear();
            }
        }

        async Task IEventOccurredConditionEvaluator.CheckEventAsync(Event @event)
        {
            if (@event is TEvent tEvent && _condition(tEvent))
            {
                // TODO: Complete instance data
                var instance = new ConditionInstance(TimeSpan.FromMilliseconds(1), "ID");
                var instanceTasks = _feeders.Select(x => x.PushInstanceAsync(instance).AsTask()).ToList();
                await Task.WhenAll(instanceTasks);
                _history.Add(instance);
            }
        }
    }
}
