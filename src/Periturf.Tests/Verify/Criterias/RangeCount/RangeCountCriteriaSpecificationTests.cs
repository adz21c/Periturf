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
using NUnit.Framework;
using Periturf.Verify.Criterias;
using Periturf.Verify.Criterias.RangeCount;
using System;

namespace Periturf.Tests.Verify.Criterias.RangeCount
{
    [TestFixture]
    class RangeCountCriteriaSpecificationTests
    {
        [Test]
        public void Given_Config_When_Ctor_Then_Mapped()
        {
            const int minimum = 1;
            const int maximum = 2;
            var timeout = TimeSpan.FromMilliseconds(1000);

            var spec = new RangeCountCriteriaSpecification(minimum, maximum, timeout);

            Assert.That(spec.Minimum, Is.EqualTo(minimum));
            Assert.That(spec.Maximum, Is.EqualTo(maximum));
            Assert.That(spec.Timeout, Is.EqualTo(timeout));
        }

        [Test]
        public void Given_Config_When_Build_Then_ConfigMapped()
        {
            const int minimum = 1;
            const int maximum = 2;
            var timeout = TimeSpan.FromMilliseconds(1000);
            var spec = new RangeCountCriteriaSpecification(minimum, maximum, timeout);

            var factory = spec.Build();

            Assert.That(factory, Is.Not.Null);

            var typedFactory = factory as RangeCountCriteriaEvaluatorFactory;
            Assert.That(typedFactory, Is.Not.Null);

            Assert.That(typedFactory.Minimum, Is.EqualTo(minimum));
            Assert.That(typedFactory.Maximum, Is.EqualTo(maximum));
        }

        [Test]
        public void Given_BackwardMinMax_When_Ctor_Then_Throws()
        {
            const int minimum = 2;
            const int maximum = 1;

            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new RangeCountCriteriaSpecification(minimum, maximum, null));

            Assert.That(ex.ParamName, Is.EqualTo("maximum"));
        }

        [Test]
        public void Given_NoConfig_When_Ctor_Then_Throws()
        {
            Assert.Throws<ArgumentException>(() => new RangeCountCriteriaSpecification(null, null, null));
        }

        [TestCase(1, null, null, ExpectedResult = "Minimum of 1 instances")]
        [TestCase(null, 1, null, ExpectedResult = "Maximum of 1 instances")]
        [TestCase(1, 2, null, ExpectedResult = "Between 1 and 2 instances")]
        [TestCase(1, 2, "My Override", ExpectedResult = "My Override")]
        public string Given_Config_When_Ctor_Then_Description(int? minimum, int? maximum, string description)
        {
            return new RangeCountCriteriaSpecification(minimum, maximum, null, description).Description;
        }
    }
}
