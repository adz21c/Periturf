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
    class VerifierTestsCtorAndDispose
    {
        private readonly ConditionIdentifier _feed1Id = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());
        private IConditionFeed _feed1;

        private IExpectationEvaluator _expectationEvaluator;
        
        private Verifier _sut;

        [SetUp]
        public void SetUp()
        {
            _feed1 = A.Fake<IConditionFeed>();
            A.CallTo(() => _feed1.WaitForInstancesAsync(A<CancellationToken>._)).Returns(new List<ConditionInstance> { new ConditionInstance(TimeSpan.FromSeconds(1), "ID") });

            _expectationEvaluator = A.Fake<IExpectationEvaluator>();
            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>._)).Returns(new ExpectationResult(true, true));

            _sut = new Verifier(
                new List<(ConditionIdentifier ID, IConditionFeed Feed)> { (_feed1Id, _feed1) },
                _expectationEvaluator);
        }

        [Test]
        public async Task Given_MetExpectations_When_Verify_Then_ResultAsExpectedAndFeedDisposed()
        {
            var result = await _sut.VerifyAsync(CancellationToken.None);

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
        public async Task Given_Verifyd_When_Dispose_Then_DoesNotDisposeAgain()
        {
            await _sut.VerifyAsync(CancellationToken.None);
            Fake.ClearRecordedCalls(_feed1);

            await _sut.DisposeAsync();
            A.CallTo(() => _feed1.DisposeAsync()).MustNotHaveHappened();
        }

        [Test]
        public async Task Given_Disposed_When_Verify_Then_Exception()
        {
            await _sut.DisposeAsync();

            Assert.That(() => _sut.VerifyAsync(CancellationToken.None), Throws.TypeOf<ObjectDisposedException>());
        }
    }
}
