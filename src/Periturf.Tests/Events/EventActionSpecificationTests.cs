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
using System;
using System.Threading.Tasks;

namespace Periturf.Tests.Events
{
    [TestFixture]
    class EventActionSpecificationTests
    {
        [Test]
        public void Given_MultipleResponses_When_ResponseAction_Then_Recorded()
        {
            var spec = new EventResponseSpecification<Object>();

            var action1 = A.Dummy<Func<Object, Task>>();
            var action2 = A.Dummy<Func<Object, Task>>();

            spec.Response(action1);
            spec.Response(action2);

            Assert.That(spec.Actions, Does.Contain(action1));
            Assert.That(spec.Actions, Does.Contain(action2));

            A.CallTo(() => action1.Invoke(A<Object>._)).MustNotHaveHappened();
            A.CallTo(() => action2.Invoke(A<Object>._)).MustNotHaveHappened();
        }
    }
}
