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
using Periturf.IdSvr4;
using Periturf.IdSvr4.Clients;
using Periturf.IdSvr4.Configuration;
using Periturf.IdSvr4.Verify;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Periturf.Tests.IdSvr4.Clients
{
    [TestFixture]
    class EnvironmentExtensionsTests
    {
        private IIdSvr4Client _componentClient;
        private IdSvr4Component _component;

        private Environment _env;

        [SetUp]
        public async Task SetupAsync()
        {
            _componentClient = A.Dummy<IIdSvr4Client>();
            _component = new IdSvr4Component(
                A.Dummy<IStore>(),
                A.Dummy<IEventMonitorSink>(),
                _componentClient);

            var host = A.Fake<IHost>();
            A.CallTo(() => host.Components).Returns(new Dictionary<string, IComponent>
            {
                { nameof(_component), _component }
            });

            _env = Environment.Setup(s =>
            {
                s.Host(nameof(host), host);
            });

            await _env.StartAsync();
        }

        [TearDown]
        public async Task TearDownAsync()
        {
            await _env.StopAsync();
        }

        [Test]
        public void Given_Environment_When_IdSvr4Client_Then_ReturnsCorrectComponentClient()
        {
            var client = _env.IdSvr4Client(nameof(_component));

            Assert.That(client, Is.Not.Null);
            Assert.That(client, Is.SameAs(_componentClient));
        }
    }
}
