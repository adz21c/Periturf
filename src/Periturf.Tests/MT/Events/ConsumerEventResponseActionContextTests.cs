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
using Periturf.Clients;
using Periturf.Events;
using Periturf.MT.Events;
using System;

namespace Periturf.Tests.MT.Events
{
    [TestFixture]
    class ConsumerEventResponseActionContextTests
    {
        private const string interceptComponent = "MyComponent";
        private const string otherComponent = "OtherComponent";
        private readonly IMessageReceivedContext<object> _data = A.Fake<IMessageReceivedContext<object>>();
        private readonly ConsumeContext _consumeContext = A.Fake<ConsumeContext>();
        private readonly IComponentClient _otherClient = A.Dummy<IComponentClient>();
        private readonly IEventResponseContext<IMessageReceivedContext<object>> _inner = A.Fake<IEventResponseContext<IMessageReceivedContext<Object>>>();
        private readonly ConsumerEventResponseContext<object> _sut;

        public ConsumerEventResponseActionContextTests()
        {
            _sut = new ConsumerEventResponseContext<Object>(
                interceptComponent,
                _consumeContext,
                _inner);
        }

        [SetUp]
        public void SetUp()
        {
            A.CallTo(() => _inner.Data).Returns(_data);
            A.CallTo(() => _inner.CreateComponentClient(otherComponent)).Returns(_otherClient);
        }

        [TearDown]
        public void TearDown()
        {
            Fake.ClearConfiguration(_consumeContext);
            Fake.ClearConfiguration(_inner);
            Fake.ClearRecordedCalls(_consumeContext);
            Fake.ClearRecordedCalls(_inner);
        }

        [Test]
        public void Given_otherComponent_When_CreateComponentClient_Then_InterceptorIgnored()
        {
            var result = _sut.CreateComponentClient(otherComponent);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(_otherClient));
            A.CallTo(() => _inner.CreateComponentClient(interceptComponent)).MustNotHaveHappened();
            A.CallTo(() => _inner.CreateComponentClient(otherComponent)).MustHaveHappened();
        }

        [Test]
        public void Given_myComponent_When_CreateComponentClient_Then_InterceptorCalled()
        {
            var result = _sut.CreateComponentClient(interceptComponent);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Not.SameAs(_otherClient));
            A.CallTo(() => _inner.CreateComponentClient(interceptComponent)).MustNotHaveHappened();
            A.CallTo(() => _inner.CreateComponentClient(otherComponent)).MustNotHaveHappened();
        }

        [Test]
        public void Given_Context_When_Data_Then_GetsInner()
        {
            var result = _sut.Data;

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(_data));
            A.CallTo(() => _inner.Data).MustHaveHappened();
        }
    }
}
