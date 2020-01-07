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
using NUnit.Framework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Periturf.Tests.Integration.AspNetCore
{
    [TestFixture]
    class WebHostTests
    {
        private const string ResponseString = "My Test";
        private const string WebHostUrl = "http://localhost:3500";

        [Test]
        public async Task Given_WebHost_When_StartAndStop_Then_ReceiveResponseAndDontReceiveResponse()
        {
            var env = Environment.Setup(e =>
            {
                e.WebHost(h =>
                {
                    h.UseStartup<StartUpDummy>();
                    h.UseUrls(WebHostUrl);
                });
            });

            var client = new HttpClient();

            Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync(WebHostUrl));

            await env.StartAsync();

            var afterStartResponse = await client.GetAsync(WebHostUrl);
            Assert.That(afterStartResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(await afterStartResponse.Content.ReadAsStringAsync(), Is.EqualTo(ResponseString));

            await env.StopAsync();

            Assert.ThrowsAsync<HttpRequestException>(() => client.GetAsync(WebHostUrl));
        }

        private class StartUpDummy
        {
            public void Configure(IApplicationBuilder app)
            {
                app.UseMiddleware<FakeMiddleware>();
            }
        }

        private class FakeMiddleware
        {
            private readonly RequestDelegate _next;

            public FakeMiddleware(RequestDelegate next)
            {
                _next = next;
            }

            public async Task Invoke(HttpContext context)
            {
                // always respond OK
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                await context.Response.WriteAsync(ResponseString);
            }
        }
    }
}
