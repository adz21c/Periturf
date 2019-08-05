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
using IdentityServer4.Events;
using NUnit.Framework;
using Periturf.IdSvr4.Verify;
using Periturf.Verify;
using System;
using System.Threading.Tasks;

namespace Periturf.Tests.IdSvr4.Verify
{
    [TestFixture]
    class EventOccurredConditionEvaluatorTests
    {
        [Test]
        public async Task Given_NoChecks_When_Evaluate_Then_ResultFalse()
        {
            var condition = A.Dummy<Func<Event, bool>>();

            var evaluator = new EventOccurredConditionEvaluator<Event>(condition);

            // Act
            var result = await evaluator.EvaluateAsync();

            // Assert
            Assert.IsFalse(result);
            A.CallTo(() => condition.Invoke(A<Event>._)).MustNotHaveHappened();
        }

        [Test]
        public async Task Given_FalseChecks_When_Evaluate_Then_ResultFalse()
        {
            var @event = A.Dummy<Event>();

            var condition = A.Dummy<Func<Event, bool>>();
            A.CallTo(() => condition.Invoke(@event)).Returns(false);

            var evaluator = new EventOccurredConditionEvaluator<Event>(condition);

            // Act
            evaluator.CheckEvent(@event);
            var result = await evaluator.EvaluateAsync();

            // Assert
            Assert.IsFalse(result);
            A.CallTo(() => condition.Invoke(@event)).MustHaveHappened();
        }


        [Test]
        public async Task Given_TrueChecks_When_Evaluate_Then_ResultTrue()
        {
            var @event = A.Dummy<Event>();

            var condition = A.Dummy<Func<Event, bool>>();
            A.CallTo(() => condition.Invoke(@event)).Returns(true);

            var evaluator = new EventOccurredConditionEvaluator<Event>(condition);

            // Act
            evaluator.CheckEvent(@event);
            var result = await evaluator.EvaluateAsync();

            // Assert
            Assert.IsTrue(result);
            A.CallTo(() => condition.Invoke(@event)).MustHaveHappened();
        }

        [Test]
        public async Task Given_FalseAndTrueChecks_When_Evaluate_Then_ResultTrue()
        {
            var @event1 = A.Dummy<Event>();
            var @event2 = A.Dummy<Event>();

            var condition = A.Dummy<Func<Event, bool>>();
            A.CallTo(() => condition.Invoke(@event1)).Returns(false);
            A.CallTo(() => condition.Invoke(@event2)).Returns(true);

            var evaluator = new EventOccurredConditionEvaluator<Event>(condition);

            // Act
            evaluator.CheckEvent(@event1);
            evaluator.CheckEvent(@event2);
            var result = await evaluator.EvaluateAsync();

            // Assert
            Assert.IsTrue(result);
            A.CallTo(() => condition.Invoke(@event1)).MustHaveHappened();
            A.CallTo(() => condition.Invoke(@event2)).MustHaveHappened();
        }

        [Test]
        public async Task Given_TrueAndFalseChecks_When_Evaluate_Then_ResultTrue()
        {
            var @event1 = A.Dummy<Event>();
            var @event2 = A.Dummy<Event>();

            var condition = A.Dummy<Func<Event, bool>>();
            A.CallTo(() => condition.Invoke(@event1)).Returns(true);
            A.CallTo(() => condition.Invoke(@event2)).Returns(false);

            var evaluator = new EventOccurredConditionEvaluator<Event>(condition);

            // Act
            evaluator.CheckEvent(@event1);
            evaluator.CheckEvent(@event2);
            var result = await evaluator.EvaluateAsync();

            // Assert
            Assert.IsTrue(result);
            A.CallTo(() => condition.Invoke(@event1)).MustHaveHappened();
            A.CallTo(() => condition.Invoke(@event2)).MustHaveHappened();
        }
    }
}
