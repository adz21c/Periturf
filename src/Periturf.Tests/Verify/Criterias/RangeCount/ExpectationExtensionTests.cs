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

        public void EachSetup()
        {
            Fake.ClearRecordedCalls(_expectationConfigurator);
            _spec = null;
        }

        [Test]
        public void Given_Configurator_When_MustNeverOccur_Then_CorrectConfig()
        {
            _expectationConfigurator.MustNeverOccur();

            Assert.IsNotNull(_spec);
            Assert.AreEqual(0, _spec.Maximum);
            Assert.IsNull(_spec.Minimum);
            Assert.IsNull(_spec.Timeout);
            Assert.AreEqual("Must never occur", _spec.Description);
        }

        [Test]
        public void Given_Configurator_When_MustNeverOccurWithin_Then_CorrectConfig()
        {
            var timeout = TimeSpan.FromMilliseconds(1000);
            _expectationConfigurator.MustNeverOccurWithin(timeout);

            Assert.IsNotNull(_spec);
            Assert.AreEqual(0, _spec.Maximum);
            Assert.IsNull(_spec.Minimum);
            Assert.AreEqual(timeout, _spec.Timeout);
            Assert.AreEqual($"Must never occur within {timeout.TotalMilliseconds}ms", _spec.Description);
        }

        [Test]
        public void Given_Configurator_When_MustOccur_Then_CorrectConfig()
        {
            _expectationConfigurator.MustOccur();

            Assert.IsNotNull(_spec);
            Assert.AreEqual(1, _spec.Minimum);
            Assert.IsNull(_spec.Maximum);
            Assert.IsNull(_spec.Timeout);
            Assert.AreEqual("Must occur", _spec.Description);
        }

        [Test]
        public void Given_Configurator_When_MustOccurWithin_Then_CorrectConfig()
        {
            var timeout = TimeSpan.FromMilliseconds(1000);
            _expectationConfigurator.MustOccurWithin(timeout);

            Assert.IsNotNull(_spec);
            Assert.AreEqual(1, _spec.Minimum);
            Assert.IsNull(_spec.Maximum);
            Assert.AreEqual(timeout, _spec.Timeout);
            Assert.AreEqual($"Must occur within {timeout.TotalMilliseconds}ms", _spec.Description);
        }

        [Test]
        public void Given_Configurator_When_MustOccurBetweenTimes_Then_CorrectConfig()
        {
            const int minimum = 1;
            const int maximum = 2;

            _expectationConfigurator.MustOccurBetweenTimes(minimum, maximum);

            Assert.IsNotNull(_spec);
            Assert.AreEqual(minimum, _spec.Minimum);
            Assert.AreEqual(maximum, _spec.Maximum);
            Assert.IsNull(_spec.Timeout);
            Assert.AreEqual($"Must occur between {minimum} and {maximum} times", _spec.Description);
        }

        [Test]
        public void Given_Configurator_When_MustOccurBetweenTimesWithin_Then_CorrectConfig()
        {
            const int minimum = 1;
            const int maximum = 2;
            var timeout = TimeSpan.FromMilliseconds(1000);

            _expectationConfigurator.MustOccurBetweenTimesWithin(minimum, maximum, timeout);

            Assert.IsNotNull(_spec);
            Assert.AreEqual(minimum, _spec.Minimum);
            Assert.AreEqual(maximum, _spec.Maximum);
            Assert.AreEqual(timeout, _spec.Timeout);
            Assert.AreEqual($"Must occur between {minimum} and {maximum} times within {timeout.TotalMilliseconds}ms", _spec.Description);
        }

        [Test]
        public void Given_Configurator_When_MustOccurMaximumTimes_Then_CorrectConfig()
        {
            const int maximum = 2;

            _expectationConfigurator.MustOccurMaximumTimes(maximum);

            Assert.IsNotNull(_spec);
            Assert.IsNull(_spec.Minimum);
            Assert.AreEqual(maximum, _spec.Maximum);
            Assert.IsNull(_spec.Timeout);
            Assert.AreEqual($"Must occur a maximum of {maximum} times", _spec.Description);
        }

        [Test]
        public void Given_Configurator_When_MustOccurMaximumTimesWithin_Then_CorrectConfig()
        {
            const int maximum = 2;
            var timeout = TimeSpan.FromMilliseconds(1000);

            _expectationConfigurator.MustOccurMaximumTimesWithin(maximum, timeout);

            Assert.IsNotNull(_spec);
            Assert.IsNull(_spec.Minimum);
            Assert.AreEqual(maximum, _spec.Maximum);
            Assert.AreEqual(timeout, _spec.Timeout);
            Assert.AreEqual($"Must occur a maximum of {maximum} times within {timeout.TotalMilliseconds}ms", _spec.Description);
        }

        [Test]
        public void Given_Configurator_When_MustOccurMinimumTimes_Then_CorrectConfig()
        {
            const int minimum = 2;

            _expectationConfigurator.MustOccurMinimumTimes(minimum);

            Assert.IsNotNull(_spec);
            Assert.AreEqual(minimum, _spec.Minimum);
            Assert.IsNull(_spec.Maximum);
            Assert.IsNull(_spec.Timeout);
            Assert.AreEqual($"Must occur a minimum of {minimum} times", _spec.Description);
        }

        [Test]
        public void Given_Configurator_When_MustOccurMinimumTimesWithin_Then_CorrectConfig()
        {
            const int minimum = 2;
            var timeout = TimeSpan.FromMilliseconds(1000);

            _expectationConfigurator.MustOccurMinimumTimesWithin(minimum, timeout);

            Assert.IsNotNull(_spec);
            Assert.AreEqual(minimum, _spec.Minimum);
            Assert.IsNull(_spec.Maximum);
            Assert.AreEqual(timeout, _spec.Timeout);
            Assert.AreEqual($"Must occur a minimum of {minimum} times within {timeout.TotalMilliseconds}ms", _spec.Description);
        }
    }
}
