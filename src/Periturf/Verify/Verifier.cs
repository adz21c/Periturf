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
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Verify
{
    class Verifier : IVerifier
    {
        private readonly List<(ConditionIdentifier ID, IConditionFeed Feed)> _feeds;
        private readonly IExpectationEvaluator _expectation;

        private VerificationResult? _result;

        private bool _evaluating;
        private bool _dependenciesDisposed;
        private bool _disposed;

        public Verifier(
            List<(ConditionIdentifier ID, IConditionFeed Feed)> feeds,
            IExpectationEvaluator expectation)
        {
            _feeds = feeds;
            _expectation = expectation;
        }

        public async Task<VerificationResult> EvaluateAsync(CancellationToken ct = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(typeof(Verifier).FullName);

            if (_result != null)
                return _result;

            if (_evaluating)
                throw new InvalidOperationException("Already evaluating");
            
            _evaluating = true;

            using var evaluateCt = CancellationTokenSource.CreateLinkedTokenSource(ct);
            try
            {
                do
                {
                    var workers = _feeds
                        .Select(x => new
                        {
                            Feed = x,
                            Task = x.Feed.WaitForInstancesAsync(evaluateCt.Token)
                        })
                        .ToList();

                    await Task.WhenAny(workers.Select(x => x.Task));

                    var instances = workers
                        .Where(x => x.Task.IsCompletedSuccessfully)
                        .SelectMany(x => x.Task.Result.Select(y => new FeedConditionInstance(x.Feed.ID, y)))
                        .OrderBy(x => x.Instance.When)
                        .ToList();

                    foreach (var instance in instances)
                    {
                        var result = _expectation.Evaluate(instance);
                        if (result.IsCompleted)
                        {
                            return _result = new VerificationResult
                            {
                                AsExpected = result.Met
                            };
                        }
                    }
                } while (true);
            }
            finally
            {
                evaluateCt.Cancel();
                await DisposeDependencies();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            await DisposeDependencies();
            _disposed = true;
        }

        private async Task DisposeDependencies()
        {
            if (_dependenciesDisposed)
                return;

            await Task.WhenAll(_feeds.Select(x => x.Feed.DisposeAsync().AsTask()));
            _dependenciesDisposed = true;
        }
    }
}
