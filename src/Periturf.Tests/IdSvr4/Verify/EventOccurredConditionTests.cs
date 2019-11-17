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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Periturf.Tests.IdSvr4.Verify
{
    [TestFixture]
    class EventOccurredConditionTests
    {
        private readonly IConditionInstanceTimeSpanFactory _timeSpanFactory = A.Dummy<IConditionInstanceTimeSpanFactory>();

        [Test]
        public void Given_Evaluator_When_Description_Then_EventTypeName()
        {
            var eventMonitorSink = A.Dummy<IEventMonitorSink>();
            var condition = A.Dummy<Func<Event, bool>>();

            var spec = new EventOccurredConditionSpecification<Event>(eventMonitorSink, condition);

            Assert.AreEqual(typeof(Event).Name, spec.Description);
        }

        [Test]
        public async Task Given_SingleEvaluator_When_Build_Then_MonitorSingleEvaluator()
        {
            var eventMonitorSink = A.Fake<IEventMonitorSink>();
            var condition = A.Dummy<Func<Event, bool>>();

            var spec = new EventOccurredConditionSpecification<Event>(eventMonitorSink, condition);

            // Act
            var evaluator = await spec.BuildAsync(_timeSpanFactory);

            // Assert
            Assert.IsNotNull(evaluator);

            A.CallTo(() => eventMonitorSink.AddEvaluator(typeof(Event), A<IEventOccurredConditionEvaluator>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Given_MultipleEvaluator_When_Build_Then_MonitorSingleEvaluator()
        {
            var eventMonitorSink = A.Fake<IEventMonitorSink>();
            var condition = A.Dummy<Func<Event, bool>>();

            var spec = new EventOccurredConditionSpecification<Event>(eventMonitorSink, condition);

            // Act
            var evaluator = await spec.BuildAsync(_timeSpanFactory);
            var evaluator2 = await spec.BuildAsync(_timeSpanFactory);

            // Assert
            Assert.IsNotNull(evaluator);
            Assert.IsNotNull(evaluator2);
            Assert.AreNotSame(evaluator, evaluator2);

            A.CallTo(() => eventMonitorSink.AddEvaluator(typeof(Event), A<IEventOccurredConditionEvaluator>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Given_MultipleEvaluator_When_DisposeSingleEvaluator_Then_MonitorContinues()
        {
            var eventMonitorSink = A.Fake<IEventMonitorSink>();
            var condition = A.Dummy<Func<Event, bool>>();

            var spec = new EventOccurredConditionSpecification<Event>(eventMonitorSink, condition);

            // Act
            var evaluator = await spec.BuildAsync(_timeSpanFactory);
            await spec.BuildAsync(_timeSpanFactory);

            await evaluator.DisposeAsync();

            // Assert
            A.CallTo(() => eventMonitorSink.AddEvaluator(typeof(Event), A<IEventOccurredConditionEvaluator>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => eventMonitorSink.RemoveEvaluator(typeof(Event), A<IEventOccurredConditionEvaluator>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Given_MultipleEvaluator_When_DisposeAllEvaluators_Then_MonitorCancelled()
        {
            var eventMonitorSink = A.Fake<IEventMonitorSink>();
            var condition = A.Dummy<Func<Event, bool>>();

            var spec = new EventOccurredConditionSpecification<Event>(eventMonitorSink, condition);

            // Act
            var evaluator = await spec.BuildAsync(_timeSpanFactory);
            var evaluator2 = await spec.BuildAsync(_timeSpanFactory);

            await evaluator.DisposeAsync();
            await evaluator2.DisposeAsync();

            // Assert
            A.CallTo(() => eventMonitorSink.AddEvaluator(typeof(Event), A<IEventOccurredConditionEvaluator>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => eventMonitorSink.RemoveEvaluator(typeof(Event), A<IEventOccurredConditionEvaluator>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Given_MultpleEvaluators_When_GetInstances_Then_GetsSameConditionInstances()
        {
            var condition = A.Dummy<Func<Event, bool>>();
            A.CallTo(() => condition.Invoke(A<Event>._)).Returns(true);

            IEventOccurredConditionEvaluator eventOccurredEvaluator = null;
            var eventMonitorSink = A.Fake<IEventMonitorSink>();
            A.CallTo(() => eventMonitorSink.AddEvaluator(typeof(Event), A<IEventOccurredConditionEvaluator>._))
                .Invokes((Type t, IEventOccurredConditionEvaluator e) => eventOccurredEvaluator = e);

            var spec = new EventOccurredConditionSpecification<Event>(eventMonitorSink, condition);

            var evt1 = A.Dummy<Event>();
            var evt2 = A.Dummy<Event>();

            // Act
            var evaluator = await spec.BuildAsync(_timeSpanFactory);
            var evaluator2 = await spec.BuildAsync(_timeSpanFactory);

            await eventOccurredEvaluator.CheckEventAsync(evt1);
            await eventOccurredEvaluator.CheckEventAsync(evt2);

            await evaluator.DisposeAsync();
            await evaluator2.DisposeAsync();

            var results = await evaluator.GetInstancesAsync().ToList();
            var results2 = await evaluator2.GetInstancesAsync().ToList();

            Assert.IsNotEmpty(results);
            Assert.AreEqual(2, results.Count);

            Assert.IsNotEmpty(results2);
            Assert.AreEqual(2, results2.Count);

            Assert.That(results.All(x => results2.Contains(x)));
        }
    }

    static class Helper
    {
        public static async Task<List<T>> ToList<T>(this IAsyncEnumerable<T> enumerable)
        {
            var list = new List<T>();
            await foreach(var item in enumerable)
            {
                list.Add(item);
            }
            return list;
        }
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              