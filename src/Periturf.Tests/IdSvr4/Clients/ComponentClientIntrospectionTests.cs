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
using FakeItEasy;
using IdentityModel.Client;
using NUnit.Framework;
using Periturf.IdSvr4.Clients;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.IdSvr4.Clients
{
    [TestFixture]
    class ComponentClientIntrospectionTests
    {
        private const string BasePath = "https://www.mywebsite.co.uk/";
        private const string AuthorizeEndpoint = BasePath + "connect/authorize";
        private const string TokenEndpoint = BasePath + "connect/token";
        private const string UserInfoEndpoint = BasePath + "connect/userinfo";
        private const string EndSessionEndpoint = BasePath + "connect/endsession";
        private const string CheckSessionEndpoint = BasePath + "connect/checksession";
        private const string RevocationEndpoint = BasePath + "connect/revocation";
        private const string DeviceAuthorizationEndpoint = BasePath + "connect/device";
        private const string IntrospectionEndpoint = BasePath + "connect/introspect";

        private const string AccessToken = "123456789";
        private const int ExpiresIn = 1000;
        private const string TokenType = "bearer";
        private readonly string TokenEndpointResponse = $"{{access_token: '{AccessToken}', token_type: '{TokenType}', expires_in: {ExpiresIn}}}";
            
        private HttpMessageHandler _messageHandler;
        private ComponentClient _componentClient;

        [SetUp]
        public void Setup()
        {
            _messageHandler = A.Fake<HttpMessageHandler>();

            A.CallTo(_messageHandler)
                .Where(x => x.Method.Name == "SendAsync")
                .WithReturnType<Task<HttpResponseMessage>>()
                .Returns(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(
                        TokenEndpointResponse,
                        Encoding.Default,
                        "application/json")
                });

            var cache = A.Fake<IDiscoveryCache>();

            A.CallTo(() => cache.GetAsync()).Returns(
                new DiscoveryResponse(
                    $"{{\"issuer\":\"{BasePath}\",\"jwks_uri\":\"{BasePath}/.well-known/jwks\",\"authorization_endpoint\":\"{AuthorizeEndpoint}\",\"token_endpoint\":\"{TokenEndpoint}\",\"userinfo_endpoint\":\"{UserInfoEndpoint}\",\"end_session_endpoint\":\"{EndSessionEndpoint}\",\"check_session_iframe\":\"{CheckSessionEndpoint}\",\"revocation_endpoint\":\"{RevocationEndpoint}\", \"device_authorization_endpoint\":\"{DeviceAuthorizationEndpoint}\",\"introspection_endpoint\":\"{IntrospectionEndpoint}\",\"frontchannel_logout_supported\":true,\"frontchannel_logout_session_supported\":true,\"scopes_supported\":[\"openid\",\"profile\",\"roles\",\"offline_access\"],\"claims_supported\":[\"sub\",\"name\",\"family_name\",\"given_name\",\"middle_name\",\"nickname\",\"preferred_username\",\"profile\",\"picture\",\"website\",\"email\",\"email_verified\",\"gender\",\"birthdate\",\"zoneinfo\",\"locale\",\"phone_number\",\"phone_number_verified\",\"address\",\"updated_at\",\"role\"],\"response_types_supported\":[\"code\",\"token\",\"id_token\",\"id_token token\",\"code id_token\",\"code token\",\"code id_token token\"],\"response_modes_supported\":[\"form_post\",\"query\",\"fragment\"],\"grant_types_supported\":[\"authorization_code\",\"client_credentials\",\"password\",\"refresh_token\",\"implicit\"],\"subject_types_supported\":[\"public\"],\"id_token_signing_alg_values_supported\":[\"RS256\"],\"code_challenge_methods_supported\":[\"plain\",\"S256\"],\"token_endpoint_auth_methods_supported\":[\"client_secret_post\",\"client_secret_basic\"]}}",
                    new DiscoveryPolicy
                    {
                        Authority = BasePath
                    }));

            _componentClient = new ComponentClient(
                new HttpClient(_messageHandler),
                cache);
        }

        [TearDown]
        public void TearDown()
        {
            _componentClient = null;
            _messageHandler = null;
        }

        [Test]
        public async Task Given_ComponentClient_When_IntrospectToken_Then_RequestResult()
        {
            var response = await _componentClient.IntrospectTokenAsync(new TokenIntrospectionRequest
            {
                ClientId = "ClientId",
                ClientSecret = "ClientSecret",
                Token = "Token"
            });

            Assert.That(response, Is.Not.Null);
            Assert.That(response.IsError, Is.False);


            A.CallTo(_messageHandler)
                .Where(x => x.Method.Name == "SendAsync")
                .WhenArgumentsMatch((HttpRequestMessage r, CancellationToken ct) =>
                    r.RequestUri.AbsoluteUri == IntrospectionEndpoint &&
                    r.Method.Method == "POST")
                .MustHaveHappened();
        }
    }
}
