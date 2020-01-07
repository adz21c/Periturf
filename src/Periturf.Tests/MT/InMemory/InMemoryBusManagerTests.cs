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
using Periturf.MT.InMemory;
using Periturf.MT.Verify;
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Periturf.Tests.MT.InMemory
{
    [TestFixture]
    class InMemoryBusManagerTests
    {
        private InMemoryBusManager _sut;
        private IMtSpecification _setupSpec;

        [SetUp]
        public void SetUp()
        {
            var factory = A.Fake<IEventResponseContextFactory>();

            _setupSpec = A.Fake<IMtSpecification>();
            _sut = new InMemoryBusManager();
            _sut.Setup(_setupSpec, factory);
        }

        [Test]
        public void Given_BusManager_When_Setup_Then_BusCreated()
        {
            Assert.That(_sut.BusControl, Is.Not.Null);
            A.CallTo(() => _setupSpec.WhenMessagePublishedSpecifications).MustHaveHappened();
        }

        [Test]
        public async Task Given_BusManager_When_ApplyConfiguration_Then_ConfigurationApplied()
        {
            var messageSpec = A.Fake<IWhenMessagePublishedSpecification>();
            var spec = A.Fake<IMtSpecification>();
            var factory = A.Fake<IEventResponseContextFactory>();
            A.CallTo(() => spec.WhenMessagePublishedSpecifications).Returns(new List<IWhenMessagePublishedSpecification> { messageSpec });

            var handle = await _sut.ApplyConfigurationAsync(spec, factory);

            Assert.That(handle, Is.Not.Null);
            A.CallTo(() => spec.WhenMessagePublishedSpecifications).MustHaveHappened();
            A.CallTo(() => messageSpec.Configure(A<IReceiveEndpointConfigurator>._, A<IEventResponseContextFactory>._)).MustHaveHappened();
        }

        [Test]
        public async Task Given_BusManager_When_ApplyVerification_Then_VerificationInitialised()
        {
            var now = DateTime.Now;
            var timeSpanFactory = A.Fake<IConditionInstanceTimeSpanFactory>();
            A.CallTo(() => timeSpanFactory.Create(A<DateTime>._)).ReturnsLazily((DateTime dt) => dt - now);

            var spec = A.Fake<IMtVerifySpecification>();

            var handle = await _sut.ApplyVerificationAsync(timeSpanFactory, spec);

            Assert.That(handle, Is.Not.Null);
            A.CallTo(() => spec.Configure(
                A<IConditionInstanceTimeSpanFactory>.That.NullCheckedMatches(e =>
                    e == timeSpanFactory,
                    e => e.Write("Factory")),
                A<IReceiveEndpointConfigurator>._)).MustHaveHappened();
        }
    }
}
