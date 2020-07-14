/*
 *     Copyright 2020 Adam Burton (adz21c@gmail.com)
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
using Periturf.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Periturf.Setup
{
    class EnvironmentSpecification : ISetupContext
    {
        private readonly List<IHostSpecification> _hostSpecifications = new List<IHostSpecification>();

        // Validate greater than 0
        public TimeSpan DefaultExpectationTimeout { get; set; }

        public bool DefaultExpectationShortCircuit { get; set; }

        public void AddHostSpecification(IHostSpecification hostSpecification)
        {
            _hostSpecifications.Add(hostSpecification ?? throw new ArgumentNullException(nameof(hostSpecification)));
        }

        public List<IHost> Build()
        {
            var hosts = _hostSpecifications.Select(hs => hs.Build()).ToList();
            if (hosts.Any(x => x == null))
                throw new InvalidOperationException("Host not created.");
            return hosts;
        }
    }
}
