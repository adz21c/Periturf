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
using Periturf.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Configuration
{
    [TestFixture]
    class EnvironmentRemoveConfigurationTests
    {
        [Test]
        public async Task Given_MultipleComponentsConfigured_When_RemoveConfiguration_Then_ConfigurationUnRegistered()
        {
            // Arrange
            var component1 = A.Fake<IComponent>();
            var componentConfigurator1 = A.Fake<IComponentConfigurator>();
            var host1 = A.Fake<IHost>();
            A.CallTo(() => host1.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(component1), component1 } }));

            var component2 = A.Fake<IComponent>();
            var componentConfigurator2 = A.Fake<IComponentConfigurator>();
            var host2 = A.Fake<IHost>();
            A.CallTo(() => host2.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(component2), component2 } }));

            var environment = Environment.Setup(x =>
            {
                x.Host(nameof(host1), host1);
                x.Host(nameof(host2), host2);
            });

            var configId = await environment.ConfigureAsync(x =>
            {
                x.AddComponentConfigurator<IComponent>(nameof(component1), cmp => componentConfigurator1);
                x.AddComponentConfigurator<IComponent>(nameof(component2), cmp => componentConfigurator2);
            });

            // Act
            await environment.RemoveConfigurationAsync(configId);

            // Assert
            A.CallTo(() => component1.UnregisterConfigurationAsync(configId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => component2.UnregisterConfigurationAsync(configId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Given_MultipleComponentsConfigured_When_RemoveConfigurationFails_Then_ThrowExceptionAsync()
        {
            // Arrange
            var component1 = A.Fake<IComponent>();
            var componentConfigurator1 = A.Fake<IComponentConfigurator>();

            var failingComponent1Exception = new Exception("failingComponent1Exception");
            var failingComponent1 = A.Fake<IComponent>();
            // Throws immediately
            A.CallTo(() => failingComponent1.UnregisterConfigurationAsync(A<Guid>._, A<CancellationToken>._)).Throws(failingComponent1Exception);
            var failingComponentConfigurator1 = A.Fake<IComponentConfigurator>();

            var host1 = A.Fake<IHost>();
            A.CallTo(() => host1.Components)
                .Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent>
                {
                    { nameof(component1), component1 },
                    { nameof(failingComponent1), failingComponent1 }
                }));

            var component2 = A.Fake<IComponent>();
            var componentConfigurator2 = A.Fake<IComponentConfigurator>();

            var failingComponent2Exception = new Exception("failingComponent2Exception");
            var failingComponent2 = A.Fake<IComponent>();
            // Throws via task
            A.CallTo(() => failingComponent2.UnregisterConfigurationAsync(A<Guid>._, A<CancellationToken>._)).Throws(failingComponent2Exception);
            var failingComponentConfigurator2 = A.Fake<IComponentConfigurator>();

            var host2 = A.Fake<IHost>();
            A.CallTo(() => host2.Components)
                .Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent>
                {
                    { nameof(component2), component2 },
                    { nameof(failingComponent2), failingComponent2 }
                }));

            var environment = Environment.Setup(x =>
            {
                x.Host(nameof(host1), host1);
                x.Host(nameof(host2), host2);
            });

            var configId = await environment.ConfigureAsync(x =>
            {
                x.AddComponentConfigurator<IComponent>(nameof(component1), cmp => componentConfigurator1);
                x.AddComponentConfigurator<IComponent>(nameof(failingComponent1), cmp => failingComponentConfigurator1);
                x.AddComponentConfigurator<IComponent>(nameof(component2), cmp => componentConfigurator2);
                x.AddComponentConfigurator<IComponent>(nameof(failingComponent2), cmp => failingComponentConfigurator2);
            });

            // Act
            var exception = Assert.ThrowsAsync<ConfigurationRemovalException>(() => environment.RemoveConfigurationAsync(configId));

            // Assert
            Assert.IsNotNull(exception.Details);
            Assert.AreEqual(2, exception.Details.Length);

            Assert.That(exception.Details.Any(x => x.ComponentName == nameof(failingComponent1) && x.Exception == failingComponent1Exception), $"{nameof(failingComponent1)} is missing from the exception details");
            Assert.That(exception.Details.Any(x => x.ComponentName == nameof(failingComponent2) && x.Exception == failingComponent2Exception), $"{nameof(failingComponent2)} is missing from the exception details");

            // Assert
            A.CallTo(() => component1.UnregisterConfigurationAsync(configId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => failingComponent1.UnregisterConfigurationAsync(configId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => component2.UnregisterConfigurationAsync(configId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => failingComponent2.UnregisterConfigurationAsync(configId, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }
    }
}
