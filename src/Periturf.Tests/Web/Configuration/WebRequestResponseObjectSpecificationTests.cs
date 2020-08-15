using FakeItEasy;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Periturf.Web;
using Periturf.Web.Configuration;
using Periturf.Web.Configuration.Requests.Responses;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Periturf.Tests.Web.Configuration
{
    [TestFixture]
    class WebRequestResponseObjectSpecificationTests
    {
        [Test]
        public async Task Given_Spec_When_Build_Then_WriterCreatesResponse()
        {
            var obj = new object();

            var writer = A.Dummy<Func<IWebResponse, object?, Task>>();
            var writerSpec = A.Fake<IWebWriterSpecification>();
            A.CallTo(() => writerSpec.Build()).Returns(writer);

            var response = A.Dummy<IWebResponse>();

            var sut = new WebRequestResponseObjectSpecification();
            sut.Object(obj);
            sut.SetWriterSpecification(writerSpec);

            var objectWriter = sut.Build();

            Assert.That(objectWriter, Is.Not.Null);

            await objectWriter(response);

            A.CallTo(() => writer.Invoke(response, obj)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Given_InvalidWriter_When_SetWriterSpecification_Then_Exception()
        {
            var sut = new WebRequestResponseObjectSpecification();
            Assert.That(() => sut.SetWriterSpecification(null), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("writerSpecification"));
        }
    }
}
