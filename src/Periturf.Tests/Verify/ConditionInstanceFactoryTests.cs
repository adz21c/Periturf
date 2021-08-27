//
//   Copyright 2021 Adam Burton (adz21c@gmail.com)
//   
//   Licensed under the Apache License, Version 2.0 (the "License")
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//

using FakeItEasy;
using NUnit.Framework;
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Text;

namespace Periturf.Tests.Verify
{
    class ConditionInstanceFactoryTests
    {
        private ITime _time;
        private ConditionInstanceFactory _sut;
        private readonly DateTime _now = DateTime.Now;

        [SetUp]
        public void SetUp()
        {
            _time = A.Fake<ITime>();
            A.CallTo(() => _time.Now).Returns(_now);

            _sut = new ConditionInstanceFactory(_time);
        }

        [Test]
        public void Given_NotStarted_When_Create_Then_InstanceZeroTime()
        {
            const string id = "MyId";

            var instance = _sut.Create(id);

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.ID, Is.EqualTo(id));
            Assert.That(instance.When, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void Given_NotStarted_When_CreateWithDate_Then_InstanceZeroTime()
        {
            const string id = "MyId";

            var instance = _sut.Create(id, _now.AddMinutes(-1));

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.ID, Is.EqualTo(id));
            Assert.That(instance.When, Is.EqualTo(TimeSpan.Zero));
        }

        [Test]
        public void Given_Started_When_Create_Then_InstanceWithTime()
        {
            const string id = "MyId";
            var timeSpan = TimeSpan.FromMinutes(5);

            _sut.Start();
            A.CallTo(() => _time.Now).Returns(_now.Add(timeSpan));

            var instance = _sut.Create(id);

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.ID, Is.EqualTo(id));
            Assert.That(instance.When, Is.EqualTo(timeSpan));
        }

        [Test]
        public void Given_Started_When_CreateWithDate_Then_InstanceWithTime()
        {
            const string id = "MyId";
            var timeSpan = TimeSpan.FromMinutes(2);

            _sut.Start();

            var newNow = _now.AddMinutes(5);
            A.CallTo(() => _time.Now).Returns(newNow);

            var date = newNow.Add(-timeSpan);

            var instance = _sut.Create(id, date);

            Assert.That(instance, Is.Not.Null);
            Assert.That(instance.ID, Is.EqualTo(id));
            Assert.That(instance.When, Is.EqualTo(date - _now));
        }
    }
}
