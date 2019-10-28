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
using System.Collections.Generic;
using System.Linq;

namespace Periturf.Verify
{
    class ExpectationSpecification : IExpectationConfigurator, IExpectationFilterConfigurator
    {
        private IExpectationCriteriaSpecification? _criteriaSpecification;
        private readonly List<IExpectationFilterSpecification> _filterSpecifications = new List<IExpectationFilterSpecification>();
        //private string? _description = null;

        //IExpectationConfigurator IExpectationConfigurator.Description(string description)
        //{
        //    if (string.IsNullOrWhiteSpace(description))
        //        throw new ArgumentOutOfRangeException(nameof(description));
            
        //    _description = description;
        //    return this;
        //}

        IExpectationConfigurator IExpectationConfigurator.Where(Action<IExpectationFilterConfigurator> config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            
            config?.Invoke(this);
            return this;
        }

        IExpectationConfigurator IExpectationConfigurator.Must(IExpectationCriteriaSpecification specification)
        {
            _criteriaSpecification = specification ?? throw new ArgumentNullException(nameof(specification));
            return this;
        }

        void IExpectationFilterConfigurator.AddSpecification(IExpectationFilterSpecification specification)
        {
            _filterSpecifications.Add(specification ?? throw new ArgumentNullException(nameof(specification)));
        }

        public ExpectationEvaluator Build(IComponentConditionEvaluator componentConditionEvaluator)
        {
            if (componentConditionEvaluator is null)
                throw new ArgumentNullException(nameof(componentConditionEvaluator));
            
            if (_criteriaSpecification == null)
                throw new InvalidOperationException("Criteria not specified");

            return new ExpectationEvaluator(
                componentConditionEvaluator,
                _filterSpecifications.Select(x => x.Build()).ToList(),
                _criteriaSpecification.Build());
        }
    }
}
