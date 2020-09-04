using FakeItEasy;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Periturf.Web;
using Periturf.Web.Configuration;
using Periturf.Web.Configuration.Requests.Responses;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Periturf.Tests.Web.Configuration
{
    [TestFixture]
    class JsonWebWriterSpecificationTests
    {
        [Test]
        public async Task Given_Spec_When_Build_Then_WriterSerializesObject()
        {
            var obj = new { Test = true };
            var response = A.Dummy<IWebResponse>();

            var sut = new JsonWebWriterSpecification();
            var jsonWriter = sut.Build();

            await jsonWriter(response, obj);

            const string expectedBody = "{\"Test\":true}";
            A.CallTo(() => response.WriteBodyAsync(A<string>.That.IsEqualTo(expectedBody))).MustHaveHappenedOnceExactly();
            A.CallToSet(() => response.ContentType).To("application/json").MustHaveHappened();
        }

        [Test]
        public async Task Given_SpecWithOverrides_When_Build_Then_WriterSerializesObjectWithOptions()
        {
            var obj = new { Test = true };
            var response = A.Dummy<IWebResponse>();

            var sut = new JsonWebWriterSpecification();
            sut.Options(new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            var jsonWriter = sut.Build();

            await jsonWriter(response, obj);

            const string expectedBody = "{\"test\":true}";
            A.CallTo(() => response.WriteBodyAsync(A<string>.That.IsEqualTo(expectedBody))).MustHaveHappenedOnceExactly();
            A.CallToSet(() => response.ContentType).To("application/json").MustHaveHappened();
        }
    }
}
