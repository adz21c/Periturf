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
