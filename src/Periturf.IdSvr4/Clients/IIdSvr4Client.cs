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
        /// <summary>
        /// Gets the discovery document asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<DiscoveryResponse> GetDiscoveryDocumentAsync(DiscoveryDocumentRequest? request = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Requests the authorization code token asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<TokenResponse> RequestAuthorizationCodeTokenAsync(AuthorizationCodeTokenRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Requests the client credentials token asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<TokenResponse> RequestClientCredentialsTokenAsync(ClientCredentialsTokenRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Requests the device token asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<TokenResponse> RequestDeviceTokenAsync(DeviceTokenRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Requests the password token asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<TokenResponse> RequestPasswordTokenAsync(PasswordTokenRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Requests the refresh token asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<TokenResponse> RequestRefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Requests the token asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<TokenResponse> RequestTokenAsync(TokenRequest request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Introspects the token asynchronous.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<IntrospectionResponse> IntrospectTokenAsync(TokenIntrospectionRequest request, CancellationToken cancellationToken = default);
    }
}
