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
    class VerifierTestsCtorAndDispose
    {
        private readonly ConditionIdentifier _feed1Id = new ConditionIdentifier(Guid.NewGuid());
        private IConditionFeed _feed1;

        private IExpectationEvaluator _expectationEvaluator;
        
        private Verifier _sut;

        [SetUp]
        public void SetUp()
        {
            _feed1 = A.Fake<IConditionFeed>();
            A.CallTo(() => _feed1.WaitForInstancesAsync(A<CancellationToken>._)).Returns(new List<ConditionInstance> { new ConditionInstance(TimeSpan.FromSeconds(1), "ID") });

            _expectationEvaluator = A.Fake<IExpectationEvaluator>();
            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>._)).Returns(new ExpectationResult { IsCompleted = true, Met = true });

            _sut = new Verifier(
                new List<(ConditionIdentifier ID, IConditionFeed Feed)> { (_feed1Id, _feed1) },
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
        }

        [Test]
        public async Task Given_NotEvaluated_When_Dispose_Then_FeedsDisposed()
        {
            await _sut.DisposeAsync();
            A.CallTo(() => _feed1.DisposeAsync()).MustHaveHappened();
        }

        [Test]
        public async Task Given_Evaluated_When_Dispose_Then_DoesNotDisposeAgain()
        {
            await _sut.EvaluateAsync(CancellationToken.None);
            Fake.ClearRecordedCalls(_feed1);

            await _sut.DisposeAsync();
            A.CallTo(() => _feed1.DisposeAsync()).MustNotHaveHappened();
        }

        [Test]
        public async Task Given_Disposed_When_Evaluate_Then_Exception()
        {
            await _sut.DisposeAsync();

            Assert.That(() => _sut.EvaluateAsync(CancellationToken.None), Throws.TypeOf<ObjectDisposedException>());
        }
    }
}
