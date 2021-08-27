//
//   Copyright 2021 Adam Burton (adz21c@gmail.com)
//   
//   Licensed under the Apache License, Version 2.0 (the "License")
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//

using System;
using System.Diagnostics;

namespace Periturf.Verify
{
    class ExpectationConstraintSpecification : IExpectationConstraintConfigurator
    {
        private ConditionIdentifier? _conditionIdentifier;
        private TimeSpan? _timeConstraintStart;
        private TimeSpan? _timeConstraintEnd;

        public IExpectationConstraintConfigurator Condition(ConditionIdentifier conditionIdentifier)
        {
            _conditionIdentifier = conditionIdentifier;
            return this;
        }

        public IExpectationConstraintConfigurator Between(TimeSpan start, TimeSpan end)
        {
            _timeConstraintStart = start;
            _timeConstraintEnd = end;
            return this;
        }

        public IExpectationConstraintConfigurator After(TimeSpan time)
        {
            _timeConstraintStart = time;
            return this;
        }

        public IExpectationConstraintConfigurator Before(TimeSpan time)
        {
            _timeConstraintEnd = time;
            return this;
        }

        public ExpectationConstraintEvaluator Build()
        {
            Debug.Assert(_conditionIdentifier != null, "_conditionIdentifier != null");

            return new ExpectationConstraintEvaluator(
                _conditionIdentifier,
                _timeConstraintStart,
                _timeConstraintEnd);
        }
    }
}
