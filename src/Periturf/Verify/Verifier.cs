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
using System.Threading.Tasks;
using Periturf.Verify;

namespace Periturf.Verify
{
    class Verifier : IVerifier
    {
        private readonly List<ExpectationEvaluator> _expectations;
        private readonly bool _shortCircuit;

        private bool _disposed;

        public Verifier(List<ExpectationEvaluator> expectations, bool shortCircuit = false)
        {
            _expectations = expectations;
            _shortCircuit = shortCircuit;
        }

        public async Task<VerificationResult> VerifyAsync()
        {
            if (_disposed)
                throw new ObjectDisposedException(typeof(Verifier).FullName);

            var expectations = _expectations.Select(x => x.EvaluateAsync()).ToList();
            var results = new List<ExpectationResult>(expectations.Count);

            if (_shortCircuit)
            {
                while (expectations.Any())
                {
                    await Task.WhenAny(expectations);
                    var completed = expectations.Where(x => x.IsCompleted).ToList();
                    if (!completed.Any())
                        continue;

                    expectations.RemoveAll(x => completed.Contains(x));
                    results.AddRange(completed.Select(x => x.Result));
                    if (completed.Any(x => !x.Result.Met.Value))
                        break;
                }

                // TODO: Pass on cancellation
                results.AddRange(Enumerable.Repeat(new ExpectationResult(new bool?()), expectations.Count));
            }
            else
            {
                await Task.WhenAll(expectations);
                results.AddRange(expectations.Select(x => x.Result));
                expectations.Clear();
            }

            return new VerificationResult(
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
