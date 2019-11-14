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

namespace Periturf.Verify.Criterias
{
    class RangeCountCriteriaSpecification : IExpectationCriteriaSpecification
    {
        public RangeCountCriteriaSpecification(int? minimum, int? maximum, TimeSpan? timeout, string? description = null)
        {
            if (minimum.HasValue && maximum.HasValue && minimum > maximum)
                throw new ArgumentOutOfRangeException(nameof(maximum), "Maximum must be greater or equal to minimum");

            if (!minimum.HasValue && !maximum.HasValue)
                throw new ArgumentException($"A value must be supplied for either {nameof(minimum)} or {nameof(maximum)}");

            Minimum = minimum;
            Maximum = maximum;
            Timeout = timeout;
            
            if (description != null)
                Description = description;
            else
            {
                if (minimum.HasValue)
                {
                    if (maximum.HasValue)
                        Description = $"Between {minimum} and {maximum} instances";
                    else
                        Description = $"Minimum of {minimum} instances";
                }
                else
                    Description = $"Maximum of {maximum} instances";
            }
        }

        public int? Minimum { get; }
        public int? Maximum { get; }
        public TimeSpan? Timeout { get; }

        public string Description { get; }

        public IExpectationCriteriaEvaluatorFactory Build()
        {
            return new RangeCountCriteriaEvaluatorFactory(Minimum, Maximum);
        }
    }
}
