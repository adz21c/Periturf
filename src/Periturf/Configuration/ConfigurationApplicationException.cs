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

namespace Periturf.Configuration
{
    /// <summary>
    /// Thrown when there are errors while applying configuration to an environment.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class ConfigurationApplicationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationApplicationException"/> class.
        /// </summary>
        /// <param name="details">The component error details.</param>
        public ConfigurationApplicationException(ComponentExceptionDetails[] details = null) : base("There was a problem while applying configuration to the environment")
        {
            Details = details ?? new ComponentExceptionDetails[] { };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationApplicationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="details">The component error details.</param>
        public ConfigurationApplicationException(string message, ComponentExceptionDetails[] details = null) : base(message)
        {
            Details = details ?? new ComponentExceptionDetails[] { };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationApplicationException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected ConfigurationApplicationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            Details = (ComponentExceptionDetails[]) info.GetValue(nameof(Details), typeof(ComponentExceptionDetails[]));
        }

        /// <summary>
        /// Gets the component error details.
        /// </summary>
        /// <value>
        /// The component error details.
        /// </value>
        public ComponentExceptionDetails[] Details { get; }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Details), Details, typeof(ComponentExceptionDetails[]));

            base.GetObjectData(info, context);
        }
    }
}