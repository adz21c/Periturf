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

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ExpectationEvaluatorTests
    {
        private readonly ConditionIdentifier _condition1 = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());
        private readonly ConditionIdentifier _condition2 = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());
        private ExpectationEvaluator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new ExpectationEvaluator(
                new List<ExpectationConstraintEvaluator> { new ExpectationConstraintEvaluator(_condition1) },
                null);
        }

        [Test]
        public void Given_MatchingInputs_When_Evaluate_Then_Met()
        {
            var feedInstance = new FeedConditionInstance(
                _condition1,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));

            var result = _sut.Evaluate(feedInstance);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Met, Is.True);
        }

        [Test]
        public void Given_MatchingInputs_When_Evaluate_Then_NotMet()
        {
            var feedInstance = new FeedConditionInstance(
                _condition2,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID2"));

            var result = _sut.Evaluate(feedInstance);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Met, Is.False);
        }

        [Test]
        public void Given_NotMatchFollowedByMatch_When_Evaluate_Then_Met()
        {
            var feedInstance = new FeedConditionInstance(
                _condition2,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID2"));

            var result = _sut.Evaluate(feedInstance);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Met, Is.False);

            var feedInstance2 = new FeedConditionInstance(
                _condition1,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));

            var result2 = _sut.Evaluate(feedInstance2);

            Assert.That(result2, Is.Not.Null);
            Assert.That(result2.Met, Is.True);
        }


        [Test]
        public void Given_MultipleConstraint_When_Evaluate_Then_MetWhenAllMet()
        {
            var sut = new ExpectationEvaluator(
                new List<ExpectationConstraintEvaluator>
                {
                    new ExpectationConstraintEvaluator(_condition1),
                    new ExpectationConstraintEvaluator(_condition2)
                },
                null);

            var feedInstance = new FeedConditionInstance(
                _condition2,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID2"));

            var result = sut.Evaluate(feedInstance);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Met, Is.False);

            var feedInstance2 = new FeedConditionInstance(
                _condition1,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));

            var result2 = sut.Evaluate(feedInstance2);

            Assert.That(result2, Is.Not.Null);
            Assert.That(result2.Met, Is.True);
        }

        [Test]
        public void Given_Completed_When_Evaluate_Then_SameResult()
        {
            var feedInstance = new FeedConditionInstance(
                _condition1,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));

            var result = _sut.Evaluate(feedInstance);

            Assume.That(result, Is.Not.Null);
            Assume.That(result.Met, Is.True);

            var feedInstance2 = new FeedConditionInstance(
                _condition1,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID2"));

            var result2 = _sut.Evaluate(feedInstance2);

            Assert.That(result2, Is.Not.Null);
            Assert.That(result2.Met, Is.True);

            var feedInstance3 = new FeedConditionInstance(
                _condition2,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID3"));

            var result3 = _sut.Evaluate(feedInstance3);

            Assert.That(result3, Is.Not.Null);
            Assert.That(result3.Met, Is.True);
        }
    }
}
