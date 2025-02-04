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

namespace Periturf
{
    /// <summary>
    /// Holds details of errors while stopping the hosts of an environment.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class EnvironmentStopException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentStopException"/> class.
        /// </summary>
        /// <param name="details">The details.</param>
        public EnvironmentStopException(HostExceptionDetails[]? details = null) : base("Failed to correctly stop environment")
        {
            Details = details ?? [];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentStopException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="details">The details.</param>
        public EnvironmentStopException(string message, HostExceptionDetails[]? details = null) : base(message)
        {
            Details = details ?? [];
        }

        /// <summary>
        /// Gets the host error details.
        /// </summary>
        /// <value>
        /// The host error details.
        /// </value>
        public HostExceptionDetails[] Details { get; }
    }
}
