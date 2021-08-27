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
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Verify
{
    /// <summary>
    /// Feeds expectations with instances of a components expected condition.
    /// </summary>
    public interface IConditionFeed : IAsyncDisposable
    {
        /// <summary>
        /// Gets the instances of the expected component condition, fed asyncronously.
        /// </summary>
        /// <returns></returns>
        Task<List<ConditionInstance>> WaitForInstancesAsync(CancellationToken ct);
    }
}
