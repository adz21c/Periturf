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
    class ExpectationConstraintEvaluatorTests
    {
        private readonly ConditionIdentifier _condition1 = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());
        private readonly ConditionIdentifier _condition2 = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());

        [Test]
        public void Given_Inputs_When_Ctor_Then_Ready()
        {
            var sut = new ExpectationConstraintEvaluator(_condition1);

            Assert.That(sut.Completed, Is.False);
            Assert.That(sut.Met, Is.Null);
        }

        [Test]
        public void Given_MatchingCondition_When_Evaluate_Then_Met()
        {
            var sut = new ExpectationConstraintEvaluator(_condition1);

            Assume.That(sut.Completed, Is.False);
            Assume.That(sut.Met, Is.Null);

            var feedInstance = new FeedConditionInstance(
                _condition1,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));

            sut.Evaluate(feedInstance);

            Assert.That(sut.Completed, Is.True);
            Assert.That(sut.Met, Is.True);
        }

        [Test]
        public void Given_NotMatchingCondition_When_Evaluate_Then_NotCompleted()
        {
            var sut = new ExpectationConstraintEvaluator(_condition1);

            Assume.That(sut.Completed, Is.False);
            Assume.That(sut.Met, Is.Null);

            var feedInstance = new FeedConditionInstance(
                _condition2,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));

            sut.Evaluate(feedInstance);

            Assert.That(sut.Completed, Is.False);
            Assert.That(sut.Met, Is.Null);
        }

        [Test]
        public void Given_Incomplete_When_Timeout_Then_NotMet()
        {
            var sut = new ExpectationConstraintEvaluator(_condition1);

            Assume.That(sut.Completed, Is.False);
            Assume.That(sut.Met, Is.Null);

            sut.Timeout();

            Assert.That(sut.Completed, Is.True);
            Assert.That(sut.Met, Is.False);
        }

        [Test]
        public void Given_CompleteAndMet_When_Timeout_Then_Met()
        {
            var sut = new ExpectationConstraintEvaluator(_condition1);

            Assume.That(sut.Completed, Is.False);
            Assume.That(sut.Met, Is.Null);

            var feedInstance = new FeedConditionInstance(
                _condition1,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));

            sut.Evaluate(feedInstance);

            Assume.That(sut.Completed, Is.True);
            Assume.That(sut.Met, Is.True);

            sut.Timeout();

            Assert.That(sut.Completed, Is.True);
            Assert.That(sut.Met, Is.True);
        }

        [Test]
        public void Given_CompleteAndNotMet_When_Timeout_Then_NotMet()
        {
            var sut = new ExpectationConstraintEvaluator(_condition1, timeConstraintEnd: TimeSpan.FromMilliseconds(150));

            Assume.That(sut.Completed, Is.False);
            Assume.That(sut.Met, Is.Null);

            sut.Evaluate(TimeSpan.FromMilliseconds(200));

            Assume.That(sut.Completed, Is.True);
            Assume.That(sut.Met, Is.False);

            sut.Timeout();

            Assert.That(sut.Completed, Is.True);
            Assert.That(sut.Met, Is.False);
        }
    }
}
