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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Periturf.Configuration;
using Periturf.MT.Verify;

namespace Periturf.MT
{
    /// <summary>
    /// Implementation of <see cref="IConfigurationHandle"/> and <see cref="IVerificationHandle"/> for
    /// dynamic receive endpoints.
    /// </summary>
    /// <seealso cref="IConfigurationHandle" />
    /// <seealso cref="IVerificationHandle" />
    public class MtHandle : IConfigurationHandle, IVerificationHandle
    {
        private readonly List<HostReceiveEndpointHandle> _receiveEndpointHandles;
        private bool _disposed;
        private bool _disposing;

        /// <summary>
        /// Initializes a new instance of the <see cref="MtHandle"/> class.
        /// </summary>
        /// <param name="receiveEndpointHandles">The receive endpoint handles.</param>
        public MtHandle(List<HostReceiveEndpointHandle> receiveEndpointHandles)
        {
            _receiveEndpointHandles = receiveEndpointHandles;
        }

        /// <summary>
        /// Disposes the handle asynchronously.
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            if (_disposing)
                return;

            _disposing = true;

            await Task.WhenAll(_receiveEndpointHandles.Select(x => x.StopAsync())).ConfigureAwait(false);

            _disposed = true;
            _disposing = false;
        }
    }
}
