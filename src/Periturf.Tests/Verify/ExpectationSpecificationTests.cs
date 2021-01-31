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

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ExpectationSpecificationTests
    {
        private readonly ConditionIdentifier _condition1 = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());
        private readonly FeedConditionInstance _feedCondition1;
        private readonly ConditionIdentifier _condition2 = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());
        private readonly FeedConditionInstance _feedCondition2;
        private ExpectationSpecification _sut;

        public ExpectationSpecificationTests()
        {
            _feedCondition1 = new FeedConditionInstance(_condition1, new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));
            _feedCondition2 = new FeedConditionInstance(_condition2, new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));
        }

        [SetUp]
        public void SetUp()
        {
            _sut = new ExpectationSpecification();
        }

        [Test]
        public void Given_Constraint_When_Build_Then_EvaluatorHasConstraint()
        {
            _sut.Constraint(x => x.Condition(_condition1));

            var evaluator = _sut.Build();

            var result = evaluator.Evaluate(_feedCondition1);

            Assume.That(result, Is.Not.Null);
            Assert.That(result.IsCompleted, Is.True);
            Assert.That(result.Met, Is.True);
        }

        [Test]
        public void Given_MultipleNoTimeConstraint_When_Build_Then_EvaluatorHasConstraints()
        {
            _sut.Constraint(x =>
            {
                x.Condition(_condition1);
                x.Condition(_condition2);
            });

            var evaluator = _sut.Build();

            var result = evaluator.Evaluate(_feedCondition1);
            var result2 = evaluator.Evaluate(_feedCondition2);

            Assume.That(result, Is.Not.Null);
            Assume.That(result2, Is.Not.Null);

            Assert.That(result.IsCompleted, Is.False);
            Assert.That(result.Met, Is.Null);
            Assert.That(result2.IsCompleted, Is.True);
            Assert.That(result2.Met, Is.True);
        }

        [Test]
        public void Given_OrderedConstraints_When_Build_Then_EvaluatorHasConstraints()
        {
            _sut.Constraint(x => x.Condition(_condition1));
            _sut.Then(t => t.Constraint(c => c.Condition(_condition2)));

            var evaluator = _sut.Build();

            var result = evaluator.Evaluate(_feedCondition2);
            var result2 = evaluator.Evaluate(_feedCondition1);
            var result3 = evaluator.Evaluate(_feedCondition1);
            var result4 = evaluator.Evaluate(_feedCondition2);

            Assume.That(result, Is.Not.Null);
            Assume.That(result2, Is.Not.Null);
            Assume.That(result3, Is.Not.Null);
            Assume.That(result4, Is.Not.Null);

            Assert.That(result.IsCompleted, Is.False);
            Assert.That(result.Met, Is.Null);
            Assert.That(result2.IsCompleted, Is.False);
            Assert.That(result2.Met, Is.Null);
            Assert.That(result3.IsCompleted, Is.False);
            Assert.That(result3.Met, Is.Null);
            Assert.That(result4.IsCompleted, Is.True);
            Assert.That(result4.Met, Is.True);
        }
    }
}
