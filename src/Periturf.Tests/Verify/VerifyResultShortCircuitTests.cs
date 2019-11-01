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
using Periturf.Components;
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class VerifyResultShortCircuitTests
    {
        [Test]
        public async Task Given_NotAllEvaluatorsMetAndShortCircuitEnabled_When_Verify_Then_NotAllExpectationsHaveResults()
        {
            var componentConditionEvaluator1 = A.Fake<IComponentConditionEvaluator>();
            A.CallTo(() => componentConditionEvaluator1.GetInstancesAsync())
                .Returns(new ConditionInstance[] { new ConditionInstance(TimeSpan.FromMilliseconds(1), "ID") }.AsAsyncEnumerable());

            var componentConditionSpecification1 = A.Fake<IComponentConditionSpecification>();
            A.CallTo(() => componentConditionSpecification1.BuildAsync(A<CancellationToken>._))
                .Returns(componentConditionEvaluator1);

            var componentConditionBuilder1 = A.Fake<ITestComponentConditionBuilder>();
            A.CallTo(() => componentConditionBuilder1.CreateCondition()).Returns(componentConditionSpecification1);

            async IAsyncEnumerable<ConditionInstance> Evaluator2Enumerable()
            {
                await Task.Delay(5000);
                yield return new ConditionInstance(TimeSpan.FromMilliseconds(1), "ID");
            }

            var componentConditionEvaluator2 = A.Fake<IComponentConditionEvaluator>();
            A.CallTo(() => componentConditionEvaluator2.GetInstancesAsync())
                .ReturnsLazily((f) => Evaluator2Enumerable());

            var componentConditionSpecification2 = A.Fake<IComponentConditionSpecification>();
            A.CallTo(() => componentConditionSpecification2.BuildAsync(A<CancellationToken>._))
                .Returns(componentConditionEvaluator2);

            var componentConditionBuilder2 = A.Fake<ITestComponentConditionBuilder>();
            A.CallTo(() => componentConditionBuilder2.CreateCondition()).Returns(componentConditionSpecification2);

            // Arrange
            var component = A.Fake<IComponent>();
            A.CallTo(() => component.CreateConditionBuilder<ITestComponentConditionBuilder>()).ReturnsNextFromSequence(
                componentConditionBuilder1,
                componentConditionBuilder2);
            var componentName = nameof(component);

            var host1 = A.Fake<IHost>();
            A.CallTo(() => host1.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(component), component } }));

            var expectationCriteriaEvaluator1 = A.Fake<IExpectationCriteriaEvaluator>();
            var expectationCriteriaSpec1 = A.Fake<IExpectationCriteriaSpecification>();
            A.CallTo(() => expectationCriteriaSpec1.Build()).Returns(expectationCriteriaEvaluator1);

            var expectationCriteriaEvaluator2 = A.Fake<IExpectationCriteriaEvaluator>();
            var expectationCriteriaSpec2 = A.Fake<IExpectationCriteriaSpecification>();
            A.CallTo(() => expectationCriteriaSpec2.Build()).Returns(expectationCriteriaEvaluator2);

            var environment = Environment.Setup(x =>
            {
                x.Host(nameof(host1), host1);
            });

            A.CallTo(() => expectationCriteriaEvaluator1.Met).Returns(false);
            A.CallTo(() => expectationCriteriaEvaluator2.Met).Returns(true);

            var verifier = await environment.VerifyAsync(c =>
            {
                c.Expect(
                     c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(componentName).CreateCondition(),
                     e => e.Must(expectationCriteriaSpec1));
                c.Expect(
                     c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(componentName).CreateCondition(),
                     e => e.Must(expectationCriteriaSpec2));
            });

            Assert.IsNotNull(verifier);

            var result = await verifier.VerifyAsync();

            Assert.IsNotNull(result);
            Assert.IsFalse(result.ExpectationsMet);
            Assert.IsNotEmpty(result.ExpectationResults);
            Assert.AreEqual(2, result.ExpectationResults.Count);
            Assert.That(result.ExpectationResults.Any(x => x.Met.HasValue && !x.Met.Value));
            Assert.That(result.ExpectationResults.Any(x => x.Completed));
            Assert.That(!result.ExpectationResults.All(x => x.Completed));
        }
    }
}
