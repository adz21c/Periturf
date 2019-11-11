using FakeItEasy;
using IdentityServer4.Events;
using NUnit.Framework;
using Periturf.IdSvr4.Verify;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.Tests.IdSvr4.Verify
{
    [TestFixture]
    class EventOccurredConditionTests
    {
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
            var evaluator2 = await spec.BuildAsync();

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
    }
}
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              