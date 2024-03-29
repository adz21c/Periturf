﻿//
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
using System.Linq;
using System.Threading.Tasks;

namespace Periturf.Configuration
{
    class ConfigurationHandle : IConfigurationHandle
    {
        private readonly List<IConfigurationHandle> _handles;
        private bool _disposed;
        private bool _disposing;

        public ConfigurationHandle(IEnumerable<IConfigurationHandle> handles)
        {
            _handles = handles.ToList();
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            if (_disposing)
                throw new InvalidOperationException("Already disposing");

            _disposing = true;

            await Task.WhenAll(_handles.Select(x => x.DisposeAsync().AsTask()).ToList());

            _disposed = true;
            _disposing = false;
        }
    }
}
