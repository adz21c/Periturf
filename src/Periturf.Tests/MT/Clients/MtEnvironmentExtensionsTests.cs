using FakeItEasy;
using MassTransit;
using NUnit.Framework;
using Periturf.MT;

namespace Periturf.Tests.MT.Clients
{
    [TestFixture]
    class MtEnvironmentExtensionsTests
    {
        [Test]
        public void Given_Env_When_MTClient_Then_Client()
        {
            const string componentName = "Component1";

            var bus = A.Dummy<IBusControl>();
            var busManager = A.Fake<IBusManager>();
            A.CallTo(() => busManager.BusControl).Returns(bus);

            var env = Environment.Setup(s =>
            {
                s.MTBus(componentName, b =>
                {
                    b.SetBusManager(busManager);
                });
            });

            var client = env.MTClient(componentName);

            Assert.That(client, Is.Not.Null);
        }
    }
}
