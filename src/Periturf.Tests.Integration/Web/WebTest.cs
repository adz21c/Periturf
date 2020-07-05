using Microsoft.AspNetCore.Hosting;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.Tests.Integration.Web
{
    [TestFixture]
    class WebTests
    {
        [Test]
        public async Task Given_IdSvr_When_ConfigureAndRemoveConfiguration_Then_ClientAuthsAndFailsAuth()
        {
            // Arrange
            const string WebHostUrl = "http://localhost:3510";

            var env = Environment.Setup(e =>
            {
                e.GenericHost(h =>
                {
                    h.Web(w =>
                    {
                        w.ConfigureBuilder(wb => wb.UseUrls(WebHostUrl));
                    });
                });
            });

            var client = new HttpClient();
            client.BaseAddress = new Uri(WebHostUrl);

            await env.StartAsync();
            try
            {
                // Act
                await using (var configHandle = await env.ConfigureAsync(c =>
                {
                    c.Web(w =>
                    {
                        w.OnRequest(r =>
                        {
                            r.Response(rs =>
                            {
                                rs.StatusCode = HttpStatusCode.OK;
                                rs.SetBody(new { Test = "Value" });
                            });
                        });
                    });
                }))
                {
                    var testResponse = await client.GetAsync(WebHostUrl);
                    var testText = await testResponse.Content.ReadAsStringAsync();
                    Assert.That(testResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                }
            }
            finally
            {
                await env.StopAsync();
            }
        }
    }
}