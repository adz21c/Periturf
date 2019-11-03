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
        private readonly List<ExpectationEvaluator> _expectations;
        private readonly bool _shortCircuit;

        private VerificationResult? _result;

        private bool _disposed;

        public Verifier(List<ExpectationEvaluator> expectations, bool shortCircuit = false)
        {
            _expectations = expectations;
            _shortCircuit = shortCircuit;
        }

        public async Task<VerificationResult> VerifyAsync(CancellationToken ct = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(typeof(Verifier).FullName);

            using (var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(ct))
            {
                var results = new List<ExpectationResult>(_expectations.Count);
                var expectations = new List<Task<ExpectationResult>>(_expectations.Count);

                try
                {
                    expectations.AddRange(_expectations.Select(x => x.EvaluateAsync(cancellationTokenSource.Token)));

                    if (_shortCircuit)
                    {
                        while (expectations.Any())
                        {
                            await Task.WhenAny(expectations);
                            var completed = expectations.Where(x => x.IsCompleted).ToList();

                            expectations.RemoveAll(x => completed.Contains(x));
                            results.AddRange(completed.Select(x => x.Result));

                            // If any failed then lets break and cancel the rest
                            if (completed.Any(x => !(x.Result.Met ?? false)))
                                break;
                        }
                    }
                    else
                    {
                        await Task.WhenAll(expectations);
                        results.AddRange(expectations.Select(x => x.Result));
                        expectations.Clear();
                    }
                }
                catch(TaskCanceledException)
                {
                    SetResult(cancellationTokenSource, results, expectations);
                    throw;
                }

                // Cancel remaining work and pass them on as inconclusive
                SetResult(cancellationTokenSource, results, expectations);

                return _result;
            }
        }

        private void SetResult(CancellationTokenSource cancellationTokenSource, List<ExpectationResult> results, List<Task<ExpectationResult>> expectations)
        {
            // Cancel the remaining
            if (expectations.Any(x => !x.IsCompleted))
                cancellationTokenSource.Cancel();

            // Transfer over the results
            // TODO: Handle Faults
            results.AddRange(
                expectations.Where(x => !x.IsFaulted).Select(x => new ExpectationResult(
                    // TODO: Pass on description
                    !x.IsCompleted ? false : x.IsCanceled ? new bool?() : x.Result.Met ?? false)));

            _result = new VerificationResult(
                results.All(x => x.Met ?? false),
                results);
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            await Task.WhenAll(_expectations.Select(x => x.DisposeAsync().AsTask()));
            _disposed = true;
        }
    }
}
