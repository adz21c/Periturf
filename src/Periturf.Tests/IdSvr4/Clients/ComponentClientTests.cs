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
using Newtonsoft.Json;
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
    class ComponentClientTests
    {
        private HttpMessageHandler _messageHandler;
        private ComponentClient _componentClient;

        [SetUp]
        public void Setup()
        {
            _messageHandler = A.Fake<HttpMessageHandler>();

            var cache = A.Fake<IDiscoveryCache>();
            A.CallTo(() => cache.GetAsync()).Returns(
                new DiscoveryResponse(
                    @"{""issuer"":""https://www.mywebsite.co.uk"",""jwks_uri"":""https://www.mywebsite.co.uk/.well-known/jwks"",""authorization_endpoint"":""https://www.mywebsite.co.uk/connect/authorize"",""token_endpoint"":""https://www.mywebsite.co.uk/connect/token"",""userinfo_endpoint"":""https://www.mywebsite.co.uk/connect/userinfo"",""end_session_endpoint"":""https://www.mywebsite.co.uk/connect/endsession"",""check_session_iframe"":""https://www.mywebsite.co.uk/connect/checksession"",""revocation_endpoint"":""https://www.mywebsite.co.uk/connect/revocation"",""introspection_endpoint"":""https://www.mywebsite.co.uk/connect/introspect"",""frontchannel_logout_supported"":true,""frontchannel_logout_session_supported"":true,""scopes_supported"":[""openid"",""profile"",""roles"",""offline_access""],""claims_supported"":[""sub"",""name"",""family_name"",""given_name"",""middle_name"",""nickname"",""preferred_username"",""profile"",""picture"",""website"",""email"",""email_verified"",""gender"",""birthdate"",""zoneinfo"",""locale"",""phone_number"",""phone_number_verified"",""address"",""updated_at"",""role""],""response_types_supported"":[""code"",""token"",""id_token"",""id_token token"",""code id_token"",""code token"",""code id_token token""],""response_modes_supported"":[""form_post"",""query"",""fragment""],""grant_types_supported"":[""authorization_code"",""client_credentials"",""password"",""refresh_token"",""implicit""],""subject_types_supported"":[""public""],""id_token_signing_alg_values_supported"":[""RS256""],""code_challenge_methods_supported"":[""plain"",""S256""],""token_endpoint_auth_methods_supported"":[""client_secret_post"",""client_secret_basic""]}",
                    new DiscoveryPolicy
                    {
                        Authority = "https://www.mywebsite.co.uk"
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
        public async Task SomethingAsync()
        {
            var x = await _componentClient.RequestPasswordTokenAsync(null);
        }
    }
}
