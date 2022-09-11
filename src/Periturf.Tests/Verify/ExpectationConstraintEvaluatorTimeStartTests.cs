//
//   Copyright 2021 Adam Burton (adz21c@gmail.com)
//   
//   Licensed under the Apache License, Version 2.0 (the "License")
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//

using FakeItEasy;
using NUnit.Framework;
using Periturf.Verify;
using System;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ExpectationConstraintEvaluatorTimeStartTests
    {
        private readonly ConditionIdentifier _condition1 = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());
        private readonly ConditionIdentifier _condition2 = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());

        [TestCase(101, false, null, Description = "Before lower bound")]
        [TestCase(100, true, true, Description = "On lower bound")]
        [TestCase(99, true, true, Description = "After lower bound")]
        public void Given_TimeConstraintStart_When_EvaluateWithMatch_Then_Result(int constraint, bool completed, bool? met)
        {
            var spec = new ExpectationConstraintSpecification();
            spec
                .Condition(_condition1)
                .After(TimeSpan.FromMilliseconds(constraint));
            var sut = spec.Build();

            Assume.That(sut.Completed, Is.False);
            Assume.That(sut.Met, Is.Null);

            var feedInstance = new FeedConditionInstance(
                _condition1,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));

            sut.Evaluate(feedInstance);

            Assert.That(sut.Completed, Is.EqualTo(completed));
            Assert.That(sut.Met, Is.EqualTo(met));
        }

        [TestCase(101, false, null, Description = "Before lower bound")]
        [TestCase(100, false, null, Description = "On lower bound")]
        [TestCase(99, false, null, Description = "After lower bound")]
        public void Given_TimeConstraintStart_When_EvaluateWithoutMatch_Then_Result(int constraint, bool completed, bool? met)
        {
            var spec = new ExpectationConstraintSpecification();
            spec
                .Condition(_condition1)
                .After(TimeSpan.FromMilliseconds(constraint));
            var sut = spec.Build();

            Assume.That(sut.Completed, Is.False);
            Assume.That(sut.Met, Is.Null);

            var feedInstance = new FeedConditionInstance(
                _condition2,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));

            sut.Evaluate(feedInstance);

            Assert.That(sut.Completed, Is.EqualTo(completed));
            Assert.That(sut.Met, Is.EqualTo(met));
        }

        [TestCase(101, Description = "Before lower bound")]
        [TestCase(100, Description = "On lower bound")]
        [TestCase(99, Description = "After lower bound")]
        public void Given_TimeConstraintStart_When_Evaluate_Then_Complete(int constraint)
        {
            var spec = new ExpectationConstraintSpecification();
            spec
                .Condition(_condition1)
                .After(TimeSpan.FromMilliseconds(constraint));
            var sut = spec.Build();

            Assume.That(sut.Completed, Is.False);
            Assume.That(sut.Met, Is.Null);

            sut.Evaluate(TimeSpan.FromMilliseconds(100));

            Assert.That(sut.Completed, Is.False);
            Assert.That(sut.Met, Is.Null);
        }
    }
}
