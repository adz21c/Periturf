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
    /// Thrown when there are errors while applying configuration to an environment.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ConfigurationApplicationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationApplicationException"/> class.
        /// </summary>
        /// <param name="details">The component error details.</param>
        public ConfigurationApplicationException(ComponentExceptionDetails[]? details = null) : base("There was a problem while applying configuration to the environment")
        {
            Details = details ?? new ComponentExceptionDetails[] { };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationApplicationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="details">The component error details.</param>
        public ConfigurationApplicationException(string message, ComponentExceptionDetails[]? details = null) : base(message)
        {
            Details = details ?? new ComponentExceptionDetails[] { };
        }

        /// <summary>
        /// Gets the component error details.
        /// </summary>
        /// <value>
        /// The component error details.
        /// </value>
        public ComponentExceptionDetails[] Details { get; }
    }
}