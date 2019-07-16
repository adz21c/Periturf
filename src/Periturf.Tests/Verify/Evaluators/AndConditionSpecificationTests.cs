﻿/*
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
using Periturf.Verify.Evaluators;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify.Evaluators
{
    [TestFixture]
    class AndConditionSpecificationTests
    {
        [Test]
        public async Task Given_ChildConditions_When_BuildEvaluator_Then_CreateChildEvaluatorsAndReturnParent()
        {
            // Arrange
            var id = Guid.NewGuid();

            var evaluator = A.Dummy<IConditionEvaluator>();
            var condition = A.Fake<IConditionSpecification>();
            A.CallTo(() => condition.BuildEvaluatorAsync(A<Guid>._, A<CancellationToken>._)).Returns(evaluator);

            var evaluator2 = A.Dummy<IConditionEvaluator>();
            var condition2 = A.Fake<IConditionSpecification>();
            A.CallTo(() => condition2.BuildEvaluatorAsync(A<Guid>._, A<CancellationToken>._)).Returns(evaluator2);

            var spec = new AndConditionSpecification(new List<IConditionSpecification> { condition, condition2 });

            // Act
            var parentEvaluator = await spec.BuildEvaluatorAsync(id);

            // Assert
            Assert.IsNotNull(parentEvaluator);
            Assert.AreEqual(typeof(AndConditionEvaluator), parentEvaluator.GetType());
            A.CallTo(() => condition.BuildEvaluatorAsync(id, A<CancellationToken>._)).MustHaveHappened();
            A.CallTo(() => condition2.BuildEvaluatorAsync(id, A<CancellationToken>._)).MustHaveHappened();
        }
    }
}
