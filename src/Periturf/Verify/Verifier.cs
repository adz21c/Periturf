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
        private readonly TimeSpan _inactivityTimeout;

        private VerificationResult? _result;

        private bool _verifying;
        private bool _dependenciesDisposed;
        private bool _disposed;

        public Verifier(
            TimeSpan inactivityTimeout,
            List<(ConditionIdentifier ID, IConditionFeed Feed)> feeds,
            IExpectationEvaluator expectation)
        {
            _inactivityTimeout = inactivityTimeout;
            _feeds = feeds;
            _expectation = expectation;
        }

        public async Task<VerificationResult> VerifyAsync(CancellationToken ct = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(typeof(Verifier).FullName);

            if (_result != null)
                return _result;

            if (_verifying)
                throw new InvalidOperationException("Already verifying");
            
            _verifying = true;

            using var evaluateCt = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var timerCt = CancellationTokenSource.CreateLinkedTokenSource(ct);
            try
            {
                var feedWaitTasks = new List<(ConditionIdentifier ID, IConditionFeed Feed, Task<List<ConditionInstance>> Task)>();
                var completedFeedWaitTasks = new List<(ConditionIdentifier ID, IConditionFeed Feed, Task<List<ConditionInstance>> Task)>();
                do
                {
                    bool usingInactivityTimer = false;
                    var nextTimer = _expectation.NextTimer;
                    TimeSpan timer;
                    if (nextTimer.HasValue)
                        timer = nextTimer.Value;
                    else
                    {
                        timer = _inactivityTimeout;
                        usingInactivityTimer = true;
                    }

                    feedWaitTasks.AddRange(
                        _feeds
                        .Where(x => !feedWaitTasks.Select(y => y.ID).Contains(x.ID))
                        .Select(x => (x.ID, x.Feed, x.Feed.WaitForInstancesAsync(evaluateCt.Token))));
                    var timerTask = Task.Delay(timer, timerCt.Token);

                    await Task.WhenAny(feedWaitTasks.Select(x => x.Task).Concat(new[] { timerTask }));

                    completedFeedWaitTasks = feedWaitTasks
                        .Where(x => x.Task.IsCompletedSuccessfully)
                        .ToList();

                    feedWaitTasks.RemoveAll(x => completedFeedWaitTasks.Select(x => x.ID).Contains(x.ID));

                    var instances = completedFeedWaitTasks
                        .SelectMany(x => x.Task.Result.Select(y => new FeedConditionInstance(x.ID, y)))
                        .OrderBy(x => x.Instance.When);

                    foreach (var instance in instances)
                    {
                        var result = _expectation.Evaluate(instance);
                        if (result.IsCompleted)
#pragma warning disable CS8629 // Nullable value type may be null.
                            return _result = new VerificationResult(result.Met.Value);
#pragma warning restore CS8629 // Nullable value type may be null.
                    }

                    if (timerTask.IsCompletedSuccessfully)
                    {
                        var result = usingInactivityTimer ? _expectation.Timeout() : _expectation.Evaluate(timer);
                        if (result.IsCompleted)
#pragma warning disable CS8629 // Nullable value type may be null.
                            return _result = new VerificationResult(result.Met.Value);
#pragma warning restore CS8629 // Nullable value type may be null.
                    }
                    else
                    {
                        timerCt.Cancel();
                        timerCt = CancellationTokenSource.CreateLinkedTokenSource(ct);
                    }
                } while (true);
            }
            finally
            {
                evaluateCt.Cancel();
                timerCt.Cancel();
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
