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

namespace Periturf.Tests.Events
{
    [TestFixture]
    class EventSpecificationTests
    {
        [Test]
        public void Given_MultiplePredicates_When_Predicate_Then_Recorded()
        {
            var spec = new EventSpecification<Object>();

            var predicate1 = A.Dummy<Func<Object, bool>>();
            var predicate2 = A.Dummy<Func<Object, bool>>();

            spec.Predicate(predicate1);
            spec.Predicate(predicate2);

            Assert.That(spec.Predicates, Does.Contain(predicate1));
            Assert.That(spec.Predicates, Does.Contain(predicate2));

            A.CallTo(() => predicate1.Invoke(A<Object>._)).MustNotHaveHappened();
            A.CallTo(() => predicate2.Invoke(A<Object>._)).MustNotHaveHappened();
        }
    }
}
