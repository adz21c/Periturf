using FakeItEasy;
using NUnit.Framework;
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class VerifierTestsInstanceEvaluation
    {
        private readonly ConditionIdentifier _feed1Id = new ConditionIdentifier(Guid.NewGuid());
        private IConditionFeed _feed1;

        private readonly ConditionIdentifier _feed2Id = new ConditionIdentifier(Guid.NewGuid());
        private IConditionFeed _feed2;

        private IExpectationEvaluator _expectationEvaluator;
        
        private Verifier _sut;

        [SetUp]
        public void SetUp()
        {
            _feed1 = A.Fake<IConditionFeed>();
            A.CallTo(() => _feed1.WaitForInstancesAsync(A<CancellationToken>._)).Returns(new List<ConditionInstance> { new ConditionInstance(TimeSpan.FromSeconds(1), "ID") });

            _feed2 = A.Fake<IConditionFeed>();
            A.CallTo(() => _feed2.WaitForInstancesAsync(A<CancellationToken>._)).Returns(new List<ConditionInstance> { new ConditionInstance(TimeSpan.FromSeconds(2), "ID") });

            _expectationEvaluator = A.Fake<IExpectationEvaluator>();
            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>._)).Returns(new ExpectationResult { IsCompleted = true, Met = true });

            _sut = new Verifier(
                new List<(ConditionIdentifier ID, IConditionFeed Feed)> { (_feed1Id, _feed1), (_feed2Id, _feed2) },
                _expectationEvaluator);
        }

        [Test]
        public async Task Given_MetExpectations_When_Evaluate_Then_ResultAsExpectedAndFeedDisposed()
        {
            var result = await _sut.EvaluateAsync(CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AsExpected, Is.True);
            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>._)).MustHaveHappened();
            A.CallTo(() => _feed1.DisposeAsync()).MustHaveHappened();
            A.CallTo(() => _feed2.DisposeAsync()).MustHaveHappened();
        }

        [Test]
        public async Task Given_AlreadyEvaluated_When_Evaluate_Then_ReturnsSameResultsDoesntEvaluateAgain()
        {
            var result = await _sut.EvaluateAsync(CancellationToken.None);
            Assume.That(result, Is.Not.Null);
            Fake.ClearRecordedCalls(_feed1);
            Fake.ClearRecordedCalls(_feed2);
            Fake.ClearRecordedCalls(_expectationEvaluator);

            var result2 = await _sut.EvaluateAsync(CancellationToken.None);

            Assert.That(result2, Is.Not.Null);
            Assert.That(result2.AsExpected, Is.EqualTo(result.AsExpected));
            A.CallTo(() => _feed1.WaitForInstancesAsync(A<CancellationToken>._)).MustNotHaveHappened();
            A.CallTo(() => _feed2.WaitForInstancesAsync(A<CancellationToken>._)).MustNotHaveHappened();
            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Given_MultipleFeedsAndInstances_When_Evaluate_Then_EvaluatedInOrder()
        {
            var instance1 = new ConditionInstance(TimeSpan.FromSeconds(1), "ID1");
            var instance2 = new ConditionInstance(TimeSpan.FromSeconds(2), "ID2");
            var instance3 = new ConditionInstance(TimeSpan.FromSeconds(3), "ID3");

            A.CallTo(() => _feed1.WaitForInstancesAsync(A<CancellationToken>._))
                .Returns(new List<ConditionInstance> { instance1, instance3 });

            A.CallTo(() => _feed2.WaitForInstancesAsync(A<CancellationToken>._))
                .Returns(new List<ConditionInstance> { instance2 });

            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>._)).ReturnsNextFromSequence(
                new ExpectationResult { IsCompleted = false },
                new ExpectationResult { IsCompleted = false },
                new ExpectationResult { IsCompleted = true, Met = true });

            await _sut.EvaluateAsync(CancellationToken.None);
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
        public async Task Given_IncompleteTask_When_EvaluateComplete_Then_TaskCancelled()
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

            var result = await _sut.EvaluateAsync(CancellationToken.None);

            Assume.That(result, Is.Not.Null);
            Assume.That(result.AsExpected, Is.True);
            Assert.That(cancellationToken.IsCancellationRequested, Is.True);
        }
    }
}
