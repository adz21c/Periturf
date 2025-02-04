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

namespace Periturf
{
    /// <summary>
    /// Contains details about an error coming from a component.
    /// </summary>
    public record ComponentExceptionDetails
    {
        /// <value>
        /// The name of the component.
        /// </value>
        public required string ComponentName { get; init; }

        /// <value>
        /// The exception.
        /// </value>
        public required Exception Exception { get; init; }
    }
}