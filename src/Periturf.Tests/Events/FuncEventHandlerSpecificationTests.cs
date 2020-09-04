/*
 *     Copyright 2020 Adam Burton (adz21c@gmail.com)
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
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Events
{
    [TestFixture]
    class FuncEventHandlerSpecificationTests
    {
        [Test]
        public void Given_Func_When_Build_Then_ReturnsFunc()
        {
            var func = A.Dummy<Func<IEventContext<object>, CancellationToken, Task>>();
            var spec = new FuncEventHandlerSpecification<object>(func);

            var returnedFunc = spec.Build();

            Assert.That(returnedFunc, Is.SameAs(func));
        }
    }
}
