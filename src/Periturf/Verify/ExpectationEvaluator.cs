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
using System.Threading.Tasks;

namespace Periturf.Verify
{
    class ExpectationEvaluator : IAsyncDisposable
    {
        private readonly IComponentConditionEvaluator _componentConditionEvaluator;
        private readonly IReadOnlyList<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>> _filters;
        private readonly IExpectationCriteriaEvaluator _criteria;

        private bool _disposed;

        public ExpectationEvaluator(
            IComponentConditionEvaluator componentConditionEvaluator,
            List<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>> filters,
            IExpectationCriteriaEvaluator criteria)
        {
            _componentConditionEvaluator = componentConditionEvaluator;
            _filters = filters;
            _criteria = criteria;
        }

        public TimeSpan? Timeout => _criteria.Timeout;

        public async Task<ExpectationResult> EvaluateAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(typeof(ExpectationEvaluator).FullName);

            var conditions = _componentConditionEvaluator.GetInstancesAsync();
            foreach (var filter in _filters)
                conditions = filter(conditions);

            await foreach(var instance in conditions)
            {
                _criteria.Evaluate(instance);
                if (_criteria.Completed)
                    break;
            }

            return new ExpectationResult(_criteria.Met);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            await _componentConditionEvaluator.DisposeAsync();
            _disposed = true;
        }
    }
}
