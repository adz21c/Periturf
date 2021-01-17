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

namespace Periturf.Verify
{
    class ExpectationConstraintEvaluator
    {
        private readonly ConditionIdentifier _condition;
        private readonly TimeSpan? _timeConstraintStart;

        public ExpectationConstraintEvaluator(
            ConditionIdentifier condition,
            TimeSpan? timeConstraintStart = null,
            TimeSpan? timeConstraintEnd = null)
        {
            _condition = condition;
            _timeConstraintStart = timeConstraintStart;
            TimeConstraintEnd = timeConstraintEnd;
        }

        public bool Completed { get; private set; }

        public bool? Met { get; private set; }

        
        public TimeSpan? TimeConstraintEnd { get; }

        public void Evaluate(FeedConditionInstance instance)
        {
            if (instance.Identifier != _condition)
            {
                if (TimeConstraintEnd.HasValue && instance.Instance.When > TimeConstraintEnd)
                {
                    Completed = true;
                    Met = false;
                }
                else
                    return;
            }

            if (_timeConstraintStart.HasValue && instance.Instance.When < _timeConstraintStart)
                return;

            if (TimeConstraintEnd.HasValue)
            {
                Completed = true;
                Met = instance.Instance.When <= TimeConstraintEnd;
            }
            else
            {
                Completed = true;
                Met = true;
            }
        }

        public void Evaluate(TimeSpan time)
        {
            if (TimeConstraintEnd.HasValue && time > TimeConstraintEnd)
            {
                Completed = true;
                Met = false;
            }
        }

        public void Timeout()
        {
            if (!Completed)
            {
                Completed = true;
                Met = false;
            }
        }
    }
}
