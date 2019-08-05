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
using System.Runtime.Serialization;

namespace Periturf.Setup
{
    /// <summary>
    /// Thrown when a host name is used multiple times.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class DuplicateHostNameException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateHostNameException"/> class.
        /// </summary>
        /// <param name="hostName">Name of the host.</param>
        public DuplicateHostNameException(string hostName) : base($"Duplicate host name: {hostName}")
        {
            HostName = hostName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateHostNameException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="hostName">Name of the host.</param>
        public DuplicateHostNameException(string message, string hostName) : base(message)
        {
            HostName = hostName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateHostNameException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected DuplicateHostNameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            HostName = info.GetString(nameof(HostName));
        }

        /// <summary>
        /// Gets the name of the host.
        /// </summary>
        /// <value>
        /// The name of the host.
        /// </value>
        public string HostName { get; }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(HostName), HostName);

            base.GetObjectData(info, context);
        }
    }
}
