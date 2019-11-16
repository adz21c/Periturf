using FakeItEasy;
using IdentityServer4.Events;
using NUnit.Framework;
using Periturf.IdSvr4.Verify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.Tests.IdSvr4.Verify
{
    [TestFixture]
    class EventOccurredConditionTests
    {
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
            var evaluator = await spec.BuildAsync();

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
            var evaluator = await spec.BuildAsync();
            var evaluator2 = await spec.BuildAsync();

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
            var evaluator = await spec.BuildAsync();
            await spec.BuildAsync();

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
            var evaluator = await spec.BuildAsync();
            var evaluator2 = await spec.BuildAsync();

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
            var evaluator = await spec.BuildAsync();
            var evaluator2 = await spec.BuildAsync();

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
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              