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
using FakeItEasy;
using MassTransit;
using NUnit.Framework;
using Periturf.MT;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.MT.Configuration
{
    [TestFixture]
    class MtConfigurationHandleTests
    {
        [Test]
        public async Task Given_Handle_When_DisposeAsync_Then_MtHandle()
        {
            var mtHandle = A.Fake<HostReceiveEndpointHandle>();
            var mtHandle2 = A.Fake<HostReceiveEndpointHandle>();

            var handle = new MtHandle(new List<HostReceiveEndpointHandle> { mtHandle, mtHandle2 });
            await handle.DisposeAsync();

            A.CallTo(() => mtHandle.StopAsync(A<CancellationToken>._)).MustHaveHappened();
            A.CallTo(() => mtHandle2.StopAsync(A<CancellationToken>._)).MustHaveHappened();
        }

        [Test]
        public async Task Given_DisposedHandle_When_DisposeAsync_Then_Nothing()
        {
            var mtHandle = A.Fake<HostReceiveEndpointHandle>();

            var handle = new MtHandle(new List<HostReceiveEndpointHandle> { mtHandle });
            await handle.DisposeAsync();

            Fake.ClearRecordedCalls(mtHandle);

            await handle.DisposeAsync();

            A.CallTo(() => mtHandle.StopAsync(A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Given_ConcurrentDispose_When_DisposeAsync_Then_SingleHandleDispose()
        {
            var mtHandle = A.Fake<HostReceiveEndpointHandle>();
            A.CallTo(() => mtHandle.StopAsync(A<CancellationToken>._)).ReturnsLazily(() => Task.Delay(500));

            var handle = new MtHandle(new List<HostReceiveEndpointHandle> { mtHandle });
            var originalDispose = Task.Run(async () => await handle.DisposeAsync());

            // Just enough delay that Stop is happening
            await Task.Delay(100);
            await handle.DisposeAsync();
            await originalDispose;

            A.CallTo(() => mtHandle.StopAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }
    }
}
