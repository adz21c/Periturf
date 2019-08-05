/*
 *     Copyright 2019 Adam Burton (adz21c@gmail.com)
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
using Periturf.Verify.Evaluators.Logical;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify.Evaluators.Logical
{
    [TestFixture]
    class XorConditionSpecificationTests
    {
        [Test]
        public async Task Given_ChildConditions_When_BuildEvaluator_Then_CreateChildEvaluatorsAndReturnParent()
        {
            // Arrange
            var id = Guid.NewGuid();

            var evaluator = A.Dummy<IConditionEvaluator>();
            var condition = A.Fake<IConditionSpecification>();
            A.CallTo(() => condition.BuildEvaluatorAsync(A<Guid>._, A<IConditionErasePlan>._, A<CancellationToken>._)).Returns(evaluator);

            var evaluator2 = A.Dummy<IConditionEvaluator>();
            var condition2 = A.Fake<IConditionSpecification>();
            A.CallTo(() => condition2.BuildEvaluatorAsync(A<Guid>._, A<IConditionErasePlan>._, A<CancellationToken>._)).Returns(evaluator2);

            var spec = new XorConditionSpecification(new List<IConditionSpecification> { condition, condition2 });

            var erasePlan = A.Dummy<IConditionErasePlan>();

            // Act
            var parentEvaluator = await spec.BuildEvaluatorAsync(id, erasePlan);

            // Assert
            Assert.IsNotNull(parentEvaluator);
            Assert.AreEqual(typeof(XorConditionEvaluator), parentEvaluator.GetType());
            A.CallTo(() => condition.BuildEvaluatorAsync(id, A<IConditionErasePlan>._, A<CancellationToken>._)).MustHaveHappened();
            A.CallTo(() => condition2.BuildEvaluatorAsync(id, A<IConditionErasePlan>._, A<CancellationToken>._)).MustHaveHappened();
        }
    }
}
