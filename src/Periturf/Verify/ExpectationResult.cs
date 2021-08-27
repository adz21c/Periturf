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

using System.Diagnostics.CodeAnalysis;

namespace Periturf.Verify
{
    /// <summary>
    /// The result of evaluating a <see cref="ConditionInstance"/>
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class ExpectationResult
    {
        internal ExpectationResult(bool isCompleted, bool? met)
        {
            IsCompleted = isCompleted;
            Met = met;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ExpectationEvaluator"/> has completed evaluating.
        /// </summary>
        /// <value>
        ///   <c>true</c> if is completed; otherwise, <c>false</c>.
        /// </value>
        public bool IsCompleted { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ExpectationEvaluator"/> determined the condition instances have met the criteria.
        /// </summary>
        /// <value>
        /// <c>null</c> when done complete evaluating, <c>true</c> if the conditions met the criteria, or <c>false</c> if the conditions didn't meet the criteria.
        /// </value>
        public bool? Met { get; }
    }
}
