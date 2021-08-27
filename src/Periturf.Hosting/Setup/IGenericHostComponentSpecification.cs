//
//   Copyright 2021 Adam Burton (adz21c@gmail.com)
//   
//   Licensed under the Apache License, Version 2.0 (the "License")
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//

using Microsoft.Extensions.Hosting;
using Periturf.Components;

namespace Periturf.Hosting.Setup
{
    /// <summary>
    /// Specifies a .NET Core Generic Host component.
    /// </summary>
    public interface IGenericHostComponentSpecification
    {
        /// <summary>
        /// The unique name for the component.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; }

        /// <summary>
        /// Creates and applies a <see cref="IComponent"/> to the provided host.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <returns></returns>
        IComponent Apply(IHostBuilder hostBuilder);
    }
}
