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
using FakeItEasy;
using IdentityServer4.Events;
using IdentityServer4.Services;
using NUnit.Framework;
using Periturf.IdSvr4.Verify;
using Periturf.Verify;
using System;
using System.Threading.Tasks;

namespace Periturf.Tests.IdSvr4.Verify
{
    [TestFixture]
    class EventMonitorSinkTests
    {
        [Test]
        public async Task Given_InnerSink_When_Persist_Then_CallsInnerPersist()
        {
            var @event = A.Dummy<Event>();
            var innerSink = A.Fake<IEventSink>();
            IEventMonitorSink sink = new EventMonitorSink(innerSink);

            // Act
            await sink.PersistAsync(@event);

            // Assert
            A.CallTo(() => innerSink.PersistAsync(@event)).MustHaveHappened();
        }

        [Test]
        public async Task Given_Evaluator_When_AddEvaluatorAndPersist_Then_EvaluatorChecked()
        {
            var @event = A.Dummy<Event>();
            var innerSink = A.Dummy<IEventSink>();
            var evaluator = A.Fake<IEventOccurredConditionEvaluator>();
            IEventMonitorSink sink = new EventMonitorSink(innerSink);

            // Act
            sink.AddEvaluator(@event.GetType(), evaluator);
            await sink.PersistAsync(@event);

            // Assert
            A.CallTo(() => evaluator.CheckEvent(@event)).MustHaveHappened();
        }

        [Test]
        public async Task Given_Evaluator_When_RemoveEvaluatorAndPersist_Then_EvaluatorIsNotChecked()
        {
            var @event = A.Dummy<Event>();
            var innerSink = A.Dummy<IEventSink>();
            var evaluator = A.Fake<IEventOccurredConditionEvaluator>();
            IEventMonitorSink sink = new EventMonitorSink(innerSink);
            sink.AddEvaluator(@event.GetType(), evaluator);

            // Act
            sink.RemoveEvaluator(@event.GetType(), evaluator);
            await sink.PersistAsync(@event);

            // Assert
            A.CallTo(() => evaluator.CheckEvent(@event)).MustNotHaveHappened();
        }

        [Test]
        public async Task Given_EvaluatorOfDifferentType_When_Persist_Then_EvaluatorIsNotChecked()
        {
            var @event = A.Dummy<Event>();
            var innerSink = A.Dummy<IEventSink>();
            var evaluator = A.Fake<IEventOccurredConditionEvaluator>();
            IEventMonitorSink sink = new EventMonitorSink(innerSink);
            sink.AddEvaluator(typeof(DeviceAuthorizationFailureEvent), evaluator);

            // Act
            await sink.PersistAsync(@event);

            // Assert
            A.CallTo(() => evaluator.CheckEvent(@event)).MustNotHaveHappened();
        }

        [Test]
        public async Task Given_2TypesOfEvaluator_When_PersistForOneEventType_Then_OneEvaluatorChecked()
        {
            var @event = A.Dummy<Event>();
            var innerSink = A.Dummy<IEventSink>();
            var evaluator = A.Fake<IEventOccurredConditionEvaluator>();
            var evaluator2 = A.Fake<IEventOccurredConditionEvaluator>();
            IEventMonitorSink sink = new EventMonitorSink(innerSink);

            // Act
            sink.AddEvaluator(typeof(DeviceAuthorizationFailureEvent), evaluator);
            sink.AddEvaluator(@event.GetType(), evaluator2);
            await sink.PersistAsync(@event);

            // Assert
            A.CallTo(() => evaluator.CheckEvent(@event)).MustNotHaveHappened();
            A.CallTo(() => evaluator2.CheckEvent(@event)).MustHaveHappened();
        }
    }
}
