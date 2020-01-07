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
using NUnit.Framework;
using Periturf.Events;
using Periturf.MT;
using Periturf.MT.Setup;
using System;

namespace Periturf.Tests.MT.Setup
{
    [TestFixture]
    class BusSpecificationTests
    {
        [Test]
        public void Given_Null_When_SetBusManager_Then_Throws()
        {
            var factory = A.Fake<IEventResponseContextFactory>();
            var spec = new BusSpecification(A.Dummy<string>(), factory);
            var ex = Assert.Throws<ArgumentNullException>(() => spec.SetBusManager(null));
            Assert.That(ex.ParamName, Is.EqualTo("busManager"));
        }
        
        [Test]
        public void Given_Configured_When_Build_Then_HostCreated()
        {
            const string componentName = "Component1";
            
            var busManager = A.Fake<IBusManager>();
            var factory = A.Fake<IEventResponseContextFactory>();

            var spec = new BusSpecification(componentName, factory);
            
            spec.SetBusManager(busManager);

            var host = spec.Build();

            Assert.That(host, Is.Not.Null);
            Assert.That(host.Components, Does.ContainKey(componentName));
            A.CallTo(() => busManager.Setup(spec, factory)).MustHaveHappened();
        }

        [Test]
        public void Given_NotConfigured_When_Build_Then_Throws()
        {
            var factory = A.Fake<IEventResponseContextFactory>();
            var spec = new BusSpecification("Host", factory);
            var ex = Assert.Throws<InvalidOperationException>(() => spec.Build());
            Assert.That(ex.Message, Is.EqualTo("Specification not configured"));
        }
    }
}
