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
using System.Diagnostics.CodeAnalysis;

namespace Periturf.Verify
{
    /// <summary>
    /// A moment when a component condition matched
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ConditionInstance
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionInstance"/> class.
        /// </summary>
        /// <param name="when">The when.</param>
        /// <param name="id">The identifier.</param>
        public ConditionInstance(TimeSpan when, string id)
        {
            When = when;
            ID = id ?? string.Empty;
        }

        /// <summary>
        /// When the condition instance occurred relative to the time of definition.
        /// </summary>
        /// <value>
        /// The when.
        /// </value>
        public TimeSpan When { get; }

        /// <summary>
        /// Component specific identifier for the condition instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string ID { get; }
    }
}
