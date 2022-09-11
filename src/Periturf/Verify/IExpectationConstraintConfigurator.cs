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

using System;

namespace Periturf.Verify
{
    /// <summary>
    /// Configures an expectation constraint.
    /// </summary>
    public interface IExpectationConstraintConfigurator
    {
        /// <summary>
        /// The condition the constraint listens for.
        /// </summary>
        /// <param name="conditionIdentifier">The condition identifier.</param>
        /// <returns></returns>
        IExpectationConstraintConfigurator Condition(ConditionIdentifier conditionIdentifier);

        /// <summary>
        /// The condition must occur between the two times.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <returns></returns>
        IExpectationConstraintConfigurator Between(TimeSpan start, TimeSpan end);

        /// <summary>
        /// The condition must occur after the specified time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        IExpectationConstraintConfigurator After(TimeSpan time);

        /// <summary>
        /// The condition must occur before the specified time.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns></returns>
        IExpectationConstraintConfigurator Before(TimeSpan time);
    }
}
