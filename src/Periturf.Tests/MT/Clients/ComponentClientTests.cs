using FakeItEasy;
using MassTransit;
using NUnit.Framework;
using Periturf.MT.Clients;
using Periturf.MT.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.MT.Clients
{
    [TestFixture]
    class ComponentClientTests
    {
        [Test]
        public void Given_Bus_When_CtorBus_Then_Mapped()
        {
            var bus = A.Dummy<IBus>();

            var client = new ComponentClient(bus);

            Assert.That(client, Is.Not.Null);
        }

        [Test]
        public async Task Given_Client_When_Publish_Then_MessagePublished()
        {
            var bus = A.Fake<IBus>();
            var message = new object();
            await new ComponentClient(bus).Publish(message);

            A.CallTo(() => bus.Publish(message, A<CancellationToken>._)).MustHaveHappened();
        }
    }
}
