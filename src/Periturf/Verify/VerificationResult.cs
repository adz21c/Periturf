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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Periturf.Verify
{
    /// <summary>
    /// The aggregated result expectations.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class VerificationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VerificationResult"/> class.
        /// </summary>
        /// <param name="expectationsMet">if set to <c>true</c> [expectations met].</param>
        /// <param name="expectationResults">The expectation results.</param>
        public VerificationResult(bool expectationsMet, IReadOnlyList<ExpectationResult> expectationResults)
        {
            ExpectationsMet = expectationsMet;
            ExpectationResults = expectationResults;
        }

        /// <summary>
        /// Gets a value indicating whether all expectations were met.
        /// </summary>
        /// <value>
        ///   <c>true</c> if all expectations were met; otherwise, <c>false</c>.
        /// </value>
        public bool ExpectationsMet { get; }

        /// <summary>
        /// Gets the expectation results.
        /// </summary>
        /// <value>
        /// The expectation results.
        /// </value>
        public IReadOnlyList<ExpectationResult> ExpectationResults { get; }
    }
}