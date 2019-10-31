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
    class VerifyDefinitionAndCleanupTests
    {
        private IExpectationCriteriaEvaluator _expectationCriteriaEvaluator1;
        private IExpectationCriteriaSpecification _expectationCriteriaSpec1;
        private IExpectationCriteriaEvaluator _expectationCriteriaEvaluator2;
        private IExpectationCriteriaSpecification _expectationCriteriaSpec2;
        private string _componentName;
        private Environment _environment;
        private IComponentConditionSpecification _componentConditionSpecification;
        private IComponentConditionEvaluator _componentConditionEvaluator1;
        private IComponentConditionEvaluator _componentConditionEvaluator2;

        [SetUp]
        public void SetUp()
        {
            _componentConditionEvaluator1 = A.Fake<IComponentConditionEvaluator>();
            _componentConditionEvaluator2 = A.Fake<IComponentConditionEvaluator>();

            _componentConditionSpecification = A.Fake<IComponentConditionSpecification>();
            A.CallTo(() => _componentConditionSpecification.BuildAsync(A<CancellationToken>._))
                .ReturnsNextFromSequence(new[] { _componentConditionEvaluator1, _componentConditionEvaluator2 });

            var componentConditionBuilder = A.Fake<ITestComponentConditionBuilder>();
            A.CallTo(() => componentConditionBuilder.CreateCondition()).Returns(_componentConditionSpecification);

            // Arrange
            var component = A.Fake<IComponent>();
            A.CallTo(() => component.CreateConditionBuilder<ITestComponentConditionBuilder>()).Returns(componentConditionBuilder);
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
        public async Task Given_SharedCondition_When_Verify_Then_SpecificationBuiltTwice()
        {
            var verifier = await _environment.VerifyAsync(c =>
            {
                var sharedCondition = c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(_componentName).CreateCondition();
                c.Expect(
                    sharedCondition,
                    e => e.Must(_expectationCriteriaSpec1));
                c.Expect(
                    sharedCondition,
                    e => e.Must(_expectationCriteriaSpec2));
            });

            Assert.IsNotNull(verifier);

            A.CallTo(() => _componentConditionSpecification.BuildAsync(A<CancellationToken>._)).MustHaveHappenedTwiceExactly();
        }

        [Test]
        public async Task Given_SharedConditionEvaluators_When_DisposeOfVerifier_Then_BothEvaluatorsStopped()
        {
            var verifier = await _environment.VerifyAsync(c =>
            {
                var sharedCondition = c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(_componentName).CreateCondition();
                c.Expect(
                    sharedCondition,
                    e => e.Must(_expectationCriteriaSpec1));
                c.Expect(
                    sharedCondition,
                    e => e.Must(_expectationCriteriaSpec2));
            });

            Assume.That(verifier !is null);

            await verifier.DisposeAsync();

            A.CallTo(() => _componentConditionEvaluator1.DisposeAsync()).MustHaveHappened();
            A.CallTo(() => _componentConditionEvaluator2.DisposeAsync()).MustHaveHappened();
        }
    }
}
