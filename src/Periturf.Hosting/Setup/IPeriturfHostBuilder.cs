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
using Microsoft.Extensions.Hosting;
using Periturf.Components;

namespace Periturf.Hosting.Setup
{
    /// <summary>
    /// Extension of the <see cref="IHostBuilder"/> to include the ability to register <see cref="IComponent"/>.
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Hosting.IHostBuilder" />
    public interface IPeriturfHostBuilder : IHostBuilder
    {
        /// <summary>
        /// Registers component with the generic host.
        /// </summary>
        /// <param name="componentName">The component name.</param>
        /// <param name="component">The component.</param>
        void AddComponent(string componentName, IComponent component);
    }
}
