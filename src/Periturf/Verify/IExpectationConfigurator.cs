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

namespace Periturf.Verify
{
    /// <summary>
    /// Builds the definition of an expectation specification.
    /// </summary>
    public interface IExpectationConfigurator
    {
        /// <summary>
        /// Sets a description for the expectation.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        IExpectationConfigurator Description(string description);

        /// <summary>
        /// Filters applied to condition instances prior to evaluator by criteria.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns></returns>
        IExpectationConfigurator Where(Action<IExpectationFilterConfigurator> config);

        /// <summary>
        /// Sets a critera specification that <c>must</c> be met.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <returns></returns>
        IExpectationConfigurator Must(IExpectationCriteriaSpecification specification);
    }
}
