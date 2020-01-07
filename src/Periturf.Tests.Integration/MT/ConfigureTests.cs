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
using System.Threading.Tasks;

namespace Periturf.Tests.Integration.MT
{
    [TestFixture]
    class VerifyTests
    {

        [Test]
        public async Task GiveMeAName()
        {
            bool received = false;
            var env = Environment.Setup(e =>
            {
                e.MTBus(b =>
                {
                    b.InMemoryHost();
                });
            });
            await env.StartAsync();

            await env.ConfigureAsync(c => c.MTBus(b =>
            {
                b.WhenMessagePublished<ITestMessage>(m =>
                {
                    m.Response(c =>
                    {
                        c.MTClient().Publish(new TestMessage2 { });
                        return Task.CompletedTask;
                    });
                });
                b.WhenMessagePublished<ITestMessage2>(m =>
                {
                    m.Response(c =>
                    {
                        received = true;
                        return Task.CompletedTask;
                    });
                });
            }));

            var mtClient = env.MTClient();
            await mtClient.Publish(new TestMessage { });

            await Task.Delay(1000);

            await env.StopAsync();

            Assert.That(received, Is.True);
        }
    }
}
