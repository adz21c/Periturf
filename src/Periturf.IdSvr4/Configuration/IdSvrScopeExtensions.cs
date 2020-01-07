﻿/*
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace IdentityServer4.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class IdSvrScopeExtensions
    {
        /// <summary>
        /// Adds a user claim to the scope.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <param name="name">The name.</param>
        [ExcludeFromCodeCoverage]
        public static void UserClaim(this Scope scope, string name)
        {
            scope.UserClaims ??= new List<string>();
            scope.UserClaims.Add(name);
        }
    }
}
