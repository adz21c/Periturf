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
using System.Diagnostics.CodeAnalysis;

namespace Periturf.Verify
{

    /// <summary>
    /// The results of an expectation evaluation.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ExpectationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpectationResult" /> class.
        /// </summary>
        /// <param name="met">The met.</param>
        /// <param name="description">The description.</param>
        public ExpectationResult(bool? met, string description)
        {
            Met = met;
            Description = description;
        }

        /// <summary>
        /// Gets a value indicating whether the expectation was met.
        /// </summary>
        /// <value>
        ///   <c>true</c> if met; otherwise, <c>false</c>.
        /// </value>
        public bool? Met { get; }

        /// <summary>
        /// Gets the expectation description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; }

        /// <summary>
        /// Gets a value indicating whether this expectation was fully evaluated or ended prematurely is completed.
        /// </summary>
        /// <value>
        ///   <c>true</c> if completed; otherwise, <c>false</c>.
        /// </value>
        public bool Completed => Met.HasValue;
    }
}