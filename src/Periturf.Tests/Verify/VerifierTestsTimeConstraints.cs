/*
 *     Copyright 2021 Adam Burton (adz21c@gmail.com)
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
using NUnit.Framework;
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class VerifierTestsTimeConstraints
    {
        private readonly ConditionIdentifier _feed1Id = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());
        private IConditionFeed _feed1;
        private IConditionSpecification _spec1;

        private readonly ConditionIdentifier _feed2Id = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());
        private IConditionFeed _feed2;
        private IConditionSpecification _spec2;

        private IExpectationEvaluator _expectationEvaluator;

        private readonly TimeSpan _inactivityTimeout = TimeSpan.FromMilliseconds(200);

        private Verifier _sut;

        [SetUp]
        public void SetUp()
        {
            _feed1 = A.Fake<IConditionFeed>();
            A.CallTo(() => _feed1.WaitForInstancesAsync(A<CancellationToken>._)).ReturnsLazily(async () =>
            {
                await Task.Delay(1000);
                return new List<ConditionInstance> { new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID") };
            });
            _spec1 = A.Fake<IConditionSpecification>();
            A.CallTo(() => _spec1.BuildAsync(A<IConditionInstanceFactory>._, A<CancellationToken>._)).Returns(_feed1);

            _feed2 = A.Fake<IConditionFeed>();
            A.CallTo(() => _feed2.WaitForInstancesAsync(A<CancellationToken>._)).ReturnsLazily(async () =>
            {
                await Task.Delay(1000);
                return new List<ConditionInstance> { new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID") };
            });
            _spec2 = A.Fake<IConditionSpecification>();
            A.CallTo(() => _spec2.BuildAsync(A<IConditionInstanceFactory>._, A<CancellationToken>._)).Returns(_feed2);

            _expectationEvaluator = A.Fake<IExpectationEvaluator>();
            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>._)).Returns(new ExpectationResult(true, true));
            var expectationSpec = A.Fake<IExpectationSpecification>();
            A.CallTo(() => expectationSpec.Build()).Returns(_expectationEvaluator);

            _sut = new Verifier(
                _inactivityTimeout,
                new List<(ConditionIdentifier, IConditionSpecification)> { (_feed1Id, _spec1), (_feed2Id, _spec2) },
                expectationSpec);
        }

        [Test]
        public async Task Given_Timer_When_TimerReached_Then_EvaluateTimer()
        {
            var timer = TimeSpan.FromMilliseconds(101);
            A.CallTo(() => _expectationEvaluator.NextTimer).Returns(timer);
            A.CallTo(() => _expectationEvaluator.Evaluate(timer)).Returns(new ExpectationResult(true, false));

            var watch = Stopwatch.StartNew();
            var result = await _sut.VerifyAsync(CancellationToken.None);
            watch.Stop();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AsExpected, Is.False);
            Assert.That(watch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(timer.TotalMilliseconds));
            A.CallTo(() => _expectationEvaluator.Evaluate(timer)).MustHaveHappened();
        }

        [Test]
        public async Task Given_InactivityTimer_When_InactivityTimerReached_Then_Timeout()
        {
            A.CallTo(() => _expectationEvaluator.Timeout()).Returns(new ExpectationResult(true, false));

            var watch = Stopwatch.StartNew();
            var result = await _sut.VerifyAsync(CancellationToken.None);
            watch.Stop();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AsExpected, Is.False);
            Assert.That(watch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(_inactivityTimeout.TotalMilliseconds));
            A.CallTo(() => _expectationEvaluator.Timeout()).MustHaveHappened();
        }

        [Test]
        public async Task Given_TimerAndInactivityTimer_When_InactivityTimerReached_Then_EvaluateTimerThenTimeout()
        {
            var timer = TimeSpan.FromMilliseconds(101);
            A.CallTo(() => _expectationEvaluator.NextTimer).ReturnsNextFromSequence(timer, new TimeSpan?());
            A.CallTo(() => _expectationEvaluator.Evaluate(timer)).Returns(new ExpectationResult(false, null));
            A.CallTo(() => _expectationEvaluator.Timeout()).Returns(new ExpectationResult(true, false));

            var watch = Stopwatch.StartNew();
            var result = await _sut.VerifyAsync(CancellationToken.None);
            watch.Stop();

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AsExpected, Is.False);
            Assert.That(watch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo((_inactivityTimeout + timer).TotalMilliseconds));
            A.CallTo(() => _expectationEvaluator.Evaluate(timer)).MustHaveHappened().Then(
                A.CallTo(() => _expectationEvaluator.Timeout()).MustHaveHappened());
        }
    }
}
