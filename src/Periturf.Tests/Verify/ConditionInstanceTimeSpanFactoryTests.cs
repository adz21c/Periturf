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
using Periturf.Verify;
using System;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ConditionInstanceTimeSpanFactoryTests
    {
        [Test]
        public void Given_Factry_When_Create_Then_TimeDifferenceSinceCtor()
        {
            var start = new DateTime(2019, 11, 17, 09, 00, 00);
            var factory = new ConditionInstanceTimeSpanFactory(start);

            var ts1 = TimeSpan.FromMilliseconds(500);
            var ts2 = TimeSpan.FromMilliseconds(1000);

            var result1 = factory.Create(start.Add(ts1));
            var result2 = factory.Create(start.Add(ts2));

            Assert.AreEqual(ts1, result1);
            Assert.AreEqual(ts2, result2);
        }
    }
}
