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
using Periturf.Configuration;
using Periturf.Events;
using Periturf.MT;
using Periturf.MT.Configuration;
using System;

namespace Periturf.Tests.MT.Configuration
{
    [TestFixture]
    class MtConfigurationContextExtensionTests
    {
        [Test]
        public void Given_MtConfigSpec_When_MTBus_Then_MtConfigurationRegistered()
        {
            const string componentName = "Component1";

            var busManager = A.Fake<IBusManager>();
            var factory = A.Fake<IEventResponseContextFactory>();

            var mtSpec = new MtConfigurationSpecification(busManager, factory, componentName);

            var context = A.Fake<IConfigurationContext>();
            A.CallTo(() => context.CreateComponentConfigSpecification<MtConfigurationSpecification>(componentName)).Returns(mtSpec);

            var config = A.Fake<Action<IMtConfigurator>>();

            context.MTBus(componentName, config);

            A.CallTo(() => config.Invoke(A<IMtConfigurator>._)).MustHaveHappened();
            A.CallTo(() => context.CreateComponentConfigSpecification<MtConfigurationSpecification>(componentName)).MustHaveHappened();
            A.CallTo(() => context.AddSpecification(mtSpec)).MustHaveHappened();
        }
    }
}
