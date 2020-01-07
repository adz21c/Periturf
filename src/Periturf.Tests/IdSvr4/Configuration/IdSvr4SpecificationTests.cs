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
using NUnit.Framework;
using Periturf.Configuration;
using Periturf.IdSvr4;
using Periturf.IdSvr4.Configuration;

namespace Periturf.Tests.IdSvr4.Configuration
{
    [TestFixture]
    class IdSvr4SpecificationTests
    {
        [Test]
        public void Given_IdSvr4Specification_When_ConfigureOptions_Then_InformationAdded()
        {
            // Arrange
            ConfigurationRegistration registration = null;
            var store = A.Fake<IStore>();
            A.CallTo(() => store.RegisterConfiguration(A<ConfigurationRegistration>._)).Invokes((ConfigurationRegistration c) => registration = c);
            
            var spec = (IConfigurationSpecification)new IdSvr4Specification(store);
            var configurator = (IIdSvr4Configurator)spec;

            var client1 = new Client { ClientId = "clientId1" };
            var client2 = new Client { ClientId = "clientId2" };

            var idResource1 = new IdentityResource { Name = "idResource1" };
            var idResource2 = new IdentityResource { Name = "idResource2" };

            var apiResource1 = new ApiResource("apiResource1");
            var apiResource2 = new ApiResource("apiResource2");

            // Act
            configurator.Client(client1);
            configurator.Client(client2);
            configurator.IdentityResource(idResource1);
            configurator.IdentityResource(idResource2);
            configurator.ApiResource(apiResource1);
            configurator.ApiResource(apiResource2);

            var handle = spec.ApplyAsync();

            // Assert
            Assert.That(handle, Is.Not.Null);
            Assert.That(registration, Is.Not.Null);

            // Client
            Assert.That(registration.Clients.Count, Is.EqualTo(2));
            Assert.That(registration.Clients, Does.Contain(client1));
            Assert.That(registration.Clients, Does.Contain(client2));

            // Id Resource
            Assert.That(registration.IdentityResources.Count, Is.EqualTo(2));
            Assert.That(registration.IdentityResources, Does.Contain(idResource1));
            Assert.That(registration.IdentityResources, Does.Contain(idResource2));

            // API Resource 1
            Assert.That(registration.ApiResources.Count, Is.EqualTo(2));
            Assert.That(registration.ApiResources, Does.Contain(apiResource1));
            Assert.That(registration.ApiResources, Does.Contain(apiResource2));
        }
    }
}
