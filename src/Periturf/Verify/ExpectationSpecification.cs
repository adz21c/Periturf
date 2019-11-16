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
using System.Text;

namespace Periturf.Verify
{
    class ExpectationSpecification : IExpectationConfigurator, IExpectationFilterConfigurator
    {
        private IExpectationCriteriaSpecification? _criteriaSpecification;
        private readonly List<IExpectationFilterSpecification> _filterSpecifications = new List<IExpectationFilterSpecification>();
        private string? _expectationDescription = null;
        private string? _filterDescription = null;

        IExpectationConfigurator IExpectationConfigurator.Description(string description)
        {
            _expectationDescription = description;
            return this;
        }

        IExpectationConfigurator IExpectationConfigurator.Where(Action<IExpectationFilterConfigurator> config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            
            config.Invoke(this);
            return this;
        }

        IExpectationConfigurator IExpectationConfigurator.Must(IExpectationCriteriaSpecification specification)
        {
            _criteriaSpecification = specification ?? throw new ArgumentNullException(nameof(specification));
            return this;
        }

        IExpectationFilterConfigurator IExpectationFilterConfigurator.Description(string description)
        {
            _filterDescription = description;
            return this;
        }

        IExpectationFilterConfigurator IExpectationFilterConfigurator.AddSpecification(IExpectationFilterSpecification specification)
        {
            _filterSpecifications.Add(specification ?? throw new ArgumentNullException(nameof(specification)));
            return this;
        }

        public TimeSpan? Timeout => _criteriaSpecification?.Timeout;

        public ExpectationEvaluator Build(TimeSpan verifierTimeout, IComponentConditionEvaluator componentConditionEvaluator, string componentConditionDescription)
        {
            if (componentConditionEvaluator is null)
                throw new ArgumentNullException(nameof(componentConditionEvaluator));
            
            if (_criteriaSpecification == null)
                throw new InvalidOperationException("Criteria not specified");

            return new ExpectationEvaluator(
                _criteriaSpecification.Timeout ?? verifierTimeout,  // Favour criteria over verifier
                componentConditionEvaluator,
                _filterSpecifications.Select(x => x.Build()).ToList(),
                _criteriaSpecification.Build(),
                GetDescription(componentConditionDescription));
        }

        private string GetDescription(string componentConditionDescription)
        {
            if (!string.IsNullOrWhiteSpace(_expectationDescription))
                return _expectationDescription;

            var filterDescription = _filterDescription ?? 
                string.Join(
                    ", ",
                    _filterSpecifications
                        .Where(x => !string.IsNullOrWhiteSpace(x.Description))
                        .Select(x => x.Description).ToArray());

            var description = new StringBuilder(componentConditionDescription);

            if (!string.IsNullOrWhiteSpace(filterDescription))
                description.Append($" WHERE {filterDescription}");

            if (!string.IsNullOrWhiteSpace(_criteriaSpecification?.Description))
                description.Append($" MUST {_criteriaSpecification.Description}");

            return description.ToString();
        }
    }
}
