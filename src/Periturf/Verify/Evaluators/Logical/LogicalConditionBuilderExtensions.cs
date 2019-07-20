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
using Periturf.Verify;
using Periturf.Verify.Evaluators.Logical;
using System.Linq;

namespace Periturf
{
    /// <summary>
    /// Extensions for configuring logical condition evaluators
    /// </summary>
    public static class LogicalConditionBuilderExtensions
    {
        /// <summary>
        /// All conditions must evaluate to true.
        /// </summary>
        /// <param name="context">The condition context.</param>
        /// <param name="conditions">The conditions configuration.</param>
        /// <returns>An "And" condition evaluator</returns>
        public static IConditionSpecification And(this IConditionContext context, params IConditionSpecification[] conditions)
        {
            return new AndConditionSpecification(conditions.ToList());
        }

        /// <summary>
        /// At least one condition must evaluate to true.
        /// </summary>
        /// <remarks>
        /// Once one condition evaluates to true, the rest are not checked.
        /// </remarks>
        /// <param name="context">The condition context.</param>
        /// <param name="conditions">The conditions configuration.</param>
        /// <returns>An "Or" condition evaluator</returns>
        public static IConditionSpecification Or(this IConditionContext context, params IConditionSpecification[] conditions)
        {
            return new OrConditionSpecification(conditions.ToList());
        }

        /// <summary>
        /// Only one condition can evaluate to true.
        /// </summary>
        /// <param name="context">The condition context.</param>
        /// <param name="conditions">The conditions configuration.</param>
        /// <returns>A "Xor" condition evaluator</returns>
        public static IConditionSpecification Xor(this IConditionContext context, params IConditionSpecification[] conditions)
        {
            return new XorConditionSpecification(conditions.ToList());
        }

        /// <summary>
        /// Negates the result of the child condition.
        /// </summary>
        /// <param name="context">The condition context.</param>
        /// <param name="condition">The condition configuration.</param>
        /// <returns>A "Not" condition evaluator</returns>
        public static IConditionSpecification Not(this IConditionContext context, IConditionSpecification condition)
        {
            return new NotConditionSpecification(condition);
        }
    }
}
