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
    class EnvironmentVerifyTests
    {
        private IConditionEvaluator _evaluator;
        private ITestComponentConditionBuilder _componentConditionBuilder;
        private IConditionSpecification _specification;
        private IComponent _component;
        private Environment _environment;
        private Guid _verifyId;
        private IVerifier _verifier;

        [SetUp]
        public async Task SetUpAsync()
        {
            // Arrange
            _evaluator = A.Fake<IConditionEvaluator>();

            _specification = A.Fake<IConditionSpecification>();
            A.CallTo(() => _specification.BuildEvaluatorAsync(A<Guid>._, A<CancellationToken>._))
                .Invokes((Guid id, CancellationToken ct) => _verifyId = id)
                .Returns(_evaluator);

            _componentConditionBuilder = A.Fake<ITestComponentConditionBuilder>();
            A.CallTo(() => _componentConditionBuilder.CreateSpecification()).Returns(_specification);

            _component = A.Fake<IComponent>();
            A.CallTo(() => _component.CreateConditionBuilder()).Returns(_componentConditionBuilder);

            var host1 = A.Fake<IHost>();
            A.CallTo(() => host1.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(_component), _component } }));

            _environment = Environment.Setup(x =>
            {
                x.Host(nameof(host1), host1);
            });

            _verifier = await _environment.VerifyAsync(c =>
                c.GetComponentConditionBuilder<ITestComponentConditionBuilder>(nameof(_component))
                    .CreateSpecification());
        }

        [Test]
        public void Given_TrueCondition_When_VerifyAndThrowAsync_Then_DoesNotThrow()
        {
            // Arrange
            A.CallTo(() => _evaluator.EvaluateAsync(A<CancellationToken>._)).Returns(true);

            // Act
            Assert.DoesNotThrow(() => _verifier.VerifyAndThrowAsync());

            // Assert
            A.CallTo(() => _evaluator.EvaluateAsync(A<CancellationToken>._)).MustHaveHappened();
        }

        [Test]
        public void Given_FalseCondition_When_VerifyAndThrowAsync_Then_ThrowsException()
        {
            // Arrange
            A.CallTo(() => _evaluator.EvaluateAsync(A<CancellationToken>._)).Returns(false);

            // Act
            Assert.ThrowsAsync<VerificationFailedException>(() => _verifier.VerifyAndThrowAsync());

            // Assert
            A.CallTo(() => _evaluator.EvaluateAsync(A<CancellationToken>._)).MustHaveHappened();
        }
    }
}
