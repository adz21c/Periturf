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
        private readonly List<(ConditionIdentifier ID, IConditionSpecification Spec)> _specs;
        private readonly IExpectationSpecification _expectationSpecification;
        private readonly TimeSpan _inactivityTimeout;

        public Verifier(
            TimeSpan inactivityTimeout,
            List<(ConditionIdentifier ID, IConditionSpecification Spec)> specs,
            IExpectationSpecification expectationSpecification)
        {
            _inactivityTimeout = inactivityTimeout;
            _specs = specs;
            _expectationSpecification = expectationSpecification;
        }

        public async Task<VerificationResult> VerifyAsync(CancellationToken ct = default)
        {
            var instanceFactory = new ConditionInstanceFactory(new Time());
            var buildFeeds = _specs.Select(x => new { x.ID, BuildTask = x.Spec.BuildAsync(instanceFactory, ct)}).ToList();

            await Task.WhenAll(buildFeeds.Select(x => x.BuildTask));

            using var evaluateCt = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var timerCt = CancellationTokenSource.CreateLinkedTokenSource(ct);
            var expectation = _expectationSpecification.Build();
            var feeds = buildFeeds.Select(x => new { x.ID, Feed = x.BuildTask.Result }).ToList();
            try
            {
                var feedWaitTasks = new List<(ConditionIdentifier ID, IConditionFeed Feed, Task<List<ConditionInstance>> Task)>();
                var completedFeedWaitTasks = new List<(ConditionIdentifier ID, IConditionFeed Feed, Task<List<ConditionInstance>> Task)>();
                instanceFactory.Start();
                do
                {
                    bool usingInactivityTimer = false;
                    var nextTimer = expectation.NextTimer;
                    TimeSpan timer;
                    if (nextTimer.HasValue)
                        timer = nextTimer.Value;
                    else
                    {
                        timer = _inactivityTimeout;
                        usingInactivityTimer = true;
                    }

                    feedWaitTasks.AddRange(
                        feeds
                        .Where(x => !feedWaitTasks.Select(y => y.ID).Contains(x.ID))
                        .Select(x => (x.ID, x.Feed, x.Feed.WaitForInstancesAsync(evaluateCt.Token))));
                    var timerTask = Task.Delay(timer.Add(TimeSpan.FromMilliseconds(1)), timerCt.Token);

                    await Task.WhenAny(feedWaitTasks.Select(x => x.Task).Concat(new[] { timerTask }));

                    completedFeedWaitTasks = feedWaitTasks
                        .Where(x => x.Task.IsCompletedSuccessfully)
                        .ToList();

                    feedWaitTasks.RemoveAll(x => completedFeedWaitTasks.Select(x => x.ID).Contains(x.ID));

                    var instances = completedFeedWaitTasks
                        .SelectMany(x => x.Task.Result.Select(y => new FeedConditionInstance(x.ID, y)))
                        .Where(x => x.Instance.When != TimeSpan.Zero)
                        .OrderBy(x => x.Instance.When);

                    foreach (var instance in instances)
                    {
                        var result = expectation.Evaluate(instance);
                        if (result.IsCompleted)
#pragma warning disable CS8629 // Nullable value type may be null.
                            return new VerificationResult(result.Met.Value);
#pragma warning restore CS8629 // Nullable value type may be null.
                    }

                    if (timerTask.IsCompletedSuccessfully)
                    {
                        var result = usingInactivityTimer ? expectation.Timeout() : expectation.Evaluate(timer);
                        if (result.IsCompleted)
#pragma warning disable CS8629 // Nullable value type may be null.
                            return new VerificationResult(result.Met.Value);
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
                await Task.WhenAll(feeds.Select(x => x.Feed.DisposeAsync().AsTask()));
            }
        }
    }
}
