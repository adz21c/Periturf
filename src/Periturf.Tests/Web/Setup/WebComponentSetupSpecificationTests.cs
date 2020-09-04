using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Periturf.Web;
using Periturf.Web.Setup;
using System;
using System.Collections.Generic;
using System.Text;

namespace Periturf.Tests.Web.Setup
{
    [TestFixture]
    class WebComponentSetupSpecificationTests
    {
        [Test]
        public void Given_BuilderAction_When_Apply_Then_IsExecuted()
        {
            var builder = A.Fake<IHostBuilder>();
            var builderAction = A.Dummy<Action<IWebHostBuilder>>();

            var sut = new WebComponentSetupSpecification("Name");
            sut.ConfigureBuilder(builderAction);

            var component = sut.Apply(builder);

            Assert.That(component, Is.Not.Null);
            Assert.That(component, Is.TypeOf<WebComponent>());
            A.CallTo(() => builderAction.Invoke(A<IWebHostBuilder>._)).MustHaveHappened();
        }
    }
}
