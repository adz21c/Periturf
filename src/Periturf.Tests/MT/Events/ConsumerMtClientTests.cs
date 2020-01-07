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
using Periturf.MT.Events;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.MT.Events
{
    [TestFixture]
    class ConsumerMtClientTests
    {
        [Test]
        public async Task Given_Client_When_Publish_Then_MessagePublished()
        {
            var consumeContext = A.Fake<ConsumeContext>();
            var message = new object();
            await new ConsumerMtClient(consumeContext).Publish(message);

            A.CallTo(() => consumeContext.Publish(message, A<CancellationToken>._)).MustHaveHappened();
        }
    }
}
