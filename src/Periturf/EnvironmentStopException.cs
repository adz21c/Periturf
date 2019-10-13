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

namespace Periturf
{
    /// <summary>
    /// Holds details of errors while stopping the hosts of an environment.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class EnvironmentStopException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentStopException"/> class.
        /// </summary>
        /// <param name="details">The details.</param>
        public EnvironmentStopException(HostExceptionDetails[]? details = null) : base("Failed to correctly stop environment")
        {
            Details = details ?? new HostExceptionDetails[] { };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentStopException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="details">The details.</param>
        public EnvironmentStopException(string message, HostExceptionDetails[]? details = null) : base(message)
        {
            Details = details ?? new HostExceptionDetails[] { };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentStopException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected EnvironmentStopException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Details = (HostExceptionDetails[])info.GetValue(nameof(Details), typeof(HostExceptionDetails[]));
        }

        /// <summary>
        /// Gets the host error details.
        /// </summary>
        /// <value>
        /// The host error details.
        /// </value>
        public HostExceptionDetails[] Details { get; }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Details), Details, typeof(HostExceptionDetails[]));

            base.GetObjectData(info, context);
        }
    }
}
