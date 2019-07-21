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
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Verify
{
    /// <summary>
    /// Defines a condition, and for components can register a condition for verification with a component.
    /// </summary>
    public interface IConditionSpecification
    {
        /// <summary>
        /// Returns an evaluator for the specified condition.
        /// </summary>
        /// <param name="verifierId">The verifier identifier to associate listeners with.</param>
        /// <param name="erasePlan">The plan to erase conditions.</param>
        /// <param name="ct">The ct.</param>
        /// <returns>
        /// An evaluator for the specified conditon
        /// </returns>
        Task<IConditionEvaluator> BuildEvaluatorAsync(Guid verifierId, IConditionErasePlan erasePlan, CancellationToken ct = default);
    }
}