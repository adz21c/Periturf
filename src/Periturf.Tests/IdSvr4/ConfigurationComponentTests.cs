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
using IdentityServer4.Stores;
using NUnit.Framework;
using Periturf.Components;
using Periturf.IdSvr4;
using Periturf.IdSvr4.Configuration;
using Periturf.IdSvr4.Verify;
using System;
using System.Threading.Tasks;

namespace Periturf.Tests.IdSvr4
{
    [TestFixture]
    class ConfigurationComponentTests
    {
        [Test]
        public async Task Given_MultipleConfigurations_When_RegisterConfiguration_Then_RegistersOnlyThatConfiguratorsConfig()
        {
            // Arrange
            const string componentName = "IdSvr4";
            const string clientId1 = "ClientID1";
            const string clientId2 = "ClientID2";

            var store = new Store();
            var component = new IdSvr4Component(store, A.Dummy<IEventMonitorSink>());
            var clientStore = (IClientStore)store;
            IComponentConfigurator configurator1 = null;
            IComponentConfigurator configurator2 = null;

            var builder = A.Fake<IConfiugrationBuilder>();
            A.CallTo(() => builder.AddComponentConfigurator(componentName, A<Func<IdSvr4Component, IComponentConfigurator>>._))
                .Invokes((string name, Func<IdSvr4Component, IComponentConfigurator> config) =>
                {
                    var configurator = config(component);

                    if (configurator1 == null)
                        configurator1 = configurator;
                    else if (configurator2 == null)
                        configurator2 = configurator;
                });


            builder.IdSvr4(componentName, x =>
            {
                x.Client(c =>
                {
                    c.ClientId = clientId1;
                });
            });

            builder.IdSvr4(componentName, x =>
            {
                x.Client(c =>
                {
                    c.ClientId = clientId2;
                });
            });

            // Act & Assert
            Assert.IsNull(await clientStore.FindClientByIdAsync(clientId1));
            Assert.IsNull(await clientStore.FindClientByIdAsync(clientId2));

            await configurator1.RegisterConfigurationAsync(Guid.NewGuid());

            Assert.IsNotNull(await clientStore.FindClientByIdAsync(clientId1));
            Assert.IsNull(await clientStore.FindClientByIdAsync(clientId2));

            await configurator2.RegisterConfigurationAsync(Guid.NewGuid());

            Assert.IsNotNull(await clientStore.FindClientByIdAsync(clientId1));
            Assert.IsNotNull(await clientStore.FindClientByIdAsync(clientId2));
        }
    }
}
