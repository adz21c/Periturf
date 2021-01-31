/*
 *     Copyright 2021 Adam Burton (adz21c@gmail.com)
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
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Verify
{
    /// <summary>
    /// Specification for a condition.
    /// </summary>
    public interface IConditionSpecification
    {
        /// <summary>
        /// Gets the name of the component.
        /// </summary>
        /// <value>
        /// The name of the component.
        /// </value>
        string ComponentName { get; }

        /// <summary>
        /// Gets the condition description.
        /// </summary>
        /// <value>
        /// The condition description.
        /// </value>
        string ConditionDescription { get; }

        /// <summary>
        /// Registers the condition with the component.
        /// </summary>
        /// <param name="conditionInstanceFactory">The condition instance factory.</param>
        /// <param name="ct">The ct.</param>
        /// <returns></returns>
        Task<IConditionFeed> BuildAsync(IConditionInstanceFactory conditionInstanceFactory, CancellationToken ct);
    }
}