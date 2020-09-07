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
using Microsoft.Extensions.Hosting;
using Periturf.Components;
using Periturf.Setup;
using System;
using System.Collections.Generic;

namespace Periturf.Hosting.Setup
{
    class GenericHostSpecification : IGenericHostConfigurator, IHostSpecification
    {
        private readonly List<IGenericHostComponentSpecification> _componentSpecifications = new List<IGenericHostComponentSpecification>();
        private readonly List<IGenericHostMultipleComponentSpecification> _multipleComponentSpecifications = new List<IGenericHostMultipleComponentSpecification>();

        public void AddComponentSpecification(IGenericHostComponentSpecification spec)
        {
            _componentSpecifications.Add(spec ?? throw new ArgumentNullException(nameof(spec)));
        }

        public void AddMultipleComponentSpecification(IGenericHostMultipleComponentSpecification spec)
        {
            _multipleComponentSpecifications.Add(spec ?? throw new ArgumentNullException(nameof(spec)));
        }

        public Components.IHost Build()
        {
            var builder = Host.CreateDefaultBuilder();
            
            var components = new Dictionary<string, IComponent>();
            foreach (var componentSpec in _componentSpecifications)
                components.Add(componentSpec.Name, componentSpec.Apply(builder));

            foreach (var componentsSpec in _multipleComponentSpecifications)
            {
                var multipleComponents = componentsSpec.Apply(builder);
                foreach (var component in multipleComponents)
                    components.Add(component.Key, component.Value);
            }

            return new HostAdapter(builder.Build(), components);
        }
    }
}
