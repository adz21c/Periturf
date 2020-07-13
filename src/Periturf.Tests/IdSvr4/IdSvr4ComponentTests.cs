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
using IdentityServer4.Models;
using IdentityServer4.Stores;
using NUnit.Framework;
using Periturf.Events;
using Periturf.IdSvr4;
using Periturf.IdSvr4.Clients;
using Periturf.IdSvr4.Configuration;
using Periturf.IdSvr4.Verify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Periturf.Tests.IdSvr4
{
    [TestFixture]
    class IdSvr4ComponentTests
    {
        [Test]
        public void Given_Component_When_CreateClient_Then_ClientInstanceReturned()
        {
            // Arrange
            var store = A.Fake<IStore>();
            var sink = A.Fake<IEventMonitorSink>();
            var client = A.Fake<IIdSvr4Client>();

            var component = new IdSvr4Component(store, sink, client);

            // Act
            var resultClient = component.CreateClient();

            Assert.That(resultClient, Is.Not.Null);
            Assert.That(resultClient, Is.SameAs(client));
        }

        [Test]
        public void Given_Component_When_CreateConfigurationSpecification_Then_SpecInstanceReturned()
        {
            // Arrange
            var store = A.Fake<IStore>();
            var sink = A.Fake<IEventMonitorSink>();
            var client = A.Fake<IIdSvr4Client>();

            var component = new IdSvr4Component(store, sink, client);

            var factory = A.Fake<IEventHandlerFactory>();

            // Act
            var spec = component.CreateConfigurationSpecification<IdSvr4Specification>(factory);

            Assert.That(spec, Is.Not.Null);
        }

        [Test]
        public void Given_Component_When_CreateConditionBuilder_Then_ConditionBuilderInstanceReturned()
        {
            // Arrange
            var store = A.Fake<IStore>();
            var sink = A.Fake<IEventMonitorSink>();
            var client = A.Fake<IIdSvr4Client>();

            var component = new IdSvr4Component(store, sink, client);

            // Act
            var builder = component.CreateConditionBuilder<IdSvr4ConditionBuilder>();

            Assert.That(builder, Is.Not.Null);
        }
    }
}
