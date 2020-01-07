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

namespace Periturf.Verify.Criterias.RangeCount
{
    class RangeCountCriteriaEvaluator : IExpectationCriteriaEvaluator
    {
        private int _count;

        public RangeCountCriteriaEvaluator(int? minimum, int? maximum)
        {
            Minimum = minimum;
            Maximum = maximum;

            if ((Minimum ?? 0) == 0)
                Met = true;
        }

        public int? Minimum { get; private set; }

        public int? Maximum { get; private set; }

        public bool? Met { get; private set; } = false;

        public bool Evaluate(ConditionInstance instance)
        {
            _count += 1;

            Met = (!Minimum.HasValue || _count >= Minimum) && (!Maximum.HasValue || _count <= Maximum);

            if (Maximum.HasValue && _count > Maximum)
                return true;

            if (Minimum.HasValue && !Maximum.HasValue && _count >= Minimum)
                return true;

            return false;
        }
    }
}
