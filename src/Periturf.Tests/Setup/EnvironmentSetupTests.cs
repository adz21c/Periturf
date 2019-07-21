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

            // Act
            var environment = Environment.Setup(x =>
            {
                x.Host(nameof(host), host);
            });

            // Assert
            Assert.IsNotNull(environment);
        }

        [TestCase(null, Description = "Null Host Name")]
        [TestCase("", Description = "Empty Host Name")]
        [TestCase(" ", Description = "Whitespace Host Name")]
        public void Given_BadHostName_When_Setup_Then_ThrowException(string hostName)
        {
            // Arrange
            var component = A.Dummy<IComponent>();
            var host = A.Dummy<IHost>();
            A.CallTo(() => host.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { "component", component } }));

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => Environment.Setup(x =>
            {
                x.Host(hostName, host);
            }));

            // Assert
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Given_NullHost_When_Setup_Then_ThrowException()
        {
            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => Environment.Setup(x =>
            {
                x.Host("Host", null);
            }));

            // Assert
            Assert.AreEqual("host", exception.ParamName);
        }

        [Test]
        public void Given_MultipleHosts_When_Setup_Then_EnvironmentCreated()
        {
            // Arrange
            var component = A.Dummy<IComponent>();
            var host = A.Dummy<IHost>();
            A.CallTo(() => host.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(component), component } }));

            var component2 = A.Dummy<IComponent>();
            var host2 = A.Dummy<IHost>();
            A.CallTo(() => host2.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(component2), component2 } }));

            // Act
            var environment = Environment.Setup(x =>
            {
                x.Host(nameof(host), host);
                x.Host(nameof(host2), host2);
            });

            // Assert
            Assert.IsNotNull(environment);
        }

        [Test]
        public void Given_MultipleHostsWithTheSameName_When_Setup_Then_ThrowException()
        {
            // Arrange
            var component = A.Dummy<IComponent>();
            var host = A.Dummy<IHost>();
            A.CallTo(() => host.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(component), component } }));

            var component2 = A.Dummy<IComponent>();
            var host2 = A.Dummy<IHost>();
            A.CallTo(() => host2.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(component2), component2 } }));

            // Act
            var exception = Assert.Throws<DuplicateHostNameException>(() => Environment.Setup(x =>
            {
                x.Host(nameof(host), host);
                x.Host(nameof(host), host2);
            }));

            // Assert
            Assert.AreEqual(nameof(host), exception.HostName);
        }


        [Test]
        public void Given_MultipleComponentsWithTheSameName_When_Setup_Then_ThrowException()
        {
            // Arrange
            var component = A.Dummy<IComponent>();
            var host = A.Dummy<IHost>();
            A.CallTo(() => host.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(component), component } }));

            var component2 = A.Dummy<IComponent>();
            var host2 = A.Dummy<IHost>();
            A.CallTo(() => host2.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(component), component2 } }));

            // Act
            var exception = Assert.Throws<DuplicateComponentNameException>(() => Environment.Setup(x =>
            {
                x.Host(nameof(host), host);
                x.Host(nameof(host2), host2);
            }));

            // Assert
            Assert.AreEqual(nameof(component), exception.ComponentName);
        }
    }
}
