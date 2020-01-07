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
using System;
using Periturf.Components;
using Periturf.Events;
using Periturf.MT.Configuration;

namespace Periturf.MT.Setup
{
    class BusSpecification : MtSpecification, IBusConfigurator
    {
        private readonly string _hostName;
        private readonly IEventResponseContextFactory _eventResponseContextFactory;
        private IBusManager? _busManager;

        public BusSpecification(string hostName, IEventResponseContextFactory eventResponseContextFactory) : base(hostName)
        {
            _hostName = hostName;
            _eventResponseContextFactory = eventResponseContextFactory;
        }

        public void SetBusManager(IBusManager busManager)
        {
            _busManager = busManager ?? throw new ArgumentNullException(nameof(busManager));
        }

        public IHost Build()
        {
            if (_busManager == null)
                throw new InvalidOperationException("Specification not configured");

            _busManager.Setup(this, _eventResponseContextFactory);

            return new MtHostComponent(_busManager, _hostName);
        }
    }
}
