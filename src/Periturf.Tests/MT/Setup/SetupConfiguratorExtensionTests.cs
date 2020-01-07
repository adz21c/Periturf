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
using Periturf.MT;
using Periturf.MT.Setup;
using Periturf.Setup;
using System;

namespace Periturf.Tests.MT.Setup
{
    [TestFixture]
    class SetupConfiguratorExtensionTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Given_BadName_When_MTBus_Then_Throws(string name)
        {
            var configurator = A.Fake<ISetupContext>();

            var ex = Assert.Throws<ArgumentNullException>(() => configurator.MTBus(name, A.Dummy<Action<IBusConfigurator>>()));
            Assert.That(ex.ParamName, Is.EqualTo("hostName"));
        }

        [Test]
        public void Given_NullConfig_When_MTBus_Then_Throws()
        {
            var configurator = A.Fake<ISetupContext>();

            var ex = Assert.Throws<ArgumentNullException>(() => configurator.MTBus("HostName", null));
            Assert.That(ex.ParamName, Is.EqualTo("config"));
        }


        [Test]
        public void Given_Config_When_MTBus_Then_NewHost()
        {
            var config = A.Dummy<Action<IBusConfigurator>>();
            A.CallTo(() => config.Invoke(A<IBusConfigurator>._)).Invokes((IBusConfigurator f) => f.InMemoryHost());
            const string hostName = "MTBus";

            var configurator = A.Fake<ISetupContext>();

            configurator.MTBus(hostName, config);

            A.CallTo(() =>
                configurator.Host(
                    hostName,
                    A<Components.IHost>.That.NullCheckedMatches(
                        x => x is MtHostComponent,
                        x => x.Write("MtHostComponent")))).MustHaveHappened();
            A.CallTo(() => config.Invoke(A<IBusConfigurator>._)).MustHaveHappened();
        }
    }
}
