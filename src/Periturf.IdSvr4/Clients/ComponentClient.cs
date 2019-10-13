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
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using IdentityModel.Client;

namespace Periturf.IdSvr4.Clients
{
    class ComponentClient : IIdSvr4Client
    {
        private readonly HttpClient _httpClient;
        private readonly IDiscoveryCache _discoveryCache;

        public ComponentClient(HttpClient httpClient, IDiscoveryCache discoveryCache)
        {
            _httpClient = httpClient;
            _discoveryCache = discoveryCache;
        }

        public async Task<DiscoveryResponse> GetDiscoveryDocumentAsync(DiscoveryDocumentRequest? request = null, CancellationToken cancellationToken = default)
        {
            return await _discoveryCache.GetAsync().ConfigureAwait(false);
        }

        public async Task<TokenResponse> RequestAuthorizationCodeTokenAsync(AuthorizationCodeTokenRequest request, CancellationToken cancellationToken = default)
        {
            var discovery = await _discoveryCache.GetAsync().ConfigureAwait(false);
            request.Address = discovery.AuthorizeEndpoint;
            return await _httpClient.RequestAuthorizationCodeTokenAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TokenResponse> RequestClientCredentialsTokenAsync(ClientCredentialsTokenRequest request, CancellationToken cancellationToken = default)
        {
            var discovery = await _discoveryCache.GetAsync().ConfigureAwait(false);
            request.Address = discovery.TokenEndpoint;
            return await _httpClient.RequestClientCredentialsTokenAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TokenResponse> RequestDeviceTokenAsync(DeviceTokenRequest request, CancellationToken cancellationToken = default)
        {
            var discovery = await _discoveryCache.GetAsync().ConfigureAwait(false);
            request.Address = discovery.DeviceAuthorizationEndpoint;
            return await _httpClient.RequestDeviceTokenAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TokenResponse> RequestPasswordTokenAsync(PasswordTokenRequest request, CancellationToken cancellationToken = default)
        {
            var discovery = await _discoveryCache.GetAsync().ConfigureAwait(false);
            request.Address = discovery.TokenEndpoint;
            return await _httpClient.RequestPasswordTokenAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TokenResponse> RequestRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            var discovery = await _discoveryCache.GetAsync().ConfigureAwait(false);
            request.Address = discovery.TokenEndpoint;
            return await _httpClient.RequestRefreshTokenAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<TokenResponse> RequestTokenAsync(TokenRequest request, CancellationToken cancellationToken = default)
        {
            var discovery = await _discoveryCache.GetAsync().ConfigureAwait(false);
            request.Address = discovery.TokenEndpoint;
            return await _httpClient.RequestTokenAsync(request, cancellationToken).ConfigureAwait(false);
        }

        public async Task<IntrospectionResponse> IntrospectTokenAsync(TokenIntrospectionRequest request, CancellationToken cancellationToken = default)
        {
            var discovery = await _discoveryCache.GetAsync().ConfigureAwait(false);
            request.Address = discovery.IntrospectionEndpoint;
            return await _httpClient.IntrospectTokenAsync(request, cancellationToken).ConfigureAwait(false);
        }
    }
}
