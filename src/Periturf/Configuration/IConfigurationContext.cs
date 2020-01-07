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
namespace Periturf.Configuration
{
    /// <summary>
    /// Gathers expectation configuration for the environment
    /// </summary>
    public interface IConfigurationContext
    {
        /// <summary>
        /// Creates a component configuration specification for the specified component.
        /// </summary>
        /// <typeparam name="TSpecification">The type of the specification.</typeparam>
        /// <param name="componentName">Name of the component.</param>
        /// <returns></returns>
        TSpecification CreateComponentConfigSpecification<TSpecification>(string componentName)
            where TSpecification : IConfigurationSpecification;

        /// <summary>
        /// Registers configuration specifications.
        /// </summary>
        /// <param name="specification">The specification.</param>
        void AddSpecification(IConfigurationSpecification specification);
    }
}