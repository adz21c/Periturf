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
using Periturf.MT.Verify;
using Periturf.Verify;
using Periturf.Verify.ComponentConditions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.MT.Verify
{
    [TestFixture]
    public class MessagePublishedConsumerTests
    {
        private Func<IMessageReceivedContext<ITestMessage>, bool> _predicate;
        private Func<IMessageReceivedContext<ITestMessage>, bool> _predicate2;
        private ConsumeContext<ITestMessage> _consumeContext;
        private IConditionInstanceTimeSpanFactory _timeSpanFactory;
        private IConditionInstanceHandler _conditionInstanceHandler;
        private VerificationEventConsumer<ITestMessage> _consumer;

        [SetUp]
        public void Setup()
        {
            _predicate = A.Fake<Func<IMessageReceivedContext<ITestMessage>, bool>>();
            _predicate2 = A.Fake<Func<IMessageReceivedContext<ITestMessage>, bool>>();

            _consumeContext = A.Fake<ConsumeContext<ITestMessage>>();
            A.CallTo(() => _consumeContext.SentTime).Returns(DateTime.Now);

            _timeSpanFactory = A.Fake<IConditionInstanceTimeSpanFactory>();
            _conditionInstanceHandler = A.Fake<IConditionInstanceHandler>();

            _consumer = new VerificationEventConsumer<ITestMessage>(
                _timeSpanFactory,
                _conditionInstanceHandler,
                new List<Func<IMessageReceivedContext<ITestMessage>, bool>> { _predicate, _predicate2 });
        }

        [Test]
        public async Task Given_NotMatchPredicate_When_Consume_Then_NothingHappens()
        {
            A.CallTo(() => _predicate.Invoke(A<IMessageReceivedContext<ITestMessage>>._)).Returns(false);
            A.CallTo(() => _predicate2.Invoke(A<IMessageReceivedContext<ITestMessage>>._)).Returns(false);

            await _consumer.Consume(_consumeContext);

            A.CallTo(() => _predicate.Invoke(A<IMessageReceivedContext<ITestMessage>>._)).MustHaveHappened();
            A.CallTo(() => _predicate2.Invoke(A<IMessageReceivedContext<ITestMessage>>._)).MustHaveHappened();
            A.CallTo(() => _conditionInstanceHandler.HandleInstanceAsync(A<ConditionInstance>._, A<CancellationToken>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Given_MatchFirstPredicate_When_Consume_Then_ConditionInstanceDispatched()
        {
            A.CallTo(() => _predicate.Invoke(A<IMessageReceivedContext<ITestMessage>>._)).Returns(true);
            A.CallTo(() => _predicate2.Invoke(A<IMessageReceivedContext<ITestMessage>>._)).Returns(false);

            await _consumer.Consume(_consumeContext);

            A.CallTo(() => _predicate.Invoke(A<IMessageReceivedContext<ITestMessage>>._)).MustHaveHappened();
            A.CallTo(() => _predicate2.Invoke(A<IMessageReceivedContext<ITestMessage>>._)).MustNotHaveHappened();
            A.CallTo(() => _conditionInstanceHandler.HandleInstanceAsync(A<ConditionInstance>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Given_MatchSecondPredicate_When_Consume_Then_ConditionInstanceDispatched()
        {
            A.CallTo(() => _predicate.Invoke(A<IMessageReceivedContext<ITestMessage>>._)).Returns(false);
            A.CallTo(() => _predicate2.Invoke(A<IMessageReceivedContext<ITestMessage>>._)).Returns(true);

            await _consumer.Consume(_consumeContext);

            A.CallTo(() => _predicate.Invoke(A<IMessageReceivedContext<ITestMessage>>._)).MustHaveHappened();
            A.CallTo(() => _predicate2.Invoke(A<IMessageReceivedContext<ITestMessage>>._)).MustHaveHappened();
            A.CallTo(() => _conditionInstanceHandler.HandleInstanceAsync(A<ConditionInstance>._, A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        public interface ITestMessage
        {
        }
    }
}
