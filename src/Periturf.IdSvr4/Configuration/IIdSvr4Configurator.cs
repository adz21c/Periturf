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
using System;
using IdentityServer4.Models;

namespace Periturf.IdSvr4.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public interface IIdSvr4Configurator
    {
        /// <summary>
        /// Configures a client.
        /// </summary>
        /// <param name="config">The configuration.</param>
        void Client(Action<Client> config)
        {
            var client = new Client();
            config(client);
            Client(client);
        }

        /// <summary>
        /// Configures a client.
        /// </summary>
        /// <param name="client">The client.</param>
        void Client(Client client);

        /// <summary>
        /// Configures an Identity resource.
        /// </summary>
        /// <param name="config">The configuration.</param>
        void IdentityResource(Action<IdentityResource> config)
        {
            var resource = new IdentityResource();
            config(resource);
            IdentityResource(resource);
        }

        /// <summary>
        /// Configures an Identity resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        void IdentityResource(IdentityResource resource);

        /// <summary>
        /// Configures an API resource.
        /// </summary>
        /// <param name="config">The configuration.</param>
        void ApiResource(Action<ApiResource> config)
        {
            var resource = new ApiResource();
            config(resource);
            ApiResource(resource);
        }

        /// <summary>
        /// Configures an API resource.
        /// </summary>
        /// <param name="resource">The resource.</param>
        void ApiResource(ApiResource resource);
    }
}