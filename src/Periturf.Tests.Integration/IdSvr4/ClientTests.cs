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
using IdentityServer4.Events;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Hosting;
using NUnit.Framework;
using Periturf.Verify.Criterias;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;

namespace Periturf.Tests.Integration.IdSvr4
{
    [TestFixture]
    class ClientTests
    {

        [Test]
        public async Task GiveMeAName()
        {
            // Arrange
            const string WebHostUrl = "http://localhost:3510";

            const string ClientId = "ClientId";
            const string ClientSecret = "secret";
            const string Scope = "Resource";


            var env = Environment.Setup(e =>
            {
                e.WebHost(h =>
                {
                    h.UseUrls(WebHostUrl);
                    h.IdSvr4();
                });
            });
            await env.StartAsync();

            var configId = await env.ConfigureAsync(c =>
            {
                c.IdSvr4(i =>
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
            });

            var verifier = await env.VerifyAsync(c => 
                c.Expect(
                    c.IdSvr4().EventOccurred<ClientAuthenticationSuccessEvent>(e => e.ClientId == ClientId),
                    e => e.MustOccurWithin(TimeSpan.FromMilliseconds(500))));

            var idSvr4Client = env.IdSvr4Client();

            await idSvr4Client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                ClientId = ClientId,
                ClientSecret = ClientSecret,
                Scope = Scope
            });

            var verificationResult = await verifier.VerifyAsync();

            await env.RemoveConfigurationAsync(configId);
            await env.StopAsync();

            Assert.IsTrue(verificationResult.ExpectationsMet);
        }
    }
}
