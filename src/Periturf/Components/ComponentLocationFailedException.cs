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

namespace Periturf.Components
{
    /// <summary>
    /// Thrown when a component could not be found.
    /// </summary>
    /// <seealso cref="System.Exception" />
    [Serializable]
    public class ComponentLocationFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentLocationFailedException" /> class.
        /// </summary>
        /// <param name="componentName">Name of the component.</param>
        public ComponentLocationFailedException(string componentName) : base($"Failed to locate component: {componentName}")
        {
            ComponentName = componentName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentStartException" /> class.
        /// </summary>
        /// <param name="componetName">Name of the componet.</param>
        /// <param name="message">The message.</param>
        public ComponentLocationFailedException(string componetName, string message) : base(message)
        {
            ComponentName = componetName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EnvironmentStartException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        protected ComponentLocationFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ComponentName = info.GetString(nameof(ComponentName));
        }

        /// <summary>
        /// Gets the name of the component that couldn't be found.
        /// </summary>
        /// <value>
        /// The component name.
        /// </value>
        public string ComponentName { get; }

        /// <summary>
        /// When overridden in a derived class, sets the <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo"></see> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext"></see> that contains contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(ComponentName), ComponentName);

            base.GetObjectData(info, context);
        }
    }
}
