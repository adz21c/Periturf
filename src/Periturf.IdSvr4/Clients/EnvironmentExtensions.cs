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
using Periturf.IdSvr4.Clients;

namespace Periturf
{
    /// <summary>
    /// 
    /// </summary>
    public static class EnvironmentExtensions
    {
        /// <summary>
        /// Creates an IdentityServer4 client for the component within the environment.
        /// </summary>
        /// <param name="environment">The environment.</param>
        /// <param name="componentName">Name of the component for which the client belongs.</param>
        /// <returns></returns>
        public static IIdSvr4Client IdSvr4Client(this Environment environment, string componentName = "IdSvr4")
        {
            return (IIdSvr4Client)environment.CreateComponentClient(componentName);
        }
    }
}
