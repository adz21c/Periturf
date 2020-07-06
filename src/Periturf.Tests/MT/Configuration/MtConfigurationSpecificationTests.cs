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
using Periturf.MT.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.MT.Configuration
{
    [TestFixture]
    class MtConfigurationSpecificationTests
    {
        [Test]
        public async Task Given_Configuration_When_Apply_Then_BusConfigured()
        {
            const string componentName = "ComponentName";

            var busManager = A.Fake<IBusManager>();
            var factory = A.Fake<IEventHandlerContextFactory>();

            var configSpec = new MtConfigurationSpecification(busManager, factory, componentName);
            var configHandle = await configSpec.ApplyAsync(CancellationToken.None);

            Assert.That(configHandle, Is.Not.Null);
            A.CallTo(() => busManager.ApplyConfigurationAsync(configSpec.MtSpec, factory)).MustHaveHappened();
        }
    }
}
