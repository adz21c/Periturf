﻿//
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
using System.Diagnostics.CodeAnalysis;

namespace Periturf
{
    /// <summary>
    /// Contains details about an error coming from a host.
    /// </summary>
    [Serializable]
    public class HostExceptionDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HostExceptionDetails"/> class.
        /// </summary>
        [ExcludeFromCodeCoverage]
#pragma warning disable CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        protected HostExceptionDetails()
#pragma warning restore CS8618 // Non-nullable field is uninitialized. Consider declaring as nullable.
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HostExceptionDetails"/> class.
        /// </summary>
        /// <param name="exception">The exception.</param>
        public HostExceptionDetails(Exception exception)
        {
            Exception = exception;
        }

        /// <summary>
        /// Gets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; }
    }
}
