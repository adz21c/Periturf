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
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Periturf.IdSvr4.Verify
{
    class ConditionInstanceFeeder
    {
        private readonly Channel<ConditionInstance> _channel = Channel.CreateUnbounded<ConditionInstance>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true
        });
        
        public ValueTask PushInstanceAsync(ConditionInstance conditionInstance, CancellationToken ct = default)
        {
            return _channel.Writer.WriteAsync(conditionInstance, ct);
        }

        public async IAsyncEnumerable<ConditionInstance> GetInstancesAsync([EnumeratorCancellation] CancellationToken ect = default)
        {
            while (!ect.IsCancellationRequested && !_channel.Reader.Completion.IsCompleted)
            {
                try
                {
                    await _channel.Reader.WaitToReadAsync(ect);
                }
                catch(OperationCanceledException)
                {
                    break;
                }

                while (_channel.Reader.TryRead(out var conditionInstance))
                    yield return conditionInstance;
            }
        }

        public void Complete()
        {
            _channel.Writer.Complete();
        }
    }
}
