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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.IdSvr4.Configuration
{
    class Store : IClientStore, IResourceStore, IStore
    {
        // Concurrent friendly configuration registration
        private readonly ConcurrentDictionary<Guid, ConfigurationRegistration> _configurations = new ConcurrentDictionary<Guid, ConfigurationRegistration>();

        // locking
        private readonly ReaderWriterLockSlim _resourceManager = new ReaderWriterLockSlim();

        // In memory store implementations
        private InMemoryClientStore _clients = new InMemoryClientStore(Enumerable.Empty<Client>());
        private InMemoryResourcesStore _resources = new InMemoryResourcesStore(Enumerable.Empty<IdentityResource>(), Enumerable.Empty<ApiResource>());

        public void RegisterConfiguration(Guid id, ConfigurationRegistration config)
        {
            _configurations[id] = config;
            RebuildStores();
        }

        public void UnregisterConfiguration(Guid id)
        {
            if (_configurations.TryRemove(id, out _))
                RebuildStores();
        }

        private void RebuildStores()
        {
            _resourceManager.EnterWriteLock();
            try
            {
                var newClients = _configurations.Values.SelectMany(x => x.Clients).ToList();
                var newIdentityResources = _configurations.Values.SelectMany(x => x.IdentityResources).ToList();
                var newApiResources = _configurations.Values.SelectMany(x => x.ApiResources).ToList();

                _clients = new InMemoryClientStore(newClients);
                _resources = new InMemoryResourcesStore(newIdentityResources, newApiResources);
            }
            finally
            {
                _resourceManager.ExitWriteLock();
            }
        }

        private async Task<T> ReadLockedAsync<T>(Func<Task<T>> readFunc)
        {
            _resourceManager.EnterReadLock();
            try
            {
                return await readFunc();
            }
            finally
            {
                _resourceManager.ExitReadLock();
            }
        }

        #region Passthrough methods

        Task<Client> IClientStore.FindClientByIdAsync(string clientId)
        {
            return ReadLockedAsync(() => _clients.FindClientByIdAsync(clientId));
        }

        Task<IEnumerable<IdentityResource>> IResourceStore.FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return ReadLockedAsync(() => _resources.FindIdentityResourcesByScopeAsync(scopeNames));
        }

        Task<IEnumerable<ApiResource>> IResourceStore.FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return ReadLockedAsync(() => _resources.FindApiResourcesByScopeAsync(scopeNames));
        }

        Task<ApiResource> IResourceStore.FindApiResourceAsync(string name)
        {
            return ReadLockedAsync(() => _resources.FindApiResourceAsync(name));
        }

        Task<Resources> IResourceStore.GetAllResourcesAsync()
        {
            return ReadLockedAsync(() => _resources.GetAllResourcesAsync());
        }

        #endregion
    }
}
