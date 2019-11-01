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

        public bool DisposeCalled { get; private set; }

        public void ResetCalls()
        {
            DisposeCalled = false;
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
                yield return new ConditionInstance(TimeSpan.FromMilliseconds(i * 50), $"ID-{i}");
            }
        }
    }
}
