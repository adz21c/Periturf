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
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class EnvironmentVerifyDefinitionAndCleanupTests
    {
        private IConditionEvaluator _evaluator;
        private IConditionEraser _eraser;
        private ITestComponentConditionBuilder _componentConditionBuilder;
        private IConditionSpecification _specification;
        private IComponent _component;
        private Environment _environment;

        [SetUp]
        public void SetUp()
        {
            // Arrange
            _evaluator = A.Fake<IConditionEvaluator>();
            A.CallTo(() => _evaluator.EvaluateAsync(A<CancellationToken>._)).Returns(true);

            _eraser = A.Fake<IConditionEraser>();

            _specification = A.Fake<IConditionSpecification>();
            A.CallTo(() => _specification.BuildEvaluatorAsync(A<Guid>._, A<IConditionErasePlan>._, A<CancellationToken>._))
                .Invokes((Guid id, IConditionErasePlan erasePlan, CancellationToken ct) => erasePlan.AddEraser(_eraser))
                .Returns(_evaluator);

            _componentConditionBuilder = A.Fake<ITestComponentConditionBuilder>();
            A.CallTo(() => _componentConditionBuilder.CreateSpecification()).Returns(_specification);

            _component = A.Fake<IComponent>();
            A.CallTo(() => _component.CreateConditionBuilder<ITestComponentConditionBuilder>()).Returns(_componentConditionBuilder);

            var host1 = A.Fake<IHost>();
            A.CallTo(() => host1.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(_component), _component } }));

            _environment = Environment.Setup(x =>
            {
                x.Host(nameof(host1), host1);
            });
        }

        [Test]
        public async Task Given_Condition_When_Verify_Then_ConditionIsSpecifiedThenBuilt()
        {
            // Act
            await _environment.VerifyAsync(c =>
                c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(nameof(_component))
                    .CreateSpecification());

            // Assert
            A.CallTo(() => _componentConditionBuilder.CreateSpecification()).MustHaveHappened().Then(
                A.CallTo(() => _specification.BuildEvaluatorAsync(A<Guid>._, A<IConditionErasePlan>._, A<CancellationToken>._)).MustHaveHappened());
        }

        [Test]
        public async Task Given_Verifier_When_CleanUp_Then_ComponentUnregistersCondition()
        {
            // Act
            var verifier = await _environment.VerifyAsync(c =>
                c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(nameof(_component))
                    .CreateSpecification());

            await verifier.CleanUpAsync();

            // Assert
            A.CallTo(() => _eraser.EraseAsync(A<CancellationToken>._)).MustHaveHappened();
        }

        [Test]
        public async Task Given_Verifier_When_ErrorDuringCleanUpWithImmediateException_Then_ThrowsException()
        {
            // Arrange
            var erase = A.Fake<IConditionEraser>();
            A.CallTo(() => erase.EraseAsync(A<CancellationToken>._)).Throws(new InvalidOperationException());   // Throws immediately

            A.CallTo(() => _specification.BuildEvaluatorAsync(A<Guid>._, A<IConditionErasePlan>._, A<CancellationToken>._))
                .Invokes((Guid id, IConditionErasePlan plan, CancellationToken ct) =>
                {
                    plan.AddEraser(erase);
                });

            var verifier = await _environment.VerifyAsync(c =>
                c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(nameof(_component))
                    .CreateSpecification());
            // Act
            Assert.ThrowsAsync<VerificationCleanUpFailedException>(() => verifier.CleanUpAsync());
        }


        [Test]
        public async Task Given_Verifier_When_ErrorDuringCleanUpWithAsyncException_Then_ThrowsException()
        {
            // Arrange
            var erase = A.Fake<IConditionEraser>();
            A.CallTo(() => erase.EraseAsync(A<CancellationToken>._)).ThrowsAsync(new InvalidOperationException());   // Throws immediately

            A.CallTo(() => _specification.BuildEvaluatorAsync(A<Guid>._, A<IConditionErasePlan>._, A<CancellationToken>._))
                .Invokes((Guid id, IConditionErasePlan plan, CancellationToken ct) =>
                {
                    plan.AddEraser(erase);
                });

            var verifier = await _environment.VerifyAsync(c =>
                c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(nameof(_component))
                    .CreateSpecification());
            // Act
            Assert.ThrowsAsync<VerificationCleanUpFailedException>(() => verifier.CleanUpAsync());
        }
    }
}
