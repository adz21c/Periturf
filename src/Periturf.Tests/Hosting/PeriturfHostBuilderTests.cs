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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Periturf.Components;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Hosting
{
    [TestFixture]
    class PeriturfHostBuilderTests
    {
        [Test]
        public void Given_MultipleComponents_When_Host_Then_NewHostRegisteredWithComponents()
        {
            // Arrange
            var configurator = A.Dummy<ISetupConfigurator>();
            var component1 = A.Dummy<IComponent>();
            var component2 = A.Dummy<IComponent>();
            const string hostName = "HostName";

            // Act
            configurator.GenericHost(hostName, c =>
            {
                c.ConfigureServices(s => s.AddHostedService<StartupDummy>());
                c.AddComponent(nameof(component1), component1);
                c.AddComponent(nameof(component2), component2);
            });

            // Assert
            A.CallTo(() => configurator.Host(
                hostName,
                A<Components.IHost>.That.NullCheckedMatches(
                    x => x.Components.Count == 2 &&
                    x.Components.GetValueOrDefault(nameof(component1)) == component1 &&
                    x.Components.GetValueOrDefault(nameof(component2)) == component2,
                    x => x.Write("ComponentDictionary")))).MustHaveHappened();
        }

        [Test]
        public void Given_Host_When_StartAndStop_Then_TheHostStartsAndStops()
        {
            // Arrange
            var component1 = A.Dummy<IComponent>();
            var component2 = A.Dummy<IComponent>();
            const string hostName = "HostName";

            var env = Environment.Setup(s =>
            {
                s.GenericHost(hostName, c =>
                {
                    c.ConfigureServices(s => s.AddHostedService<StartupDummy>());
                    c.AddComponent(nameof(component1), component1);
                    c.AddComponent(nameof(component2), component2);
                });
            });

            // Act
            Assert.DoesNotThrowAsync(() => env.StartAsync());
            Assert.DoesNotThrowAsync(() => env.StopAsync());
        }

        [Test]
        public void Given_MultipleHosts_When_StartAndStop_Then_TheHostsStartsAndStops()
        {
            // Arrange
            var component1 = A.Dummy<IComponent>();
            var component2 = A.Dummy<IComponent>();
            const string hostName1 = "HostName1";
            const string hostName2 = "HostName2";

            var env = Environment.Setup(s =>
            {
                s.GenericHost(hostName1, c =>
                {
                    c.ConfigureServices(s => s.AddHostedService<StartupDummy>());
                    c.AddComponent(nameof(component1), component1);
                });

                s.GenericHost(hostName2, c =>
                {
                    c.ConfigureServices(s => s.AddHostedService<StartupDummy>());
                    c.AddComponent(nameof(component2), component2);
                });
            });

            // Act
            Assert.DoesNotThrowAsync(() => env.StartAsync());
            Assert.DoesNotThrowAsync(() => env.StopAsync());
        }

        [TestCase(null, Description = "Null Component Name")]
        [TestCase("", Description = "Empty Component Name")]
        [TestCase(" ", Description = "Whitespace Component Name")]
        public void Given_BadComponentName_When_AddComponent_Then_ThrowException(string componentName)
        {
            // Arrange
            var configurator = A.Dummy<ISetupConfigurator>();
            var component1 = A.Dummy<IComponent>();
            const string hostName1 = "HostName1";

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => configurator.GenericHost(hostName1, c =>
            {
                c.ConfigureServices(s => s.AddHostedService<StartupDummy>());
                c.AddComponent(componentName, component1);
            }));

            // Assert
            Assert.AreEqual("componentName", exception.ParamName);
        }

        [Test]
        public void Given_NullComponent_When_AddComponent_Then_ThrowException()
        {
            // Arrange
            var configurator = A.Dummy<ISetupConfigurator>();
            const string hostName1 = "HostName1";

            // Act
            var exception = Assert.Throws<ArgumentNullException>(() => configurator.GenericHost(hostName1, c =>
            {
                c.ConfigureServices(s => s.AddHostedService<StartupDummy>());
                c.AddComponent("ComponentName", null);
            }));

            // Assert
            Assert.AreEqual("component", exception.ParamName);
        }

        private class StartupDummy : IHostedService
        {
            public Task StartAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }

            public Task StopAsync(CancellationToken cancellationToken)
            {
                return Task.CompletedTask;
            }
        }
    }
}
