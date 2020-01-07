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
using Periturf.MT.Events;
using Periturf.MT.Verify;
using System;

namespace Periturf.Tests.MT.Verify
{
    [TestFixture]
    class MtConditionBuilderTests
    {
        private MtConditionBuilder _sut;
        private IBusManager _busManager;

        [SetUp]
        public void SetUp()
        {
            _busManager = A.Fake<IBusManager>();
            _sut = new MtConditionBuilder(_busManager);
        }

        [Test]
        public void Given_ConditionBuilder_When_WhenMessagePublished_Then_SpecificationReturned()
        {
            var condition = A.Dummy<Func<IMessageReceivedContext<ITestMessage>, bool>>();
            var spec = _sut.WhenMessagePublished<ITestMessage>(c => c.Predicate(condition));
            
            Assert.That(spec, Is.Not.Null);
            Assert.That(spec, Is.TypeOf<Periturf.MT.Verify.WhenMessagePublishedSpecification<ITestMessage>>());
            var typedSpec = (Periturf.MT.Verify.WhenMessagePublishedSpecification<ITestMessage>)spec;
            Assert.That(typedSpec.Predicates, Does.Contain(condition));
        }
    }
}
