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
    class XorConditionEvaluatorTests
    {
        [TestCase(true, true, false)]
        [TestCase(false, true, true)]
        [TestCase(true, false, true)]
        [TestCase(false, false, false)]
        public async Task Given_ChildConditions_When_Evaluate_Then_ExpectedResult(bool conditionResult, bool condition2Result, bool parentConditionResult)
        {
            // Arrange
            var id = Guid.NewGuid();

            var evaluator = A.Dummy<IConditionEvaluator>();
            A.CallTo(() => evaluator.EvaluateAsync(A<CancellationToken>._)).Returns(conditionResult);

            var evaluator2 = A.Dummy<IConditionEvaluator>();
            A.CallTo(() => evaluator2.EvaluateAsync(A<CancellationToken>._)).Returns(condition2Result);

            var spec = new XorConditionEvaluator(new List<IConditionEvaluator> { evaluator, evaluator2 });

            // Act
            var parentResult = await spec.EvaluateAsync();

            // Assert
            Assert.AreEqual(parentConditionResult, parentResult);
        }
    }
}
