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
using IdentityModel.Client;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;

namespace Periturf.Tests.Integration.IdSvr4
{
    [TestFixture]
    class ClientConfigurationTests
    {

        [Test]
        public async Task Given_IdSvr_When_ConfigureAndRemoveConfiguration_Then_ClientAuthsAndFailsAuth()
        {
            // Arrange
            const string WebHostUrl = "http://localhost:3502";
            const string TokenEndpointUrl = WebHostUrl + "/IdSvr4/connect/token";

            const string ClientId = "ClientId";
            const string ClientSecret = "secret";
            const string Scope = "Resource";

            var tokenRequest = new ClientCredentialsTokenRequest
            {
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                Scope = Scope
            };

            var env = Environment.Setup(e =>
            {
                e.WebHost(h =>
                {
                    h.UseUrls(WebHostUrl);
                    h.IdSvr4();
                });
            });

            var client = new HttpClient();
            client.BaseAddress = new Uri(TokenEndpointUrl);
            
            await env.StartAsync();
            try
            {
                // Assume the client does not already exist
                var assumptionResponse = await client.RequestClientCredentialsTokenAsync(tokenRequest);
                Assume.That(assumptionResponse.IsError);
                Assume.That(assumptionResponse.HttpStatusCode == HttpStatusCode.BadRequest);
                Assume.That(assumptionResponse.Error.ToLower() == "invalid_client");

                // Act
                await using (var configHandle = await env.ConfigureAsync(c =>
                {
                    c.IdSvr4("IdSvr4", i =>
                    {
                        i.Client(cl =>
                        {
                            cl.ClientId = ClientId;
                            cl.AllowedGrantTypes = IdentityServer4.Models.GrantTypes.ClientCredentials;
                            cl.Secret(ClientSecret.Sha256());
                            cl.AccessTokenType = AccessTokenType.Jwt;
                            cl.Scope(Scope);
                        });
                        i.ApiResource(new ApiResource(Scope, Scope));
                    });
                }))
                {
                    // Assert
                    var configuredResponse = await client.RequestClientCredentialsTokenAsync(tokenRequest);
                    Assert.That(configuredResponse.IsError, Is.False);
                    Assert.That(configuredResponse.HttpStatusCode, Is.EqualTo(HttpStatusCode.OK));
                    Assert.That(!string.IsNullOrWhiteSpace(configuredResponse.AccessToken));
                }


                // Assert
                var unconfiguredResponse = await client.RequestClientCredentialsTokenAsync(tokenRequest);
                Assert.That(unconfiguredResponse.IsError, Is.True);
                Assert.That(unconfiguredResponse.HttpStatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
                Assert.That(unconfiguredResponse.Error.ToLower(), Is.EqualTo("invalid_client"));
            }
            finally
            {
                await env.StopAsync();
            }
        }
    }
}
