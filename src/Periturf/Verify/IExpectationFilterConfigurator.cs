﻿/*
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
    /// Builds an expectation filter specification
    /// </summary>
    public interface IExpectationFilterConfigurator
    {
        /// <summary>
        /// Describes the filter.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <returns></returns>
        IExpectationFilterConfigurator Description(string description);

        /// <summary>
        /// Adds a filter specification to the expectation.
        /// </summary>
        /// <param name="specification">The specification.</param>
        /// <returns></returns>
        IExpectationFilterConfigurator AddSpecification(IExpectationFilterSpecification specification);
    }
}