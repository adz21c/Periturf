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
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Verify
{
    class ExpectationEvaluator : IAsyncDisposable
    {
        private readonly IComponentConditionEvaluator _componentConditionEvaluator;
        private readonly IReadOnlyList<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>> _filters;
        private readonly IExpectationCriteriaEvaluatorFactory _criteria;

        private ExpectationResult? _result;

        private bool _evaluating;
        private bool _dependenciesDisposed;
        private bool _disposed;

        public ExpectationEvaluator(
            TimeSpan timeout,
            IComponentConditionEvaluator componentConditionEvaluator,
            List<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>> filters,
            IExpectationCriteriaEvaluatorFactory criteria,
            string description = "")
        {
            Timeout = timeout;
            _componentConditionEvaluator = componentConditionEvaluator;
            _filters = filters;
            _criteria = criteria;
            Description = description;
        }

        public string Description { get; }

        public TimeSpan Timeout { get; }

        public async Task<ExpectationResult> EvaluateAsync(CancellationToken ct = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(typeof(ExpectationEvaluator).FullName);

            if (_result != null)
                return _result;

            if (_evaluating)
                throw new InvalidOperationException("Already evaluating");
            
            _evaluating = true;

            // Build condition instance filter
            var conditions = _componentConditionEvaluator.GetInstancesAsync();
            foreach (var filter in _filters)
                conditions = filter(conditions);

            // Prepare cancellation
            using (var expectationTimeout = new CancellationTokenSource(Timeout))
            using (var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(ct, expectationTimeout.Token))
            {
                // Start evaluating
                var criteriaEvaluator = _criteria.CreateInstance();
                try
                {
                    await foreach (var instance in conditions.WithCancellation(cancellationTokenSource.Token))
                    {
                        var done = criteriaEvaluator.Evaluate(instance);
                        if (done)
                            break;
                    }
                }
                catch (TaskCanceledException) when (expectationTimeout.IsCancellationRequested)
                {
                    // if cancelled by expectation timeout then act as if the stream finished
                    return await CompleteExpectationAsync(criteriaEvaluator);
                }
                finally
                {
                    _evaluating = false;
                }

                return await CompleteExpectationAsync(criteriaEvaluator);
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            await DisposeDependenciesAsync();
            _disposed = true;
        }

        private async Task<ExpectationResult> CompleteExpectationAsync(IExpectationCriteriaEvaluator criteriaEvaluator)
        {
            await DisposeDependenciesAsync();
            return _result = new ExpectationResult(
                criteriaEvaluator.Met,
                Description);
        }

        private async ValueTask DisposeDependenciesAsync()
        {
            if (_dependenciesDisposed)
                return;

            await _componentConditionEvaluator.DisposeAsync();
            _dependenciesDisposed = true;
        }
    }
}
