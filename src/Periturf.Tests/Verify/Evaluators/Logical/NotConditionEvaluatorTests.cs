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
    class NotConditionEvaluatorTests
    {
        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task Given_ChildConditions_When_Evaluate_Then_ExpectedResult(bool conditionResult, bool parentConditionResult)
        {
            // Arrange
            var id = Guid.NewGuid();

            var evaluator = A.Dummy<IConditionEvaluator>();
            A.CallTo(() => evaluator.EvaluateAsync(A<CancellationToken>._)).Returns(conditionResult);

            var spec = new NotConditionEvaluator(evaluator);

            // Act
            var parentResult = await spec.EvaluateAsync();

            // Assert
            Assert.AreEqual(parentConditionResult, parentResult);
        }
    }
}
