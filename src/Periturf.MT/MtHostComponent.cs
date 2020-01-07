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
using Periturf.Clients;
using Periturf.Components;
using Periturf.Configuration;
using Periturf.Events;
using Periturf.MT.Clients;
using Periturf.MT.Configuration;
using Periturf.MT.Verify;
using Periturf.Verify;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.MT
{
    class MtHostComponent : Components.IHost, IComponent
    {
        private readonly string _componentName;

        public MtHostComponent(IBusManager busManager, string componentName)
        {
            BusManager = busManager;
            _componentName = componentName;
            Components = new Dictionary<string, IComponent> { { componentName, this } };
        }
        public IBusManager BusManager { get; }

        public IReadOnlyDictionary<string, IComponent> Components { get; }

        public async Task StartAsync(CancellationToken ct = default)
        {
            Debug.Assert(BusManager.BusControl != null);
            await BusManager.BusControl.StartAsync(ct).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken ct = default)
        {
            Debug.Assert(BusManager.BusControl != null);
            await BusManager.BusControl.StopAsync(ct).ConfigureAwait(false);
        }

        public IComponentClient CreateClient()
        {
            Debug.Assert(BusManager.BusControl != null);
            return new ComponentClient(BusManager.BusControl);
        }

        public TComponentConditionBuilder CreateConditionBuilder<TComponentConditionBuilder>() where TComponentConditionBuilder : IComponentConditionBuilder
        {
            return (TComponentConditionBuilder)(object) new MtConditionBuilder(BusManager);
        }

        public TSpecification CreateConfigurationSpecification<TSpecification>(IEventResponseContextFactory eventResponseContextFactory) where TSpecification : IConfigurationSpecification
        {
            return (TSpecification)(object) new MtConfigurationSpecification(BusManager, eventResponseContextFactory, _componentName);
        }
    }
}
