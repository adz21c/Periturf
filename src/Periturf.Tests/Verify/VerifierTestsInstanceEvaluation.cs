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
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class VerifierTestsInstanceEvaluation
    {
        private readonly ConditionIdentifier _feed1Id = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());
        private IConditionFeed _feed1;
        private IConditionSpecification _spec1;

        private readonly ConditionIdentifier _feed2Id = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());
        private IConditionFeed _feed2;
        private IConditionSpecification _spec2;

        private IExpectationEvaluator _expectationEvaluator;
        
        private Verifier _sut;

        [SetUp]
        public void SetUp()
        {
            _feed1 = A.Fake<IConditionFeed>();
            A.CallTo(() => _feed1.WaitForInstancesAsync(A<CancellationToken>._)).Returns(new List<ConditionInstance> { new ConditionInstance(TimeSpan.FromSeconds(1), "ID") });
            _spec1 = A.Fake<IConditionSpecification>();
            A.CallTo(() => _spec1.BuildAsync(A<IConditionInstanceFactory>._, A<CancellationToken>._)).Returns(_feed1);

            _feed2 = A.Fake<IConditionFeed>();
            A.CallTo(() => _feed2.WaitForInstancesAsync(A<CancellationToken>._)).Returns(new List<ConditionInstance> { new ConditionInstance(TimeSpan.FromSeconds(2), "ID") });
            _spec2 = A.Fake<IConditionSpecification>();
            A.CallTo(() => _spec2.BuildAsync(A<IConditionInstanceFactory>._, A<CancellationToken>._)).Returns(_feed2);

            _expectationEvaluator = A.Fake<IExpectationEvaluator>();
            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>._)).Returns(new ExpectationResult(true, true));
            var expectationSpec = A.Fake<IExpectationSpecification>();
            A.CallTo(() => expectationSpec.Build()).Returns(_expectationEvaluator);

            _sut = new Verifier(
                TimeSpan.FromMilliseconds(500),
                new List<(ConditionIdentifier, IConditionSpecification)> { (_feed1Id, _spec1), (_feed2Id, _spec2) },
                expectationSpec);
        }

        [Test]
        public async Task Given_MetExpectations_When_Verify_Then_ResultAsExpectedAndFeedDisposed()
        {
            var result = await _sut.VerifyAsync(CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AsExpected, Is.True);
            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>._)).MustHaveHappened();
            A.CallTo(() => _feed1.DisposeAsync()).MustHaveHappened();
            A.CallTo(() => _feed2.DisposeAsync()).MustHaveHappened();
        }

        [Test]
        public async Task Given_MultipleFeedsAndInstances_When_Verify_Then_VerifiedInOrder()
        {
            var instance1 = new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1");
            var instance2 = new ConditionInstance(TimeSpan.FromMilliseconds(200), "ID2");
            var instance3 = new ConditionInstance(TimeSpan.FromMilliseconds(300), "ID3");

            A.CallTo(() => _feed1.WaitForInstancesAsync(A<CancellationToken>._))
                .Returns(new List<ConditionInstance> { instance1, instance3 });

            A.CallTo(() => _feed2.WaitForInstancesAsync(A<CancellationToken>._))
                .Returns(new List<ConditionInstance> { instance2 });

            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>._)).ReturnsNextFromSequence(
                new ExpectationResult(false, null),
                new ExpectationResult(false, null),
                new ExpectationResult(true, true));

            var result = await _sut.VerifyAsync(CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AsExpected, Is.True);

            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>.That.NullCheckedMatches(
                    i => i.Instance == instance1 && i.Identifier == _feed1Id,
                    e => e.Write("Expectation1")))).MustHaveHappenedOnceExactly()
                .Then(A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>.That.NullCheckedMatches(
                        i => i.Instance == instance2 && i.Identifier == _feed2Id,
                        e => e.Write("Expectation2")))).MustHaveHappenedOnceExactly())
                .Then(A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>.That.NullCheckedMatches(
                        i => i.Instance == instance3 && i.Identifier == _feed1Id,
                        e => e.Write("Expectation3")))).MustHaveHappenedOnceExactly());
        }

        [Test]
        public async Task Given_InstanceTimeSpanZero_When_Verify_Then_NotEvaluated()
        {
            var instance1 = new ConditionInstance(TimeSpan.Zero, "ID1");
            var instance2 = new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID2");

            A.CallTo(() => _feed1.WaitForInstancesAsync(A<CancellationToken>._))
                .Returns(new List<ConditionInstance> { instance1 });

            A.CallTo(() => _feed2.WaitForInstancesAsync(A<CancellationToken>._))
                .Returns(new List<ConditionInstance> { instance2 });

            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>._)).Returns(new ExpectationResult(true, false));

            await _sut.VerifyAsync(CancellationToken.None);

            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>.That.NullCheckedMatches(
                    i => i.Instance == instance1,
                    e => e.Write("Expectation1")))).MustNotHaveHappened();
            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>.That.NullCheckedMatches(
                    i => i.Instance == instance2,
                    e => e.Write("Expectation2")))).MustHaveHappenedOnceExactly();
        }

        [Test, Ignore("Need to fix broken test")]
        public async Task Given_IncompleteTask_When_VerifyComplete_Then_TaskCancelled()
        {
            var instance1 = new ConditionInstance(TimeSpan.FromSeconds(1), "ID1");

            A.CallTo(() => _feed1.WaitForInstancesAsync(A<CancellationToken>._))
                .Returns(new List<ConditionInstance> { instance1 });
            
            CancellationToken cancellationToken = CancellationToken.None;
            A.CallTo(() => _feed2.WaitForInstancesAsync(A<CancellationToken>._))
                .Invokes(async (CancellationToken ct) =>
                {
                    cancellationToken = ct;
                    await Task.Delay(1000, ct);
                });

            var result = await _sut.VerifyAsync(CancellationToken.None);

            Assume.That(result, Is.Not.Null);
            Assume.That(result.AsExpected, Is.True);
            Assert.That(cancellationToken.IsCancellationRequested, Is.True);
        }
    }
}
