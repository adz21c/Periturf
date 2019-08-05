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
using Microsoft.AspNetCore.Hosting;
using Periturf.Components;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.AspNetCore.Setup
{
    class WebHostAdapter : IHost
    {
        private readonly IWebHost _host;

        public WebHostAdapter(IWebHost host, IDictionary<string, IComponent> components)
        {
            _host = host;
            Components = new ReadOnlyDictionary<string, IComponent>(components);
        }

        public IReadOnlyDictionary<string, IComponent> Components { get; }

        public Task StartAsync(CancellationToken ct = default)
        {
            return _host.StartAsync(ct);
        }

        public Task StopAsync(CancellationToken ct = default)
        {
            return _host.StopAsync(ct);
        }
    }
}
