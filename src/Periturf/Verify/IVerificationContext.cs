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
    /// Context within which the verification will be defined.
    /// </summary>
    public interface IVerificationContext
    {
        /// <summary>
        /// Override the inactivity timeout.
        /// </summary>
        /// <value>
        /// The inactivity timeout.
        /// </value>
        TimeSpan InactivityTimeout { get; set; }

        /// <summary>
        /// Register a condition for verification.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <returns>An identifier for the condition.</returns>
        ConditionIdentifier Condition(Func<IConditionConfigurator, IConditionSpecification> config);

        /// <summary>
        /// Defines the expectation.
        /// </summary>
        /// <param name="config">The configuration.</param>
        void Expect(Action<IExpectationConfigurator> config);
    }
}
