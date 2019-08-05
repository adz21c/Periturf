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
using Periturf.Components;
using Periturf.IdSvr4;
using System;
using System.Linq;

namespace Periturf.Tests.IdSvr4.Configuration
{
    [TestFixture]
    class ConfigurationBuilderExtensionsTests
    {
        [Test]
        public void Given_ConfigurationBuilder_When_ConfigureIdSvr4_Then_IdSvr4ComponentAdded()
        {
            // Arrange
            var config = A.Dummy<Action<IdSvr4Configurator>>();
            var builder = A.Fake<IConfiugrationBuilder>();
            const string componentName = "IdSvr4";
            A.CallTo(() => builder.AddComponentConfigurator(componentName, A<Func<IdSvr4Component, IComponentConfigurator>>._))
                .Invokes(x => x.GetArgument<Func<IdSvr4Component, IComponentConfigurator>>(1).Invoke(null));


            // Act
            builder.IdSvr4(componentName, config);

            // Assert
            A.CallTo(() => builder.AddComponentConfigurator(componentName, A<Func<IdSvr4Component, IComponentConfigurator>>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => config.Invoke(A<IdSvr4Configurator>._)).MustHaveHappenedOnceExactly();
        }


        [Test]
        public void Given_IdSvr4Configurator_When_ConfigureOptions_Then_InformationAdded()
        {
            // Arrange
            var configurator = new IdSvr4Configurator();
            const string clientId1 = "clientId1";
            const string clientId2 = "clientId2";
            const string redirectUri = "http://localhost/Redirect";
            const string redirectUri2 = "http://localhost/Redirect2";
            const string scope = "Scope";
            const string scope2 = "Scope2";
            const string secret = "Secret";
            const string secret2 = "Secret2";
            const string idResource1Name = "idResource1";
            const string idResource2Name = "idResource2";
            const string apiResource1Name = "apiResource1";
            const string apiResource2Name = "apiResource2";
            const string claim = "Claim";
            const string claim2 = "Claim2";

            // Act
            configurator.Client(c =>
            {
                c.ClientId = clientId1;
                c.RedirectUri(redirectUri);
                c.RedirectUri(redirectUri2);
                c.Scope(scope);
                c.Scope(scope2);
                c.Secret(s => { s.Value = secret; });
                c.Secret(secret2);
            });
            configurator.Client(c => c.ClientId = clientId2);
            configurator.IdentityResource(i =>
            {
                i.Name = idResource1Name;
                i.UserClaim(claim);
                i.UserClaim(claim2);
            });
            configurator.IdentityResource(i => i.Name = idResource2Name);
            configurator.ApiResource(a =>
            {
                a.Name = apiResource1Name;
                a.UserClaim(claim);
                a.UserClaim(claim2);
                a.Scope(s =>
                {
                    s.Name = scope;
                    s.UserClaim(claim);
                    s.UserClaim(claim2);
                });
                a.Scope(s => { s.Name = scope2; });
                a.Secret(s => { s.Value = secret; });
                a.Secret(secret2);
            });
            configurator.ApiResource(a => a.Name = apiResource2Name);

            var registration = configurator.Build();

            // Assert
            Assert.AreEqual(2, registration.Clients.Count);
            
            // Client1
            var client1 = registration.Clients.SingleOrDefault(x => x.ClientId == clientId1);
            Assert.NotNull(client1);

            Assert.IsNotEmpty(client1.RedirectUris);
            Assert.AreEqual(2, client1.RedirectUris.Count);
            Assert.That(client1.RedirectUris.Contains(redirectUri));
            Assert.That(client1.RedirectUris.Contains(redirectUri2));

            Assert.IsNotEmpty(client1.AllowedScopes);
            Assert.AreEqual(2, client1.AllowedScopes.Count);
            Assert.That(client1.AllowedScopes.Contains(scope));
            Assert.That(client1.AllowedScopes.Contains(scope2));

            Assert.IsNotEmpty(client1.ClientSecrets);
            Assert.AreEqual(2, client1.ClientSecrets.Count);
            Assert.That(client1.ClientSecrets.Any(x => x.Value == secret));
            Assert.That(client1.ClientSecrets.Any(x => x.Value == secret2));
            
            // Client 2
            Assert.NotNull(registration.Clients.SingleOrDefault(x => x.ClientId == clientId2));

            Assert.AreEqual(2, registration.IdentityResources.Count);

            // Id Resource 1
            var idResource1 = registration.IdentityResources.SingleOrDefault(x => x.Name == idResource1Name);
            Assert.NotNull(idResource1);

            Assert.IsNotEmpty(idResource1.UserClaims);
            Assert.AreEqual(2, idResource1.UserClaims.Count);
            Assert.That(idResource1.UserClaims.Contains(claim));
            Assert.That(idResource1.UserClaims.Contains(claim2));

            // Id Resource 2
            Assert.NotNull(registration.IdentityResources.SingleOrDefault(x => x.Name == idResource2Name));

            Assert.AreEqual(2, registration.ApiResources.Count);

            // API Resource 1
            var apiResource1 = registration.ApiResources.SingleOrDefault(x => x.Name == apiResource1Name);
            Assert.NotNull(apiResource1);

            Assert.IsNotEmpty(apiResource1.ApiSecrets);
            Assert.AreEqual(2, apiResource1.ApiSecrets.Count);
            Assert.That(apiResource1.ApiSecrets.Any(x => x.Value == secret));
            Assert.That(apiResource1.ApiSecrets.Any(x => x.Value == secret2));

            Assert.IsNotEmpty(apiResource1.Scopes);
            Assert.AreEqual(2, apiResource1.Scopes.Count);
            Assert.That(apiResource1.Scopes.Any(x => x.Name == scope));
            Assert.That(apiResource1.Scopes.Any(x => x.Name == scope2));

            Assert.IsNotEmpty(apiResource1.UserClaims);
            Assert.AreEqual(2, apiResource1.UserClaims.Count);
            Assert.That(apiResource1.UserClaims.Contains(claim));
            Assert.That(apiResource1.UserClaims.Contains(claim2));

            // API Resource 1 Scope
            var apiResource1Scope = apiResource1.Scopes.Single(x => x.Name == scope);
            Assert.AreEqual(2, apiResource1Scope.UserClaims.Count);
            Assert.That(apiResource1Scope.UserClaims.Contains(claim));
            Assert.That(apiResource1Scope.UserClaims.Contains(claim2));

            // API Resource 2
            Assert.NotNull(registration.ApiResources.SingleOrDefault(x => x.Name == apiResource2Name));
        }
    }
}
