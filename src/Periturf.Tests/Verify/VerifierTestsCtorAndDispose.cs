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
        private IConditionSpecification _spec1;
        private IConditionFeed _feed1;

        private IExpectationEvaluator _expectationEvaluator;
        
        private Verifier _sut;

        [SetUp]
        public void SetUp()
        {
            _feed1 = A.Fake<IConditionFeed>();
            A.CallTo(() => _feed1.WaitForInstancesAsync(A<CancellationToken>._)).Returns(new List<ConditionInstance> { new ConditionInstance(TimeSpan.FromSeconds(1), "ID") });

            _spec1 = A.Fake<IConditionSpecification>();
            A.CallTo(() => _spec1.BuildAsync(A<IConditionInstanceFactory>._, A<CancellationToken>._)).Returns(_feed1);

            _expectationEvaluator = A.Fake<IExpectationEvaluator>();
            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>._)).Returns(new ExpectationResult(true, true));
            var expectationSpec = A.Fake<IExpectationSpecification>();
            A.CallTo(() => expectationSpec.Build()).Returns(_expectationEvaluator);

            _sut = new Verifier(
                TimeSpan.FromMilliseconds(500),
                new List<(ConditionIdentifier, IConditionSpecification)> { (_feed1Id, _spec1) },
                expectationSpec);
        }

        [Test]
        public async Task Given_MetExpectations_When_Verify_Then_FeedCreatedAndResultAsExpectedAndFeedDisposed()
        {
            var result = await _sut.VerifyAsync(CancellationToken.None);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.AsExpected, Is.True);
            A.CallTo(() => _spec1.BuildAsync(A<IConditionInstanceFactory>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly().Then(
            A.CallTo(() => _expectationEvaluator.Evaluate(A<FeedConditionInstance>._)).MustHaveHappened()).Then(
            A.CallTo(() => _feed1.DisposeAsync()).MustHaveHappened());
        }
    }
}
