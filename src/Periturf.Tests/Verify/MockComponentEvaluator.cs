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
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    class MockComponentEvaluator : IComponentConditionEvaluator
    {
        private readonly TimeSpan _delays;
        private readonly int? _numberOfInstances;

        public MockComponentEvaluator(TimeSpan delays, int? numberOfInstances)
        {
            _delays = delays;
            _numberOfInstances = numberOfInstances;
        }

        public int InstanceCount { get; private set; }

        public bool DisposeCalled { get; private set; }

        public void ResetCalls()
        {
            DisposeCalled = false;
            InstanceCount = 0;
        }

        public ValueTask DisposeAsync()
        {
            DisposeCalled = true;
            return new ValueTask();
        }

        public async IAsyncEnumerable<ConditionInstance> GetInstancesAsync([EnumeratorCancellation] CancellationToken ect = default)
        {
            for (int i = 0; !ect.IsCancellationRequested && (!_numberOfInstances.HasValue || i < _numberOfInstances); ++i)
            {
                await Task.Delay(_delays, ect);
                InstanceCount += 1;
                yield return new ConditionInstance(TimeSpan.FromMilliseconds(i * 50), $"ID-{i}");
            }
        }
    }
}
