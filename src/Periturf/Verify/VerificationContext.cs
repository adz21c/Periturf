/*
 *     Copyright 2021 Adam Burton (adz21c@gmail.com)
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
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Verify
{
    class VerificationContext : IVerificationContext, IConditionConfigurator
    {
        private readonly ComponentLocator _componentLocator;
        private readonly Dictionary<ConditionIdentifier, IConditionSpecification> _conditions = new Dictionary<ConditionIdentifier, IConditionSpecification>();
        private ExpectationSpecification? _expectationSpecification;

        public VerificationContext(ComponentLocator componentLocator)
        {
            _componentLocator = componentLocator;
        }

        public ConditionIdentifier Condition(Func<IConditionConfigurator, IConditionSpecification> config)
        {
            var spec = config(this);
            var identifier = new ConditionIdentifier(spec.ComponentName, spec.ConditionDescription, Guid.NewGuid());
            _conditions.Add(identifier, spec);
            return identifier;
        }

        public void Expect(Action<IExpectationConfigurator> config)
        {
            _expectationSpecification = new ExpectationSpecification();
            config.Invoke(_expectationSpecification);
        }

        public async Task<Verifier> BuildAsync(CancellationToken ct)
        {
            Debug.Assert(_expectationSpecification != null, "_expectationSpecification != null");

            var conditionBuilds = _conditions
                .Select(x => new
                {
                    Identifier = x.Key,
                    Task = x.Value.BuildAsync(ct)
                })
                .ToList();

            await Task.WhenAll(conditionBuilds.Select(x => x.Task));

            return new Verifier(
                TimeSpan.FromMilliseconds(0),   // TODO populate
                conditionBuilds.Select(x => (x.Identifier, x.Task.Result)).ToList(),
                _expectationSpecification.Build());
        }

        IConditionBuilder IConditionConfigurator.GetConditionBuilder(string componentName)
        {
            return _componentLocator.GetComponent(componentName).CreateConditionBuilder();
        }
    }
}
