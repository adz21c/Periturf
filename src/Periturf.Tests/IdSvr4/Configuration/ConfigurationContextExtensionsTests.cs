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
using Periturf.IdSvr4.Configuration;
using System;

namespace Periturf.Tests.IdSvr4.Configuration
{
    [TestFixture]
    class ConfigurationContextExtensionsTests
    {
        [Test]
        public void Given_Context_When_IdSvr4_Then_ConfiguredAndSpecAdded()
        {
            var context = A.Fake<IConfigurationContext>();
            var config = A.Fake<Action<IIdSvr4Configurator>>();

            context.IdSvr4(config);

            A.CallTo(() => config.Invoke(A<IIdSvr4Configurator>._)).MustHaveHappened();
            A.CallTo(() => context.AddSpecification(A<IConfigurationSpecification>._)).MustHaveHappened();
        }
    }
}
