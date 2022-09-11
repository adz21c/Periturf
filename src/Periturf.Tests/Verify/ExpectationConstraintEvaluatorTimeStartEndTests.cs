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
    class ExpectationConstraintEvaluatorTimeStartEndTests
    {
        private readonly ConditionIdentifier _condition1 = new ConditionIdentifier(A.Dummy<string>(), A.Dummy<string>(), Guid.NewGuid());

        [TestCase(90, false, null, Description = "Before upper bound")]
        [TestCase(120, true, true, Description = "On upper bound")]
        [TestCase(180, true, false, Description = "After upper bound")]
        public void Given_TimeConstraintStartEnd_When_EvaluateWithMatch_Then_Result(int instance, bool completed, bool? met)
        {
            var spec = new ExpectationConstraintSpecification();
            spec
                .Condition(_condition1)
                .Between(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(150));
            var sut = spec.Build();

            Assume.That(sut.Completed, Is.False);
            Assume.That(sut.Met, Is.Null);

            var feedInstance = new FeedConditionInstance(
                _condition1,
                new ConditionInstance(TimeSpan.FromMilliseconds(instance), "ID1"));

            sut.Evaluate(feedInstance);

            Assert.That(sut.Completed, Is.EqualTo(completed));
            Assert.That(sut.Met, Is.EqualTo(met));
        }
    }
}
