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
using IdentityServer4.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.IdSvr4.Verify
{
    class EventOccurredConditionEvaluator<TEvent> : IEventOccurredConditionEvaluator where TEvent : Event
    {
        private readonly Func<TEvent, bool> _checker;
        private bool _occurred = false;

        public EventOccurredConditionEvaluator(Func<TEvent, bool> checker)
        {
            _checker = checker;
        }

        public Guid Id { get; } = Guid.NewGuid();

        public void CheckEvent(Event @event)
        {
            var upcastEvent = @event as TEvent;

            if (_checker(upcastEvent))
                _occurred = true;
        }

        public Task<bool> EvaluateAsync(CancellationToken ct = default)
        {
            return Task.FromResult(_occurred);
        }
    }
}
