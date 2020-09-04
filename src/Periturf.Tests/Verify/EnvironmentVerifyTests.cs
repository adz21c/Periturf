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
using System.Threading;
using System.Threading.Tasks;
using Periturf.Setup;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class EnvironmentVerifyTests
    {
        private const string ComponentName = "ComponentName";
        
        private IExpectationCriteriaEvaluator _expectationCriteria;
        private IExpectationCriteriaSpecification _expectationSpecification;
        private Environment _environment;

        [SetUp]
        public void SetUp()
        {
            // Expectation
            _expectationCriteria = A.Fake<IExpectationCriteriaEvaluator>();

            var expectationCriteriaFactory = A.Fake<IExpectationCriteriaEvaluatorFactory>();
            A.CallTo(() => expectationCriteriaFactory.CreateInstance()).Returns(_expectationCriteria);

            _expectationSpecification = A.Fake<IExpectationCriteriaSpecification>();
            A.CallTo(() => _expectationSpecification.Build()).Returns(expectationCriteriaFactory);
            A.CallTo(() => _expectationSpecification.Timeout).Returns(TimeSpan.FromMilliseconds(100));

            // Component
            var componentConditionSpecification = A.Fake<IComponentConditionSpecification>();
            A.CallTo(() => componentConditionSpecification.BuildAsync(A<IConditionInstanceTimeSpanFactory>._, A<CancellationToken>._)).Returns(new MockComponentEvaluator(TimeSpan.FromMilliseconds(10), 5));
            
            var componentConditionBuilder = A.Fake<ITestComponentConditionBuilder>();
            A.CallTo(() => componentConditionBuilder.CreateCondition()).Returns(componentConditionSpecification);

            var component = A.Fake<IComponent>();
            A.CallTo(() => component.CreateConditionBuilder<ITestComponentConditionBuilder>()).Returns(componentConditionBuilder);

            // Environment
            var host1 = A.Fake<IHost>();
            A.CallTo(() => host1.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { ComponentName, component } }));

            var hostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => hostSpec.Build()).Returns(host1);

            _environment = Environment.Setup(x =>
            {
                x.AddHostSpecification(hostSpec);
            });
        }

        [Test]
        public async Task Given_MetCondition_When_Verify_Then_VerifyExpectationsMet()
        {
            // Arrange
            var verifier = await _environment.VerifyAsync(c =>
                c.Expect(
                    c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(ComponentName).CreateCondition(),
                    e => e.Must(_expectationSpecification)));
            A.CallTo(() => _expectationCriteria.Met).Returns(true);

            // Act
            var result = await verifier.VerifyAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ExpectationsMet, Is.True);
        }

        [Test]
        public async Task Given_FalseCondition_When_VerifyAndThrowAsync_Then_ThrowsException()
        {
            // Arrange
            var verifier = await _environment.VerifyAsync(c =>
                c.Expect(
                    c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(ComponentName).CreateCondition(),
                    e => e.Must(_expectationSpecification)));
            A.CallTo(() => _expectationCriteria.Met).Returns(false);

            // Act
            var result = await verifier.VerifyAsync();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ExpectationsMet, Is.False);
        }

        [Test]
        public void Given_Environment_When_VerifyWithWrongComponentName_Then_ThrowsException()
        {
            // Arrange
            const string wrongComponentName = "WrongComponentName";

            // Act & Assert
            var exception = Assert.ThrowsAsync<ComponentLocationFailedException>(() => _environment.VerifyAsync(c =>
                c.Expect(
                    c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(wrongComponentName).CreateCondition(),
                    e => e.Must(_expectationSpecification))));
            Assert.That(exception.ComponentName, Is.EqualTo(wrongComponentName));
        }

        [Test]
        public void Given_Zero_When_Timeout_Then_Throws()
        {
            // Act & Assert
            var exception = Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => _environment.VerifyAsync(c =>
                c.Timeout(TimeSpan.Zero)));

            Assert.That(exception.ParamName, Is.EqualTo("timeout"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task Given_GlobalShortCircuit_When_Verify_Then_Matches(bool enabled)
        {
            // Act & Assert
            var verifier = await _environment.VerifyAsync(c =>
            {
                c.ShortCircuit(enabled);
                c.Expect(
                    c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(ComponentName).CreateCondition(),
                    e => e.Must(_expectationSpecification));
            });
            var typedVerifier = verifier as Verifier;

            Assert.That(typedVerifier, Is.Not.Null);
            Assert.That(typedVerifier.ShortCircuit, Is.EqualTo(enabled));
        }

        [Test]
        public async Task Given_GlobalTimeout_When_Verify_Then_Matches()
        {
            var globalTimeout = TimeSpan.FromMilliseconds(5000);
            // Block out other timeouts
            A.CallTo(() => _expectationSpecification.Timeout).Returns(null);


            // Act & Assert
            var verifier = await _environment.VerifyAsync(c =>
            {
                c.Timeout(globalTimeout);
                c.Expect(
                    c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(ComponentName).CreateCondition(),
                    e => e.Must(_expectationSpecification));
            });

            var typedVerifier = verifier as Verifier;

            Assert.That(typedVerifier, Is.Not.Null);
            Assert.That(typedVerifier.Expectations.Count, Is.EqualTo(1));
            Assert.That(typedVerifier.Expectations.Single().Timeout, Is.EqualTo(globalTimeout));
        }
    }
}
