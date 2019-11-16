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

        private VerificationResult? _result;

        private bool _verifying;
        private bool _dependenciesDisposed;
        private bool _disposed;

        public Verifier(List<ExpectationEvaluator> expectations, bool shortCircuit = false)
        {
            Expectations = expectations;
            ShortCircuit = shortCircuit;
        }

        public bool ShortCircuit { get; }
        public List<ExpectationEvaluator> Expectations { get; }

        public async Task<VerificationResult> VerifyAsync(CancellationToken ct = default)
        {
            if (_disposed)
                throw new ObjectDisposedException(typeof(Verifier).FullName);

            if (_result != null)
                return _result;

            if (_verifying)
                throw new InvalidOperationException("Already verifying");

            _verifying = true;

            using (var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(ct))
            {
                var results = new List<ExpectationResult>(Expectations.Count);
                var expectations = new List<(ExpectationEvaluator evaluator, Task<ExpectationResult> task)>(Expectations.Count);

                try
                {
                    expectations.AddRange(Expectations.Select(x => (x, x.EvaluateAsync(cancellationTokenSource.Token))));

                    if (ShortCircuit)
                    {
                        while (expectations.Any())
                        {
                            await Task.WhenAny(expectations.Select(x => x.task));
                            var completed = expectations.Where(x => x.task.IsCompleted).ToList();

                            expectations.RemoveAll(x => completed.Contains(x));
                            results.AddRange(completed.Select(x => x.task.Result));

                            // If any failed then lets break and cancel the rest
                            if (completed.Any(x => !(x.task.Result.Met ?? false)))
                            {
                                cancellationTokenSource.Cancel();   // Cancel remaining tasks
                                // Pass over remaining incomplete tasks
                                results.AddRange(
                                    expectations.Select(x => new ExpectationResult(
                                        null,
                                        x.evaluator.Description)));
                                break;
                            }
                        }
                    }
                    else
                    {
                        await Task.WhenAll(expectations.Select(x => x.task));
                        results.AddRange(expectations.Select(x => x.task.Result));
                        expectations.Clear();
                    }
                }
                finally
                {
                    _verifying = false;
                }

                await DisposeDependencies();

                return _result = new VerificationResult(
                    results.All(x => x.Met ?? false),
                    results);
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

            await Task.WhenAll(Expectations.Select(x => x.DisposeAsync().AsTask()));
            _dependenciesDisposed = true;
        }
    }
}
