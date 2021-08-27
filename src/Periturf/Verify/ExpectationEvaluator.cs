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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Periturf.Verify
{
    class ExpectationEvaluator : IExpectationEvaluator
    {
        private readonly List<ExpectationConstraintEvaluator> _constraints;
        private readonly IExpectationEvaluator? _next;
        private bool _evaluated = false;
        private ExpectationResult? _completedResult;

        public ExpectationEvaluator(
            List<ExpectationConstraintEvaluator> constraints,
            IExpectationEvaluator? next)
        {
            _constraints = constraints;
            _next = next;
        }

        public TimeSpan? NextTimer
        {
            get
            {
                if (_evaluated)
                    return _next?.NextTimer;

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
            if (_evaluated)
            {
                Debug.Assert(_next != null, "_next != null");
                var nextResult = _next.Evaluate(instance);
                if (nextResult.IsCompleted)
                    _completedResult = nextResult;

                return nextResult;
            }

            foreach (var constraint in _constraints.Where(x => !x.Completed))
                constraint.Evaluate(instance);

            return TryComplete();
        }

        public ExpectationResult Evaluate(TimeSpan timer)
        {
            if (_constraints.Any(x => x.TimeConstraintEnd.HasValue))
            {
                foreach (var constraint in _constraints.Where(x => x.TimeConstraintEnd.HasValue))
                    constraint.Evaluate(timer);
            }

            return TryComplete();
        }

        public ExpectationResult Timeout()
        {
            if (_completedResult != null)
                return _completedResult;

            foreach (var constraint in _constraints.Where(x => !x.Completed))
                constraint.Timeout();

            return TryComplete();
        }

        private ExpectationResult TryComplete()
        {
            if (!_constraints.All(x => x.Completed))
                return new ExpectationResult(false, null);

            _evaluated = true;

            if (_constraints.Any(x => x.Met == false))
                return _completedResult = new ExpectationResult(true, false);

            if (_next == null)
                return _completedResult = new ExpectationResult(true, true);
            
            // it continues in the next expectation
            return new ExpectationResult(false, null);
        }
    }
}
