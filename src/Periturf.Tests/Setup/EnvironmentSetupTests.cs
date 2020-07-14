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
using Periturf.Setup;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Periturf.Tests.Setup
{
    [TestFixture]
    class EnvironmentSetupTests
    {
        [Test]
        public void Given_SingleHost_When_Setup_Then_EnvironmentCreated()
        {
            // Arrange
            var component = A.Dummy<IComponent>();
            var host = A.Dummy<IHost>();
            A.CallTo(() => host.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { "component", component } }));
            var hostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => hostSpec.Build()).Returns(host);

            // Act
            var environment = Environment.Setup(x =>
            {
                x.AddHostSpecification(hostSpec);
            });

            // Assert
            Assert.That(environment, Is.Not.Null);
        }

        [Test]
        public void Given_NullSpec_When_Setup_Then_ThrowException()
        {
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => Environment.Setup(x =>
            {
                x.AddHostSpecification(null);
            }));

            // Assert
            Assert.That(exception.ParamName, Is.EqualTo("hostSpecification"));
        }

        [Test]
        public void Given_MultipleHosts_When_Setup_Then_EnvironmentCreated()
        {
            // Arrange
            var component = A.Dummy<IComponent>();
            var host = A.Dummy<IHost>();
            A.CallTo(() => host.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(component), component } }));
            var hostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => hostSpec.Build()).Returns(host);

            var component2 = A.Dummy<IComponent>();
            var host2 = A.Dummy<IHost>();
            A.CallTo(() => host2.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(component2), component2 } }));
            var hostSpec2 = A.Fake<IHostSpecification>();
            A.CallTo(() => hostSpec2.Build()).Returns(host2);

            // Act
            var environment = Environment.Setup(x =>
            {
                x.AddHostSpecification(hostSpec);
                x.AddHostSpecification(hostSpec2);
            });

            // Assert
            Assert.That(environment, Is.Not.Null);
        }

        [Test]
        public void Given_ValidTimeout_When_Setup_Then_Created()
        {
            var env = Environment.Setup(s => s.DefaultExpectationTimeout = TimeSpan.FromMilliseconds(1));

            Assert.That(env, Is.Not.Null);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void Given_ValidShortCircuit_When_Setup_Then_Created(bool shortCircuit)
        {
            var env = Environment.Setup(s => s.DefaultExpectationShortCircuit = shortCircuit);

            Assert.That(env, Is.Not.Null);
        }
    }
}
