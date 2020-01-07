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
using IdentityServer4.Models;
using NUnit.Framework;
using Periturf.IdSvr4.Configuration;

namespace Periturf.Tests.IdSvr4.Configuration
{
    [TestFixture]
    class IdSvr4ConfiguratorTests
    {
        class Configurator : IIdSvr4Configurator
        {
            public Client ClientObj { get; private set; }
            public bool ClientCalled { get; private set; }
            public void Client(Client client)
            {
                ClientCalled = true;
                ClientObj = client;
            }

            public IdentityResource IdentityResourceObj { get; private set; }
            public bool IdentityResourceCalled { get; private set; }
            public void IdentityResource(IdentityResource resource)
            {
                IdentityResourceCalled = true;
                IdentityResourceObj = resource;
            }

            public ApiResource ApiResourceObj { get; private set; }
            public bool ApiResourceCalled { get; private set; }
            public void ApiResource(ApiResource resource)
            {
                ApiResourceCalled = true;
                ApiResourceObj = resource;
            }
        }

        [Test]
        public void Given_Configurator_When_Client_Then_ClientCreatedAndConfigured()
        {
            const string clientId = "ClientID";
            var testConfigurator = new Configurator();
            var configurator = (IIdSvr4Configurator)testConfigurator;
            
            configurator.Client(c => { c.ClientId = clientId; });

            Assert.That(testConfigurator.ClientCalled, Is.True);
            Assert.That(testConfigurator.ClientObj, Is.Not.Null);
            Assert.That(testConfigurator.ClientObj.ClientId, Is.EqualTo(clientId));
        }

        [Test]
        public void Given_Configurator_When_IdResource_Then_IdResourceCreatedAndConfigured()
        {
            const string resourceId = "ResourceID";
            var testConfigurator = new Configurator();
            var configurator = (IIdSvr4Configurator)testConfigurator;

            configurator.IdentityResource(c => { c.Name = resourceId; });

            Assert.That(testConfigurator.IdentityResourceCalled, Is.True);
            Assert.That(testConfigurator.IdentityResourceObj, Is.Not.Null);
            Assert.That(testConfigurator.IdentityResourceObj.Name, Is.EqualTo(resourceId));
        }

        [Test]
        public void Given_Configurator_When_ApiResource_Then_ApiResourceCreatedAndConfigured()
        {
            const string resourceId = "ResourceID";
            var testConfigurator = new Configurator();
            var configurator = (IIdSvr4Configurator)testConfigurator;

            configurator.ApiResource(c => { c.Name = resourceId; });

            Assert.That(testConfigurator.ApiResourceCalled, Is.True);
            Assert.That(testConfigurator.ApiResourceObj, Is.Not.Null);
            Assert.That(testConfigurator.ApiResourceObj.Name, Is.EqualTo(resourceId));
        }
    }
}
