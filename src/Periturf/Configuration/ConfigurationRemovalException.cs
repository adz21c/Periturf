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

using System;

namespace Periturf.Configuration
{
    /// <summary>
    /// Thrown when there are errors while removing configuration from an environment.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ConfigurationRemovalException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationRemovalException"/> class.
        /// </summary>
        /// <param name="id">The identifier for the configuration.</param>
        /// <param name="details">The component error details.</param>
        public ConfigurationRemovalException(Guid id, ComponentExceptionDetails[]? details = null) : base("There was a problem while removing configuration from environment")
        {
            Id = id;
            Details = details ?? [];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationRemovalException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="id">The identifier for the configuration.</param>
        /// <param name="details">The component error details.</param>
        public ConfigurationRemovalException(string message, Guid id, ComponentExceptionDetails[]? details = null) : base(message)
        {
            Id = id;
            Details = details ?? [];
        }

        /// <summary>
        /// Gets the identifier for the configuration.
        /// </summary>
        /// <value>
        /// The identifier for the configuration.
        /// </value>
        public Guid Id { get; }

        /// <summary>
        /// Gets the component error details.
        /// </summary>
        /// <value>
        /// The component error details.
        /// </value>
        public ComponentExceptionDetails[] Details { get; }
    }
}