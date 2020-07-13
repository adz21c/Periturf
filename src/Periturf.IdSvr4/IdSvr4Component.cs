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
using Periturf.IdSvr4.Clients;
using Periturf.IdSvr4.Configuration;
using Periturf.IdSvr4.Verify;
using Periturf.Verify;

namespace Periturf.IdSvr4
{
    class IdSvr4Component : IComponent
    {
        private readonly IStore _store;
        private readonly IEventMonitorSink _eventMonitor;
        private readonly IIdSvr4Client _client;

        public IdSvr4Component(IStore store, IEventMonitorSink eventMonitor, IIdSvr4Client client)
        {
            _store = store;
            _eventMonitor = eventMonitor;
            _client = client;
        }

        public TComponentConditionBuilder CreateConditionBuilder<TComponentConditionBuilder>()
            where TComponentConditionBuilder : IComponentConditionBuilder
        {
            return (TComponentConditionBuilder)(object)new IdSvr4ConditionBuilder(_eventMonitor);
        }

        public IComponentClient CreateClient()
        {
            return _client;
        }

        public TSpecification CreateConfigurationSpecification<TSpecification>(IEventHandlerFactory eventHandlerFactory) where TSpecification : IConfigurationSpecification
        {
            return (TSpecification)(object)new IdSvr4Specification(_store);
        }
    }
}
