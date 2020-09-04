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
    class WebRequestResponseSpecificationTests
    {
        [TestCase(null, Description = "NULL")]
        [TestCase("", Description = "Empty")]
        [TestCase(" ", Description = "Whitespace")]
        public void Given_InvalidKey_When_AddCookie_Then_Exception(string key)
        {
            var sut = new WebRequestResponseSpecification();
            Assert.That(() => sut.AddCookie(key, null), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("key"));
        }

        [Test]
        public void Given_Null_When_SetBodySpec_Then_Exception()
        {
            var sut = new WebRequestResponseSpecification();
            Assert.That(() => sut.SetBodySpecification(null), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("spec"));
        }

        [Test]
        public async Task Given_Spec_When_Build_Then_WriterCreatesResponse()
        {
            var statusCode = HttpStatusCode.Created;
            var contentType = "application/json";
            
            var header1Name = "Header1";
            var header1Value = "Value1";
            var header2Name = "Header2";
            var header2Value = "Value2";

            var cookie1Name = "Cookie1";
            var cookie1Value = "Value1";
            var cookie2Name = "Cookie2";
            var cookie2Value = "Value2";
            var cookie2Options = new CookieOptions();

            var bodyWriter = A.Dummy<Func<IWebResponse, Task>>();
            var bodySpec = A.Fake<IWebRequestResponseBodySpecification>();
            A.CallTo(() => bodySpec.Build()).Returns(bodyWriter);

            var response = A.Dummy<IWebResponse>();

            var sut = new WebRequestResponseSpecification();
            sut.StatusCode = statusCode;
            sut.ContentType = contentType;
            sut.AddHeader(header1Name, header1Value);
            sut.AddHeader(header2Name, header2Value);
            sut.AddCookie(cookie1Name, cookie1Value);
            sut.AddCookie(cookie2Name, cookie2Value, cookie2Options);
            sut.SetBodySpecification(bodySpec);

            var responseFactory = sut.BuildFactory();

            Assert.That(responseFactory, Is.Not.Null);

            await responseFactory(response);

            A.CallToSet(() => response.StatusCode).To(statusCode).MustHaveHappened();
            A.CallToSet(() => response.ContentType).To(contentType).MustHaveHappened();
            A.CallTo(() =>
                response.AddHeader(
                    A<string>.That.IsEqualTo(header1Name),
                    A<IEnumerable<object>>.That.Contains(header1Value)))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() =>
                response.AddHeader(
                    A<string>.That.IsEqualTo(header2Name),
                    A<IEnumerable<object>>.That.Contains(header2Value)))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() =>
                response.AddCookie(
                    A<string>.That.IsEqualTo(cookie1Name),
                    A<string>.That.IsEqualTo(cookie1Value),
                    null))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() =>
                response.AddCookie(
                    A<string>.That.IsEqualTo(cookie2Name),
                    A<string>.That.IsEqualTo(cookie2Value),
                    cookie2Options))
                .MustHaveHappenedOnceExactly();
            A.CallTo(() => bodyWriter.Invoke(response)).MustHaveHappenedOnceExactly();
        }
    }
}
