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
using NUnit.Framework;
using Periturf.Configuration;
using System;
using System.Threading.Tasks;

namespace Periturf.Tests.Configuration
{
    [TestFixture]
    class ConfigurationHandleTests
    {
        [Test]
        public async Task Given_Disposables_When_Dispose_Then_AllDisposed()
        {
            var internalHandle1 = A.Fake<IConfigurationHandle>();
            var internalHandle2 = A.Fake<IConfigurationHandle>();

            var handle = new ConfigurationHandle(new[] { internalHandle1, internalHandle2 });
            await handle.DisposeAsync();

            A.CallTo(() => internalHandle1.DisposeAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => internalHandle2.DisposeAsync()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Given_DisposingHandle_When_Dispose_Then_Throws()
        {
            var internalHandle1 = A.Fake<IConfigurationHandle>();
            // Force delay so the second dispose fails during dispose
            A.CallTo(() => internalHandle1.DisposeAsync()).Invokes(async () => await Task.Delay(100));

            var handle = new ConfigurationHandle(new[] { internalHandle1 });
            var disposeTask = Task.Run(() => handle.DisposeAsync().AsTask());

            Assert.ThrowsAsync<InvalidOperationException>(() => handle.DisposeAsync().AsTask());

            await disposeTask;

            A.CallTo(() => internalHandle1.DisposeAsync()).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Given_DisposedHandle_When_Dispose_Then_NothingHappens()
        {
            var internalHandle1 = A.Fake<IConfigurationHandle>();

            var handle = new ConfigurationHandle(new[] { internalHandle1 });
            await handle.DisposeAsync();
            await handle.DisposeAsync();

            A.CallTo(() => internalHandle1.DisposeAsync()).MustHaveHappenedOnceExactly();
        }
    }
}
