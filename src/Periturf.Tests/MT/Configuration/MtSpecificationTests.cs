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
using Periturf.MT.Configuration;
using System;

namespace Periturf.Tests.MT.Configuration
{
    [TestFixture]
    class MtSpecificationTests
    {
        private const string componentName = "ComponentName";

        [Test]
        public void Given_Spec_When_WhenMessageReceived_Then_Configured()
        {
            var spec = new MtSpecification(componentName);

            IWhenMessagePublishedConfigurator<IMessage> configurator = null;

            var whenMessageConfig = A.Fake<Action<IWhenMessagePublishedConfigurator<IMessage>>>();
            A.CallTo(() => whenMessageConfig.Invoke(A<IWhenMessagePublishedConfigurator<IMessage>>._)).Invokes((IWhenMessagePublishedConfigurator<IMessage> c) => configurator = c);
            
            spec.WhenMessagePublished(whenMessageConfig);

            Assert.That(configurator, Is.Not.Null);
            Assert.That(spec.WhenMessagePublishedSpecifications, Is.Not.Empty);
            Assert.That(spec.WhenMessagePublishedSpecifications, Does.Contain(configurator));
            A.CallTo(() => whenMessageConfig.Invoke(A<IWhenMessagePublishedConfigurator<IMessage>>._)).MustHaveHappened();
        }

        public interface IMessage
        {

        }
    }
}
