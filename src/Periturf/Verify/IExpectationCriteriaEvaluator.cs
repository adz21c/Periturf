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

namespace Periturf.Verify
{
    /// <summary>
    /// Checks whether one or more condition instances match a criteria.
    /// </summary>
    public interface IExpectationCriteriaEvaluator
    {
        /// <summary>
        /// Gets a value indicating whether the criteria has been met.
        /// </summary>
        /// <value>
        ///   <c>true</c> if met; otherwise, <c>false</c>.
        /// </value>
        bool? Met { get; }

        /// <summary>
        /// Evaluates the specified condition instance.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns><c>true</c> if the evaluator has completed evaluation early, else <c>false</c></returns>
        bool Evaluate(ConditionInstance instance);
    }
}