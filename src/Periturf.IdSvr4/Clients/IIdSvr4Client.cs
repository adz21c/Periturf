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
using System.Threading;
using System.Threading.Tasks;

namespace Periturf
{
    /// <summary>
    /// Client for IdentityServer4 components
    /// </summary>
    /// <seealso cref="Periturf.IComponentClient" />
    public interface IIdSvr4Client : IComponentClient
    {
        Task<DiscoveryResponse> GetDiscoveryDocumentAsync(DiscoveryDocumentRequest request = null, CancellationToken cancellationToken = default);

        Task<TokenResponse> RequestAuthorizationCodeTokenAsync(AuthorizationCodeTokenRequest request, CancellationToken cancellationToken = default);
        
        Task<TokenResponse> RequestClientCredentialsTokenAsync(ClientCredentialsTokenRequest request, CancellationToken cancellationToken = default);
        
        Task<TokenResponse> RequestDeviceTokenAsync(DeviceTokenRequest request, CancellationToken cancellationToken = default);
        
        Task<TokenResponse> RequestPasswordTokenAsync(PasswordTokenRequest request, CancellationToken cancellationToken = default);
        
        Task<TokenResponse> RequestRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
        
        Task<TokenResponse> RequestTokenAsync(TokenRequest request, CancellationToken cancellationToken = default);
    }
}
