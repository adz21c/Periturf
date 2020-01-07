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
using IdentityServer4.Stores;
using NUnit.Framework;
using Periturf.IdSvr4;
using Periturf.IdSvr4.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Periturf.Tests.IdSvr4
{
    [TestFixture]
    class StoreTests
    {
        [Test]
        public async Task Given_Configuration_When_QueryClientAndResourceStores_Then_ReceiveExpectedResults()
        {
            // Arrange
            const string enabledClientId = "Client1";
            const string disabledClientId = "Client2";
            const string fakeClientId = "FakeClient";
            const string apiResource = "API";
            const string fakeApiResource = "Fake_API";
            const string apiScope = "API_Scope";
            const string fakeApiScope = "Fake_API_Scope";
            const string idResource = "ID";
            const string fakeIdResource = "FakeID";

            var config = new ConfigurationRegistration(
                new List<Client>
                {
                    new Client { ClientId = enabledClientId, Enabled = true },
                    new Client { ClientId = disabledClientId, Enabled = false }
                },
                new List<IdentityResource>
                {
                    new IdentityResource { Name = idResource }
                },
                new List<ApiResource>
                {
                    new ApiResource(apiResource)
                    {
                        Scopes = new List<Scope>
                        {
                            new Scope(apiScope)
                        }
                    }
                });

            var store = new Store();
            store.RegisterConfiguration(config);

            var clientStore = (IClientStore)store;
            var resourceStore = (IResourceStore)store;

            // Act & Assert
            Assert.That(await clientStore.FindClientByIdAsync(enabledClientId), Is.Not.Null);
            Assert.That(await clientStore.FindClientByIdAsync(disabledClientId), Is.Not.Null);
            Assert.That(await clientStore.FindClientByIdAsync(fakeClientId), Is.Null);
            Assert.That(await clientStore.FindEnabledClientByIdAsync(enabledClientId), Is.Not.Null);
            Assert.That(await clientStore.FindEnabledClientByIdAsync(disabledClientId), Is.Null);
            Assert.That(await clientStore.FindEnabledClientByIdAsync(fakeClientId), Is.Null);

            Assert.That(await resourceStore.FindApiResourceAsync(apiResource), Is.Not.Null);
            Assert.That(await resourceStore.FindApiResourceAsync(fakeApiResource), Is.Null);
            Assert.That(await resourceStore.FindApiResourcesByScopeAsync(new[] { apiScope }), Is.Not.Empty);
            Assert.That(await resourceStore.FindApiResourcesByScopeAsync(new[] { fakeApiScope }), Is.Empty);
            Assert.That(await resourceStore.FindIdentityResourcesByScopeAsync(new[] { idResource }), Is.Not.Empty);
            Assert.That(await resourceStore.FindIdentityResourcesByScopeAsync(new[] { fakeIdResource }), Is.Empty);

            var resources = await resourceStore.GetAllResourcesAsync();
            Assert.That(resources, Is.Not.Null);
            Assert.That(resources.IdentityResources, Is.Not.Empty);
            Assert.That(resources.ApiResources, Is.Not.Empty);
        }

        [Test]
        public async Task Given_RegisteredConfiguration_When_UnregisterConfiguration_Then_ConfigurationRemoved()
        {
            // Arrange
            const string enabledClientId = "Client1";
            const string apiResource = "API";
            const string apiScope = "API_Scope";
            const string idResource = "ID";

            var config = new ConfigurationRegistration(
                new List<Client>
                {
                    new Client { ClientId = enabledClientId, Enabled = true }
                },
                new List<IdentityResource>
                {
                    new IdentityResource { Name = idResource }
                },
                new List<ApiResource>
                {
                    new ApiResource(apiResource)
                    {
                        Scopes = new List<Scope> { new Scope(apiScope) }
                    }
                });

            var store = new Store();
            var configHandle = store.RegisterConfiguration(config);

            var clientStore = (IClientStore)store;
            var resourceStore = (IResourceStore)store;

            Assume.That((await clientStore.FindClientByIdAsync(enabledClientId)) != null);
            Assume.That((await resourceStore.FindApiResourceAsync(apiResource)) != null);
            Assume.That((await resourceStore.FindApiResourcesByScopeAsync(new[] { apiScope })).Any());
            Assume.That((await resourceStore.FindIdentityResourcesByScopeAsync(new[] { idResource })).Any());

            // Act
            Assert.DoesNotThrowAsync(() => configHandle.DisposeAsync().AsTask());

            // Assert
            Assert.That(await clientStore.FindClientByIdAsync(enabledClientId), Is.Null);
            Assert.That(await resourceStore.FindApiResourceAsync(apiResource), Is.Null);
            Assert.That(await resourceStore.FindApiResourcesByScopeAsync(new[] { apiScope }), Is.Empty);
            Assert.That(await resourceStore.FindIdentityResourcesByScopeAsync(new[] { idResource }), Is.Empty);
        }
    }
}
