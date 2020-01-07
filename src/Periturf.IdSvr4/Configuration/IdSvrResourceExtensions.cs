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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace IdentityServer4.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class IdSvrResourceExtensions
    {
        /// <summary>
        /// Adds a user claim to the resource
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="name">The name.</param>
        [ExcludeFromCodeCoverage]
        public static void UserClaim(this Resource resource, string name)
        {
            resource.UserClaims ??= new List<string>();
            resource.UserClaims.Add(name);
        }

        /// <summary>
        /// Adds a secret to the API resource
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="secret">The secret.</param>
        [ExcludeFromCodeCoverage]
        public static void Secret(this ApiResource resource, string secret)
        {
            resource.Secret(new Secret(secret));
        }

        /// <summary>
        /// Adds a secret to the API resource
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="config">The configuration.</param>
        [ExcludeFromCodeCoverage]
        public static void Secret(this ApiResource resource, Action<Secret> config)
        {
            var secret = new Secret();
            config(secret);
            resource.Secret(secret);
        }

        /// <summary>
        /// Adds a secret to the API resource
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="secret">The secret.</param>
        [ExcludeFromCodeCoverage]
        public static void Secret(this ApiResource resource, Secret secret)
        {
            resource.ApiSecrets ??= new List<Secret>();
            resource.ApiSecrets.Add(secret);
        }


        /// <summary>
        /// Adds a scope to the API resource
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="config">The configuration.</param>
        [ExcludeFromCodeCoverage]
        public static void Scope(this ApiResource resource, Action<Scope> config)
        {
            var scope = new Scope();
            config(scope);
            resource.Scope(scope);
        }

        /// <summary>
        /// Adds a scope to the API resource
        /// </summary>
        /// <param name="resource">The resource.</param>
        /// <param name="scope">The scope.</param>
        [ExcludeFromCodeCoverage]
        public static void Scope(this ApiResource resource, Scope scope)
        {
            resource.Scopes ??= new List<Scope>();
            resource.Scopes.Add(scope);
        }
    }
}
