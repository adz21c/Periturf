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
using MassTransit;
using NUnit.Framework;
using Periturf.Events;
using Periturf.MT;
using Periturf.MT.Clients;
using Periturf.MT.Configuration;
using Periturf.MT.Verify;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.MT
{
    [TestFixture]
    class MtHostComponentTests
    {
        private const string _componentName = "Component";
        private IBusManager _busManager;
        private IEventResponseContextFactory _factory;
        private IBusControl _busControl;
        private MtHostComponent _host;

        [SetUp]
        public void SetUp()
        {
            _factory = A.Fake<IEventResponseContextFactory>();

            _busControl = A.Fake<IBusControl>();
            _busManager = A.Fake<IBusManager>();
            A.CallTo(() => _busManager.BusControl).Returns(_busControl);
            _host = new MtHostComponent(_busManager, _componentName);
        }

        [Test]
        public void Given_HostComponent_When_CheckProperties_Then_ExistsAndIsHost()
        {
            Assert.That(_host.BusManager, Is.SameAs(_busManager));
            Assert.That(_host.Components, Does.ContainKey(_componentName));
            Assert.That(_host.Components, Does.ContainValue(_host));
            Assert.That(_host.Components[_componentName], Is.SameAs(_host));
        }

        [Test]
        public async Task Given_HostComponent_When_Start_Then_StartBus()
        {
            await _host.StartAsync();
            A.CallTo(() => _busControl.StartAsync(A<CancellationToken>._)).MustHaveHappened();
        }

        [Test]
        public async Task Given_HostComponent_When_Stop_Then_StopBus()
        {
            await _host.StopAsync();
            A.CallTo(() => _busControl.StopAsync(A<CancellationToken>._)).MustHaveHappened();
        }

        [Test]
        public void Given_HostComponent_When_CreateConfigurationSpecification_Then_Created()
        {
            var spec = _host.CreateConfigurationSpecification<MtConfigurationSpecification>(_factory);
            Assert.That(spec, Is.Not.Null);
        }

        [Test]
        public void Given_HostComponent_When_CreateConditionBuilder_Then_Created()
        {
            var builder = _host.CreateConditionBuilder<MtConditionBuilder>();
            Assert.That(builder, Is.Not.Null);
        }

        [Test]
        public void Given_HostComponent_When_CreateClient_Then_Created()
        {
            var client = _host.CreateClient();
            Assert.That(client, Is.Not.Null);
            Assert.That(client, Is.AssignableTo<IMTClient>());
        }
    }
}
