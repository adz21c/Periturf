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
using System;
using System.Collections.Generic;
using System.Text;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ConditionInstanceTests
    {
        [Test]
        public void Given_NegativeWhen_When_Ctor_Then_Throw()
        {
            var ex = Assert.Throws<ArgumentOutOfRangeException>(() => new ConditionInstance(TimeSpan.FromMilliseconds(-1), "ID"));
            Assert.AreEqual("when", ex.ParamName);
        }

        [TestCase(0, "ID", Description = "When 0")]
        [TestCase(1, "ID", Description = "When positive")]
        [TestCase(1, null, Description = "ID null")]
        [TestCase(1, "", Description = "ID empty string")]
        [TestCase(1, "ID", Description = "ID empty string")]
        public void Given_ValidParameters_When_Ctor_Then_Created(int when, string id)
        {
            var instance = new ConditionInstance(TimeSpan.FromMilliseconds(when), id);
            Assert.AreEqual(TimeSpan.FromMilliseconds(when), instance.When);
            Assert.AreEqual(id ?? string.Empty, instance.ID);
        }
    }
}
