/*
 *     Copyright 2020 Adam Burton (adz21c@gmail.com)
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
using Periturf.Events;
using System;
using System.Collections.Generic;

namespace Periturf.Tests.Events
{
    [TestFixture]
    class EventSpecificationTests
    {
        [Test]
        public void Given_Null_When_Ctor_Then_Exception()
        {
            Assert.That(() => new TestEventSpecification(null), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("eventHandlerFactory"));
        }

        [Test]
        public void Given_Null_When_AddHandlerSpec_Then_Exception()
        {
            var eventHandlerFactory = A.Fake<IEventHandlerFactory>();

            var spec = new TestEventSpecification(eventHandlerFactory);

            Assert.That(() => spec.AddHandlerSpecification(null), Throws.ArgumentNullException.With.Property("ParamName").EqualTo("spec"));
        }

        [Test]
        public void Given_HandlerSpec_When_AddHandlerSpec_Then_AddedToEventSpec()
        {
            var eventHandlerFactory = A.Fake<IEventHandlerFactory>();

            var spec = new TestEventSpecification(eventHandlerFactory);

            var handlerSpec = A.Dummy<IEventHandlerSpecification<Object>>();

            spec.AddHandlerSpecification(handlerSpec);

            Assert.That(spec.HandlerSpecifications, Does.Contain(handlerSpec));
        }

        [Test]
        public void Given_EventSpec_When_CreateHandler_Then_CallsHandlerFactory()
        {
            var eventHandler = A.Fake<IEventHandler<Object>>();

            var eventHandlerFactory = A.Fake<IEventHandlerFactory>();
            A.CallTo(() => eventHandlerFactory.Create<Object>(A<List<IEventHandlerSpecification<Object>>>._)).Returns(eventHandler);

            var spec = new TestEventSpecification(eventHandlerFactory);
            var handlerSpec = A.Dummy<IEventHandlerSpecification<Object>>();
            spec.AddHandlerSpecification(handlerSpec);

            var handler = spec.TestBuild();

            Assert.That(handler, Is.SameAs(eventHandler));
            A.CallTo(() => eventHandlerFactory.Create<Object>(A<List<IEventHandlerSpecification<Object>>>.That.NullCheckedMatches(
                h => h.Contains(handlerSpec),
                w => w.Write("HandlerSpecList")))).MustHaveHappened();
        }

        class TestEventSpecification : EventSpecification<Object>
        {
            public TestEventSpecification(IEventHandlerFactory eventHandlerFactory) : base(eventHandlerFactory)
            { }

            public IEventHandler<Object> TestBuild()
            {
                return CreateHandler();
            }
        }
    }
}
