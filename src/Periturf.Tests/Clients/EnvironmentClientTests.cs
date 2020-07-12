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
using Periturf.Clients;
using Periturf.Components;
using Periturf.Setup;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Periturf.Tests.Clients
{
    [TestFixture]
    class EnvironmentClientTests
    {
        private IComponentClient _componentClient1;
        private IComponent _component1;


        private IComponentClient _componentClient2;
        private IComponent _component2;

        private Environment _env;

        [SetUp]
        public async Task SetupAsync()
        {
            _componentClient1 = A.Dummy<IComponentClient>();
            _component1 = A.Fake<IComponent>();
            A.CallTo(() => _component1.CreateClient()).Returns(_componentClient1);

            _componentClient2 = A.Dummy<IComponentClient>();
            _component2 = A.Fake<IComponent>();
            A.CallTo(() => _component2.CreateClient()).Returns(_componentClient2);

            var host = A.Fake<IHost>();
            A.CallTo(() => host.Components).Returns(new Dictionary<string, IComponent>
            {
                { nameof(_component1), _component1 },
                { nameof(_component2), _component2 }
            });

            var hostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => hostSpec.Build()).Returns(host);

            _env = Environment.Setup(s =>
            {
                s.AddHostSpecification(hostSpec);
            });

            await _env.StartAsync();
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            await _env.StopAsync();
        }

        [Test]
        public void Given_MultipleComponents_When_CreateClient_Then_ReturnsCorrectComponentClient()
        {
            var client = _env.CreateComponentClient(nameof(_component2));

            Assert.That(client, Is.Not.Null);
            Assert.That(client, Is.SameAs(_componentClient2));
            A.CallTo(() => _component2.CreateClient()).MustHaveHappened();
            A.CallTo(() => _component1.CreateClient()).MustNotHaveHappened();
        }

        [Test]
        public void Given_MultipleComponents_When_CreateClient_Then_ReturnsCorrectComponentClientForEach()
        {
            var client2 = _env.CreateComponentClient(nameof(_component2));
            var client1 = _env.CreateComponentClient(nameof(_component1));

            Assert.That(client1, Is.Not.Null);
            Assert.That(client1, Is.SameAs(_componentClient1));

            Assert.That(client2, Is.Not.Null);
            Assert.That(client2, Is.SameAs(_componentClient2));

            A.CallTo(() => _component2.CreateClient()).MustHaveHappened().Then(
                A.CallTo(() => _component1.CreateClient()).MustHaveHappened());
        }

        [TestCase(null, Description = "Null")]
        [TestCase("", Description = "Empty")]
        [TestCase(" ", Description = "Whitespace")]
        public void Given_InvalidComponentName_When_CreateClient_Then_Throws(string componentName)
        {
            var exception = Assert.Throws<ArgumentNullException>(() => _env.CreateComponentClient(componentName));

            Assert.That(exception.ParamName, Is.EqualTo("componentName"));
        }

        [Test]
        public void Given_ComponentDoesNotExist_When_CreateClient_Then_Throws()
        {
            const string componentName = "NotAComponent";
            var exception = Assert.Throws<ComponentLocationFailedException>(() => _env.CreateComponentClient(componentName));

            Assert.That(exception.ComponentName, Is.EqualTo(componentName));
        }
    }
}
