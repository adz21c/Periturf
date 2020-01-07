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
using IdentityModel.Client;
using static IdentityModel.OidcConstants;
using Microsoft.AspNetCore.Hosting;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;

namespace Periturf.Tests.Integration.IdSvr4
{
    [TestFixture]
    class ComponentStartStopTests
    {
        private const string WebHostUrl = "http://localhost:3501";

        [Test]
        public async Task Given_IdSvr4_When_StartAndStop_Then_RespondAndDontRespond()
        {
            // Arrange
            var env = Environment.Setup(e =>
            {
                e.WebHost(h =>
                {
                    h.UseUrls(WebHostUrl);
                    h.IdSvr4();
                });
            });

            // Assu,e
            var client = new HttpClient();
            var beforeStartResponse = await client.RequestTokenAsync(new IdentityModel.Client.TokenRequest
            {
                Address = WebHostUrl + "/IdSvr4/connect/token",
                GrantType = GrantTypes.ClientCredentials,
                ClientId = "Client1",
                ClientSecret = "Secret"
            });
            Assume.That(beforeStartResponse.IsError);
            Assume.That(beforeStartResponse.Exception != null);

            // Act
            await env.StartAsync();

            // Assert
            var afterStartResponse = await client.RequestTokenAsync(new IdentityModel.Client.TokenRequest
            {
                Address = WebHostUrl + "/IdSvr4/connect/token",
                GrantType = GrantTypes.ClientCredentials,
                ClientId = "Client1",
                ClientSecret = "Secret"
            });
            Assert.That(afterStartResponse.IsError, Is.True);
            Assert.That(afterStartResponse.HttpStatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(afterStartResponse.Error, Is.EqualTo("invalid_client"));

            // Act
            await env.StopAsync();

            // Assert
            var afterStopResponse = await client.RequestTokenAsync(new IdentityModel.Client.TokenRequest
            {
                Address = WebHostUrl + "/IdSvr4/connect/token",
                GrantType = GrantTypes.ClientCredentials,
                ClientId = "Client1",
                ClientSecret = "Secret"
            });
            Assert.That(afterStopResponse.IsError, Is.True);
            Assert.That(afterStopResponse.Exception, Is.Not.Null);
        }
    }
}
