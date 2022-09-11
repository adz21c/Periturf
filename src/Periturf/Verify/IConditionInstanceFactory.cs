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
    /// Creates condition instances, ensuring a correct <see cref="TimeSpan"/>.
    /// </summary>
    public interface IConditionInstanceFactory
    {
        /// <summary>
        /// Creates a condition instance with a TimeSpan based on the current time (now).
        /// </summary>
        /// <param name="id">The instance identifier.</param>
        /// <returns></returns>
        ConditionInstance Create(string id);

        /// <summary>
        /// Creates a condition instance with a TimeSpan based on the supplied event time.
        /// </summary>
        /// <param name="id">The instance identifier.</param>
        /// <param name="eventDateTime">The event date time.</param>
        /// <returns></returns>
        ConditionInstance Create(string id, DateTime eventDateTime);
    }
}