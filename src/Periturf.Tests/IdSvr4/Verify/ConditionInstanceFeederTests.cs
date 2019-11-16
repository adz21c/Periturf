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
using NUnit.Framework;
using Periturf.IdSvr4.Verify;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.IdSvr4.Verify
{
    [TestFixture]
    class ConditionInstanceFeederTests
    {
        [Test]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Blocker Code Smell", "S2699:Tests should include assertions", Justification = "<Pending>")]
        public async Task Given_Feeder_When_CancelToken_Then_CancelsGracefully()
        {
            var cancellationSource = new CancellationTokenSource();
            var feeder = new ConditionInstanceFeeder();
            var feederTask = Task.Run(async () =>
            {
                await foreach (var item in feeder.GetInstancesAsync().WithCancellation(cancellationSource.Token))
                { }
            });

            await Task.WhenAny(feederTask, Task.Delay(100));
            cancellationSource.Cancel();
            
            await feederTask;
        }
    }
}
