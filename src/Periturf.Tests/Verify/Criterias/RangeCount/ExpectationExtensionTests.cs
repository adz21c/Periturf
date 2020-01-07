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
using Periturf.Verify;
using Periturf.Verify.Criterias;
using Periturf.Verify.Criterias.RangeCount;
using System;

namespace Periturf.Tests.Verify.Criterias.RangeCount
{
    [TestFixture]
    class ExpectationExtensionTests
    {
        private readonly IExpectationConfigurator _expectationConfigurator = A.Fake<IExpectationConfigurator>();
        private RangeCountCriteriaSpecification _spec;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            A.CallTo(() => _expectationConfigurator.Must(A<IExpectationCriteriaSpecification>._))
                .Invokes((IExpectationCriteriaSpecification s) => _spec = s as RangeCountCriteriaSpecification)
                .Returns(_expectationConfigurator);
        }

        [SetUp]
        public void EachSetup()
        {
            Fake.ClearRecordedCalls(_expectationConfigurator);
            _spec = null;
        }

        [Test]
        public void Given_Configurator_When_MustNeverOccur_Then_CorrectConfig()
        {
            _expectationConfigurator.MustNeverOccur();

            Assert.That(_spec, Is.Not.Null);
            Assert.That(_spec.Maximum, Is.EqualTo(0));
            Assert.That(_spec.Minimum, Is.Null);
            Assert.That(_spec.Timeout, Is.Null);
            Assert.That(_spec.Description, Is.EqualTo("Must never occur"));
        }

        [Test]
        public void Given_Configurator_When_MustNeverOccurWithin_Then_CorrectConfig()
        {
            var timeout = TimeSpan.FromMilliseconds(1000);
            _expectationConfigurator.MustNeverOccurWithin(timeout);

            Assert.That(_spec, Is.Not.Null);
            Assert.That(_spec.Maximum, Is.EqualTo(0));
            Assert.That(_spec.Minimum, Is.Null);
            Assert.That(_spec.Timeout, Is.EqualTo(timeout));
            Assert.That(_spec.Description, Is.EqualTo($"Must never occur within {timeout.TotalMilliseconds}ms"));
        }

        [Test]
        public void Given_Configurator_When_MustOccur_Then_CorrectConfig()
        {
            _expectationConfigurator.MustOccur();

            Assert.That(_spec, Is.Not.Null);
            Assert.That(_spec.Minimum, Is.EqualTo(1));
            Assert.That(_spec.Maximum, Is.Null);
            Assert.That(_spec.Timeout, Is.Null);
            Assert.That(_spec.Description, Is.EqualTo("Must occur"));
        }

        [Test]
        public void Given_Configurator_When_MustOccurWithin_Then_CorrectConfig()
        {
            var timeout = TimeSpan.FromMilliseconds(1000);
            _expectationConfigurator.MustOccurWithin(timeout);

            Assert.That(_spec, Is.Not.Null);
            Assert.That(_spec.Minimum, Is.EqualTo(1));
            Assert.That(_spec.Maximum, Is.Null);
            Assert.That(_spec.Timeout, Is.EqualTo(timeout));
            Assert.That(_spec.Description, Is.EqualTo($"Must occur within {timeout.TotalMilliseconds}ms"));
        }

        [Test]
        public void Given_Configurator_When_MustOccurBetweenTimes_Then_CorrectConfig()
        {
            const int minimum = 1;
            const int maximum = 2;

            _expectationConfigurator.MustOccurBetweenTimes(minimum, maximum);

            Assert.That(_spec, Is.Not.Null);
            Assert.That(_spec.Minimum, Is.EqualTo(minimum));
            Assert.That(_spec.Maximum, Is.EqualTo(maximum));
            Assert.That(_spec.Timeout, Is.Null);
            Assert.That(_spec.Description, Is.EqualTo($"Must occur between {minimum} and {maximum} times"));
        }

        [Test]
        public void Given_Configurator_When_MustOccurBetweenTimesWithin_Then_CorrectConfig()
        {
            const int minimum = 1;
            const int maximum = 2;
            var timeout = TimeSpan.FromMilliseconds(1000);

            _expectationConfigurator.MustOccurBetweenTimesWithin(minimum, maximum, timeout);

            Assert.That(_spec, Is.Not.Null);
            Assert.That(_spec.Minimum, Is.EqualTo(minimum));
            Assert.That(_spec.Maximum, Is.EqualTo(maximum));
            Assert.That(_spec.Timeout, Is.EqualTo(timeout));
            Assert.That(_spec.Description, Is.EqualTo($"Must occur between {minimum} and {maximum} times within {timeout.TotalMilliseconds}ms"));
        }

        [Test]
        public void Given_Configurator_When_MustOccurMaximumTimes_Then_CorrectConfig()
        {
            const int maximum = 2;

            _expectationConfigurator.MustOccurMaximumTimes(maximum);

            Assert.That(_spec, Is.Not.Null);
            Assert.That(_spec.Minimum, Is.Null);
            Assert.That(_spec.Maximum, Is.EqualTo(maximum));
            Assert.That(_spec.Timeout, Is.Null);
            Assert.That(_spec.Description, Is.EqualTo($"Must occur a maximum of {maximum} times"));
        }

        [Test]
        public void Given_Configurator_When_MustOccurMaximumTimesWithin_Then_CorrectConfig()
        {
            const int maximum = 2;
            var timeout = TimeSpan.FromMilliseconds(1000);

            _expectationConfigurator.MustOccurMaximumTimesWithin(maximum, timeout);

            Assert.That(_spec, Is.Not.Null);
            Assert.That(_spec.Minimum, Is.Null);
            Assert.That(_spec.Maximum, Is.EqualTo(maximum));
            Assert.That(_spec.Timeout, Is.EqualTo(timeout));
            Assert.That(_spec.Description, Is.EqualTo($"Must occur a maximum of {maximum} times within {timeout.TotalMilliseconds}ms"));
        }

        [Test]
        public void Given_Configurator_When_MustOccurMinimumTimes_Then_CorrectConfig()
        {
            const int minimum = 2;

            _expectationConfigurator.MustOccurMinimumTimes(minimum);

            Assert.That(_spec, Is.Not.Null);
            Assert.That(_spec.Minimum, Is.EqualTo(minimum));
            Assert.That(_spec.Maximum, Is.Null);
            Assert.That(_spec.Timeout, Is.Null);
            Assert.That(_spec.Description, Is.EqualTo($"Must occur a minimum of {minimum} times"));
        }

        [Test]
        public void Given_Configurator_When_MustOccurMinimumTimesWithin_Then_CorrectConfig()
        {
            const int minimum = 2;
            var timeout = TimeSpan.FromMilliseconds(1000);

            _expectationConfigurator.MustOccurMinimumTimesWithin(minimum, timeout);

            Assert.That(_spec, Is.Not.Null);
            Assert.That(_spec.Minimum, Is.EqualTo(minimum));
            Assert.That(_spec.Maximum, Is.Null);
            Assert.That(_spec.Timeout, Is.EqualTo(timeout));
            Assert.That(_spec.Description, Is.EqualTo($"Must occur a minimum of {minimum} times within {timeout.TotalMilliseconds}ms"));
        }
    }
}
