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
using NUnit.Framework;
using Periturf.IdSvr4.Verify;
using Periturf.Verify;
using System;
using System.Threading.Tasks;

namespace Periturf.Tests.IdSvr4.Verify
{
    [TestFixture]
    class EventOccurredConditionSpecificationTests
    {
        [Test]
        public async Task Given_Spec_When_BuildEvaluator_Then_EvaluatorReturnedAndRegisteredForCheckAndErasePlanned()
        {
            var eventMonitorSink = A.Fake<IEventMonitorSink>();
            var erasePlan = A.Fake<IConditionErasePlan>();
            var condition = A.Dummy<Func<Event, bool>>();

            IConditionSpecification spec = new EventOccurredConditionSpecification<Event>(eventMonitorSink, condition);

            // Act
            var evaluator = await spec.BuildEvaluatorAsync(Guid.NewGuid(), erasePlan);

            // Assert
            Assert.IsNotNull(evaluator);
            Assert.AreEqual(typeof(EventOccurredConditionEvaluator<Event>), evaluator.GetType());
            A.CallTo(() => erasePlan.AddEraser(A<IConditionEraser>._)).MustHaveHappened();

            var eventOccurredEvaluator = (IEventOccurredConditionEvaluator)evaluator;
            A.CallTo(() => eventMonitorSink.AddEvaluator(typeof(Event), eventOccurredEvaluator)).MustHaveHappened();
        }

        [Test]
        public async Task Given_Spec_When_Erase_Then_RemovesEvaluatorFromMonitorSink()
        {
            // Arrange
            var eventMonitorSink = A.Fake<IEventMonitorSink>();
            var erasePlan = A.Fake<IConditionErasePlan>();
            var condition = A.Dummy<Func<Event, bool>>();

            IConditionSpecification spec = new EventOccurredConditionSpecification<Event>(eventMonitorSink, condition);
            IEventOccurredConditionEvaluator evaluator = null;
            A.CallTo(() => eventMonitorSink.AddEvaluator(typeof(Event), A<IEventOccurredConditionEvaluator>._)).Invokes((Type t, IEventOccurredConditionEvaluator e) => evaluator = e);
            await spec.BuildEvaluatorAsync(Guid.NewGuid(), erasePlan);

            var eraser = (IConditionEraser)spec;

            Fake.ClearRecordedCalls(eventMonitorSink);
            Fake.ClearRecordedCalls(erasePlan);
            Fake.ClearRecordedCalls(condition);

            Assume.That(evaluator != null);

            // Act
            await eraser.EraseAsync();

            // Assert
            A.CallTo(() => eventMonitorSink.RemoveEvaluator(typeof(Event), evaluator)).MustHaveHappened();
        }

        [Test]
        public void Given_SpecThatHasntBuiltEvaluator_When_Erase_Then_Throws()
        {
            // Arrange
            var eventMonitorSink = A.Fake<IEventMonitorSink>();
            var condition = A.Dummy<Func<Event, bool>>();

            IConditionSpecification spec = new EventOccurredConditionSpecification<Event>(eventMonitorSink, condition);

            var eraser = (IConditionEraser)spec;

            Fake.ClearRecordedCalls(eventMonitorSink);
            Fake.ClearRecordedCalls(condition);

            // Act
            Assert.ThrowsAsync<InvalidOperationException>(() => eraser.EraseAsync());
        }
    }
}
