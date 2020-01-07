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
using Periturf.Events;
using Periturf.MT.Configuration;
using Periturf.MT.Events;
using System;
using System.Threading.Tasks;

namespace Periturf.Tests.MT.Configuration
{
    [TestFixture]
    class MessageReceivedSpecificationTests
    {
        private const string componentName = "ComponentName";
        
        [Test]
        public void Given_GenericType_When_MessageType_Then_GenericTypeMatches()
        {
            var spec = (IWhenMessagePublishedSpecification) new WhenMessagePublishedSpecification<ITestMessage>(componentName);
            Assert.That(spec.MessageType, Is.EqualTo(typeof(ITestMessage)));
        }

        [Test]
        public void Given_NullConfigurator_When_Apply_Then_Throws()
        {
            var spec = new WhenMessagePublishedSpecification<ITestMessage>(componentName);
            var ispec = (IWhenMessagePublishedSpecification)spec;
            var configurator = (IWhenMessagePublishedConfigurator<ITestMessage>)spec;
            var action = A.Dummy<Func<IEventResponseContext<IMessageReceivedContext<ITestMessage>>, Task>>();
            var factory = A.Dummy<IEventResponseContextFactory>();

            configurator.Response(action);

            var ex = Assert.Throws<ArgumentNullException>(() => ispec.Configure(null, factory));
            Assert.That(ex.ParamName, Is.EqualTo("configurator"));
        }

        [Test]
        public void Given_NullFactory_When_Apply_Then_Throws()
        {
            var spec = new WhenMessagePublishedSpecification<ITestMessage>(componentName);
            var ispec = (IWhenMessagePublishedSpecification)spec;
            var configurator = (IWhenMessagePublishedConfigurator<ITestMessage>)spec;
            var action = A.Dummy<Func<IEventResponseContext<IMessageReceivedContext<ITestMessage>>, Task>>();
            var receiveEndpointConfigurator = A.Fake<IReceiveEndpointConfigurator>();

            configurator.Response(action);

            var ex = Assert.Throws<ArgumentNullException>(() => ispec.Configure(receiveEndpointConfigurator, null));
            Assert.That(ex.ParamName, Is.EqualTo("eventResponseContextFactory"));
        }

        [Test]
        public void Given_Spec_When_Apply_Then_ConfiguresConsumer()
        {
            var spec = new WhenMessagePublishedSpecification<ITestMessage>(componentName);
            var ispec = (IWhenMessagePublishedSpecification)spec;
            
            var action = A.Dummy<Func<IEventResponseContext<IMessageReceivedContext<ITestMessage>>, Task>>();

            var factory = A.Dummy<IEventResponseContextFactory>();

            var configurator = (IWhenMessagePublishedConfigurator<ITestMessage>)spec;
            configurator.Response(action);

            var receiveEndpointConfigurator = A.Fake<IReceiveEndpointConfigurator>();

            Assert.DoesNotThrow(() => ispec.Configure(receiveEndpointConfigurator, factory));

            // TODO: A.CallTo(() => receiveEndpointConfigurator.cons)
        }

        public interface ITestMessage
        { }
    }
}
