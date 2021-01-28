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
using System;
using System.Collections.Generic;
using System.Linq;

namespace Periturf.Verify
{
    class ExpectationSpecification : IExpectationConfigurator, IExpectationSpecification
    {
        private ExpectationSpecification? _expectationSpecifications;
        private readonly List<ExpectationConstraintSpecification> _expectationConstraintSpecifications = new List<ExpectationConstraintSpecification>();

        public void Constraint(Action<IExpectationConstraintConfigurator> config)
        {
            var spec = new ExpectationConstraintSpecification();
            config(spec);
            _expectationConstraintSpecifications.Add(spec);
        }

        public void Then(Action<IExpectationConfigurator> config)
        {
            var spec = new ExpectationSpecification();
            config(spec);
            _expectationSpecifications = spec;
        }

        public IExpectationEvaluator Build()
        {
            return new ExpectationEvaluator(
                _expectationConstraintSpecifications.Select(x => x.Build()).ToList(),
                _expectationSpecifications?.Build());
        }
    }
}
