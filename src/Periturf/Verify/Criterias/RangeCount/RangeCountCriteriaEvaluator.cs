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

namespace Periturf.Verify.Criterias
{
    class RangeCountCriteriaEvaluator : IExpectationCriteriaEvaluator
    {
        private readonly int? _min;
        private readonly int? _max;

        private int _count;

        public RangeCountCriteriaEvaluator(int? min, int? max)
        {
            _min = min;
            _max = max;
        }

        public bool Completed => true;

        public bool? Met { get; private set; } = false;

        public void Evaluate(ConditionInstance instance)
        {
            _count += 1;

            if ((!_min.HasValue || _count >= _min) && (!_max.HasValue || _count <= _max))
                Met = true;
        }
    }
}
