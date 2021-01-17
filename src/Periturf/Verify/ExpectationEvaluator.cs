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
    class ExpectationEvaluator : IExpectationEvaluator
    {
        private readonly List<ExpectationConstraintEvaluator> _constraints;
        private readonly ExpectationEvaluator? _next;
        private bool _completed = false;
        private ExpectationResult? _completedResult;

        public ExpectationEvaluator(
            List<ExpectationConstraintEvaluator> constraints,
            ExpectationEvaluator? next)
        {
            _constraints = constraints;
            _next = next;
        }

        public TimeSpan? NextTimeout
        {
            get
            {
                if (_completed)
                    return _next?.NextTimeout;

                var nextTimeout = _constraints
                    .Where(x => !x.Completed)
                    .Where(x => x.TimeConstraintEnd.HasValue)
                    .Select(x => x.TimeConstraintEnd)
                    .OrderBy(x => x)
                    .FirstOrDefault();

                return nextTimeout;
            }
        }

        public ExpectationResult Evaluate(FeedConditionInstance instance)
        {
            if (_completedResult != null)
                return _completedResult;

            // If we have another set of constraints then defer to the next set
            if (_next != null && _constraints.All(x => x.Completed && x.Met.Value))
            {
                var nextResult = _next.Evaluate(instance);
                if (nextResult.IsCompleted)
                    _completedResult = nextResult;

                return nextResult;
            }

            foreach (var constraint in _constraints.Where(x => !x.Completed))
            {
                constraint.Evaluate(instance);
            }

            if (!_constraints.All(x => x.Completed))
                return new ExpectationResult(false, false);

            if (_next == null)
            {
                return _completedResult = new ExpectationResult(true, true);
            }
            else
                return new ExpectationResult(false, false);
        }
    }
}
