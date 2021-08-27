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

namespace Periturf.Setup
{
    /// <summary>
    /// Gathers configuration for environment setup.
    /// </summary>
    public interface ISetupContext
    {
        /// <summary>
        /// Adds a new host specification.
        /// </summary>
        /// <param name="hostSpecification">The host specification.</param>
        void AddHostSpecification(IHostSpecification hostSpecification);

        /// <summary>
        /// Timeout for a verification if there are no condition instances for the specified timespan.
        /// </summary>
        /// <value>
        /// The verify inactivity timeout.
        /// </value>
        TimeSpan VerifyInactivityTimeout { get; set; }
    }
}
