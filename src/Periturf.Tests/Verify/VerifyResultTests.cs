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
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class VerifyResultTests
    {
        private IExpectationCriteriaEvaluator _expectationCriteriaEvaluator1;
        private IExpectationCriteriaSpecification _expectationCriteriaSpec1;
        private IExpectationCriteriaEvaluator _expectationCriteriaEvaluator2;
        private IExpectationCriteriaSpecification _expectationCriteriaSpec2;
        private string _componentName;
        private Environment _environment;

        [SetUp]
        public void SetUp()
        {
            ITestComponentConditionBuilder CreateConditionBuilder()
            {
                var componentConditionEvaluator = A.Fake<IComponentConditionEvaluator>();
                A.CallTo(() => componentConditionEvaluator.GetInstances())
                    .Returns(new ConditionInstance[] { new ConditionInstance(TimeSpan.FromMilliseconds(1), "ID") }.AsAsyncEnumerable());

                var componentConditionSpecification = A.Fake<IComponentConditionSpecification>();
                A.CallTo(() => componentConditionSpecification.BuildAsync(A<CancellationToken>._))
                    .Returns(componentConditionEvaluator);

                var componentConditionBuilder = A.Fake<ITestComponentConditionBuilder>();
                A.CallTo(() => componentConditionBuilder.CreateCondition()).Returns(componentConditionSpecification);

                return componentConditionBuilder;
            }

            // Arrange
            var component = A.Fake<IComponent>();
            A.CallTo(() => component.CreateConditionBuilder<ITestComponentConditionBuilder>()).ReturnsNextFromSequence(
                CreateConditionBuilder(),
                CreateConditionBuilder());
            _componentName = nameof(component);

            var host1 = A.Fake<IHost>();
            A.CallTo(() => host1.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(component), component } }));

            _expectationCriteriaEvaluator1 = A.Fake<IExpectationCriteriaEvaluator>();
            _expectationCriteriaSpec1 = A.Fake<IExpectationCriteriaSpecification>();
            A.CallTo(() => _expectationCriteriaSpec1.Build()).Returns(_expectationCriteriaEvaluator1);

            _expectationCriteriaEvaluator2 = A.Fake<IExpectationCriteriaEvaluator>();
            _expectationCriteriaSpec2 = A.Fake<IExpectationCriteriaSpecification>();
            A.CallTo(() => _expectationCriteriaSpec2.Build()).Returns(_expectationCriteriaEvaluator2);

            _environment = Environment.Setup(x =>
            {
                x.Host(nameof(host1), host1);
            });
        }

        [Test]
        public async Task Given_AllEvaluatorsMet_When_Verify_Then_ExpectationsMet()
        {
            A.CallTo(() => _expectationCriteriaEvaluator1.Met).Returns(true);
            A.CallTo(() => _expectationCriteriaEvaluator2.Met).Returns(true);

            var verifier = await _environment.VerifyAsync(c =>
            {
                c.Expect(
                     c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(_componentName).CreateCondition(),
                     e => e.Must(_expectationCriteriaSpec1));
                c.Expect(
                     c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(_componentName).CreateCondition(),
                     e => e.Must(_expectationCriteriaSpec2));
            });

            Assert.IsNotNull(verifier);

            var result = await verifier.VerifyAsync();

            Assert.IsNotNull(result);
            Assert.IsTrue(result.ExpectationsMet);
            Assert.IsNotEmpty(result.ExpectationResults);
            Assert.AreEqual(2, result.ExpectationResults.Count);
        }

        [Test]
        public async Task Given_NotAllEvaluatorsMet_When_Verify_Then_ExpectationsNotMet()
        {
            A.CallTo(() => _expectationCriteriaEvaluator1.Met).Returns(true);
            A.CallTo(() => _expectationCriteriaEvaluator2.Met).Returns(false);

            var verifier = await _environment.VerifyAsync(c =>
            {
                c.Expect(
                     c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(_componentName).CreateCondition(),
                     e => e.Must(_expectationCriteriaSpec1));
                c.Expect(
                     c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(_componentName).CreateCondition(),
                     e => e.Must(_expectationCriteriaSpec2));
            });

            Assert.IsNotNull(verifier);

            var result = await verifier.VerifyAsync();

            Assert.IsNotNull(result);
            Assert.IsFalse(result.ExpectationsMet);
            Assert.IsNotEmpty(result.ExpectationResults);
            Assert.AreEqual(2, result.ExpectationResults.Count);
        }
    }
}
