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
using Microsoft.Extensions.Hosting;
using NUnit.Framework;
using Periturf.Hosting.Setup;

namespace Periturf.Tests.Hosting
{
    [TestFixture]
    class GenericHostSpecificationTests
    {
        [Test]
        public void Given_MultipleComponents_When_Host_Then_NewHostRegisteredWithComponents()
        {
            var componentName = "Component";
            var component = A.Dummy<Components.IComponent>();
            var componentSpecification = A.Dummy<IGenericHostComponentSpecification>();
            A.CallTo(() => componentSpecification.Apply(A<IHostBuilder>._)).Returns(component);
            A.CallTo(() => componentSpecification.Name).Returns(componentName);

            var spec = new GenericHostSpecification();
            spec.AddComponentSpecification(componentSpecification);
            
            var host = spec.Build();

            Assert.That(host, Is.Not.Null);
            Assert.That(host.Components, Does.ContainKey(componentName));
            Assert.That(host.Components, Does.ContainValue(component));
        }

        [Test]
        public void Given_Host_When_StartAndStop_Then_TheHostStartsAndStops()
        {
            var env = Environment.Setup(s =>
            {
                s.GenericHost(c => { });
            });

            // Act
            Assert.DoesNotThrowAsync(() => env.StartAsync());
            Assert.DoesNotThrowAsync(() => env.StopAsync());
        }
    }
}
