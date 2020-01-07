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
using Periturf.MT.Clients;
using System;

namespace Periturf.Tests.MT.Configuration
{
    [TestFixture]
    class MtClientExtensionTests
    {
        [Test]
        public void Given_Context_When_MtClient_Then_GetMtClient()
        {
            const string componentName = "ComponentName";

            var client = A.Dummy<IMTClient>();
            
            var context = A.Fake<IEventResponseContext<Object>>();
            A.CallTo(() => context.CreateComponentClient(A<string>._)).Returns(client);

            var result = context.MTClient(componentName);

            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.SameAs(client));
            A.CallTo(() => context.CreateComponentClient(componentName)).MustHaveHappened();
        }
    }
}
