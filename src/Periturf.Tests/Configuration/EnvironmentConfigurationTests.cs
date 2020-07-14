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
using Periturf.Events;
using Periturf.Setup;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Configuration
{
    [TestFixture]
    class EnvironmentConfigurationTests
    {
        private IConfigurationHandle _configHandle1;
        private IConfigurationSpecification _configSpec1;
        private IConfigurationHandle _configHandle2;
        private IConfigurationSpecification _configSpec2;
        private IComponent _component1;
        private IComponent _component2;
        private Environment _environment;

        [SetUp]
        public void SetUp()
        {
            _configHandle1 = A.Fake<IConfigurationHandle>();
            _configSpec1 = A.Fake<IConfigurationSpecification>();
            A.CallTo(() => _configSpec1.ApplyAsync(A<CancellationToken>._)).Returns(_configHandle1);
            _component1 = A.Fake<IComponent>();
            A.CallTo(() => _component1.CreateConfigurationSpecification<IConfigurationSpecification>(A<IEventHandlerFactory>._)).Returns(_configSpec1);
            var host1 = A.Fake<IHost>();
            A.CallTo(() => host1.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(_component1), _component1 } }));
            var host1Spec = A.Fake<IHostSpecification>();
            A.CallTo(() => host1Spec.Build()).Returns(host1);

            _configHandle2 = A.Fake<IConfigurationHandle>();
            _configSpec2 = A.Fake<IConfigurationSpecification>();
            A.CallTo(() => _configSpec2.ApplyAsync(A<CancellationToken>._)).Returns(_configHandle2);
            _component2 = A.Fake<IComponent>();
            A.CallTo(() => _component2.CreateConfigurationSpecification<IConfigurationSpecification>(A<IEventHandlerFactory>._)).Returns(_configSpec2);
            var host2 = A.Fake<IHost>();
            A.CallTo(() => host2.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(_component2), _component2 } }));
            var host2Spec = A.Fake<IHostSpecification>();
            A.CallTo(() => host2Spec.Build()).Returns(host2);

            _environment = Environment.Setup(x =>
            {
                x.AddHostSpecification(host1Spec);
                x.AddHostSpecification(host2Spec);
            });
        }

        [Test]
        public async Task Given_MultipleComponents_When_Configure_Then_ComponentsAreConfigured()
        {
            var handle = await _environment.ConfigureAsync(x =>
            {
                var c1 = x.CreateComponentConfigSpecification<IConfigurationSpecification>(nameof(_component1));
                x.AddSpecification(c1);
                var c2 = x.CreateComponentConfigSpecification<IConfigurationSpecification>(nameof(_component2));
                x.AddSpecification(c2);
            });

            Assert.That(handle, Is.Not.Null);
            A.CallTo(() => _configSpec1.ApplyAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _configSpec2.ApplyAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _configHandle1.DisposeAsync()).MustNotHaveHappened();
            A.CallTo(() => _configHandle2.DisposeAsync()).MustNotHaveHappened();
        }

        [TestCase(null, Description = "Null Component Name")]
        [TestCase("", Description = "Empty Component Name")]
        [TestCase(" ", Description = "Whitespace Component Name")]
        public void Given_BadComponentName_When_Configure_Then_ThrowException(string componentName)
        {
            var exception = Assert.ThrowsAsync<ArgumentNullException>(() => _environment.ConfigureAsync(x =>
            {
                var c1 = x.CreateComponentConfigSpecification<IConfigurationSpecification>(componentName);
                x.AddSpecification(c1);
            }));

            Assert.That(exception.ParamName, Is.EqualTo("componentName"));
            A.CallTo(() => _component1.CreateConfigurationSpecification<IConfigurationSpecification>(A<IEventHandlerFactory>._)).MustNotHaveHappened();
            A.CallTo(() => _configSpec1.ApplyAsync(A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Test]
        public void Given_WrongComponentName_When_Configure_Then_ThrowException()
        {
            const string wrongComponentName = "Wrong";
            var exception = Assert.ThrowsAsync<ComponentLocationFailedException>(() => _environment.ConfigureAsync(x =>
            {
                var c1 = x.CreateComponentConfigSpecification<IConfigurationSpecification>(wrongComponentName);
                x.AddSpecification(c1);
            }));

            Assert.That(exception.ComponentName, Is.EqualTo(wrongComponentName));
            A.CallTo(() => _component1.CreateConfigurationSpecification<IConfigurationSpecification>(A<IEventHandlerFactory>._)).MustNotHaveHappened();
            A.CallTo(() => _configSpec1.ApplyAsync(A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Given_MultipleComponentsConfigured_When_RemoveConfiguration_Then_ConfigurationUnRegistered()
        {
            var handle = await _environment.ConfigureAsync(x =>
            {
                var c1 = x.CreateComponentConfigSpecification<IConfigurationSpecification>(nameof(_component1));
                x.AddSpecification(c1);
                var c2 = x.CreateComponentConfigSpecification<IConfigurationSpecification>(nameof(_component2));
                x.AddSpecification(c2);
            });

            Assume.That(handle, Is.Not.Null);

            A.CallTo(() => _configHandle1.DisposeAsync()).MustNotHaveHappened();
            A.CallTo(() => _configHandle2.DisposeAsync()).MustNotHaveHappened();

            await handle.DisposeAsync();

            A.CallTo(() => _configHandle1.DisposeAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _configHandle2.DisposeAsync()).MustHaveHappenedOnceExactly();
        }
    }
}
