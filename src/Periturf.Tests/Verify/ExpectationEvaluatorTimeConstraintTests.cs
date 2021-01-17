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
    class ExpectationEvaluatorTimeConstraintTests
    {
        private readonly ConditionIdentifier _condition1 = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());
        private readonly ConditionIdentifier _condition2 = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());

        [Test]
        public void A1()
        {
            var timeConstraint = TimeSpan.FromMilliseconds(100);

            var sut = new ExpectationEvaluator(
                new List<ExpectationConstraintEvaluator>
                {
                    new ExpectationConstraintEvaluator(_condition1, null, timeConstraint)
                },
                null);

            Assert.That(sut.NextTimeout, Is.EqualTo(timeConstraint));
        }

        [Test]
        public void B()
        {
            var timeConstraint = TimeSpan.FromMilliseconds(100);
            var timeConstraint2 = TimeSpan.FromMilliseconds(200);

            var sut = new ExpectationEvaluator(
                new List<ExpectationConstraintEvaluator>
                {
                    new ExpectationConstraintEvaluator(_condition1, null, timeConstraint),
                    new ExpectationConstraintEvaluator(_condition2, null, timeConstraint2)
                },
                null);

            Assume.That(sut.NextTimeout, Is.EqualTo(timeConstraint));

            var feedInstance = new FeedConditionInstance(
                _condition1,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));

            sut.Evaluate(feedInstance);

            Assert.That(sut.NextTimeout, Is.EqualTo(timeConstraint2));
        }

        [Test]
        public void C()
        {
            var timeConstraint = TimeSpan.FromMilliseconds(100);

            var sut = new ExpectationEvaluator(
                new List<ExpectationConstraintEvaluator>
                {
                    new ExpectationConstraintEvaluator(_condition1, null, timeConstraint)
                },
                null);

            Assume.That(sut.NextTimeout, Is.EqualTo(timeConstraint));

            var feedInstance = new FeedConditionInstance(
                _condition1,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));

            sut.Evaluate(feedInstance);

            Assert.That(sut.NextTimeout, Is.Null);
        }
    }
}