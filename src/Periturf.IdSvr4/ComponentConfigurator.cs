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
using Periturf.Components;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.IdSvr4
{
    class ComponentConfigurator : IComponentConfigurator
    {
        private readonly IdSvr4Component _component;
        private readonly ConfigurationRegistration _config;

        public ComponentConfigurator(IdSvr4Component component, ConfigurationRegistration config)
        {
            _component = component;
            _config = config;
        }

        public Task RegisterConfigurationAsync(Guid id, CancellationToken ct = default)
        {
            _component.RegisterConfiguration(id, _config);
            return Task.CompletedTask;
        }
    }
}
