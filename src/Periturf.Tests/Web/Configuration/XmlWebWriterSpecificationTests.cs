using FakeItEasy;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Periturf.Web;
using Periturf.Web.Configuration;
using Periturf.Web.Configuration.Requests.Responses;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;

namespace Periturf.Tests.Web.Configuration
{
    [TestFixture]
    class XmlWebWriterSpecificationTests
    {
        [Test]
        public async Task Given_Spec_When_Build_Then_WriterSerializesObject()
        {
            var obj = new TestClass { Test = true };
            using (var stream = new MemoryStream())
            {
                var response = A.Dummy<IWebResponse>();
                A.CallTo(() => response.BodyStream).Returns(stream);

                var sut = new XmlWebWriterSpecification();
                var xmlWriter = sut.Build();

                await xmlWriter(response, obj);
                stream.Flush();

                var resultBytes = stream.ToArray();
                var result = System.Text.Encoding.UTF8.GetString(resultBytes);

                const string expectedBody = "<?xml version=\"1.0\"?>\r\n"
+ "<TestClass xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n"
+ "  <Test>true</Test>\r\n"
+ "</TestClass>";
                Assert.That(result, Is.EqualTo(expectedBody));
                A.CallToSet(() => response.ContentType).To("application/xml").MustHaveHappened();
            }
        }
    }

    public class TestClass
    {
        public bool Test { get; set; }
    }
}
