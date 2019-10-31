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
using IdentityServer4.Events;
using Periturf.Verify;

namespace Periturf.IdSvr4.Verify
{
    class IdSvr4ConditionBuilder : IIdSvr4ConditionBuilder
    {
        private readonly IEventMonitorSink _eventMonitorSink;

        public IdSvr4ConditionBuilder(IEventMonitorSink eventMonitorSink)
        {
            _eventMonitorSink = eventMonitorSink;
        }

        public IComponentConditionSpecification EventOccurred<TEvent>(Func<TEvent, bool> condition) where TEvent : Event
        {
            return new EventOccurredConditionSpecification<TEvent>(_eventMonitorSink, condition);
        }
    }
}
