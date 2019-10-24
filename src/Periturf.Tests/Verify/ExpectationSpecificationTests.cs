using FakeItEasy;
using NUnit.Framework;
using Periturf.Verify;
using System;
using System.Collections.Generic;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ExpectationSpecificationTests
    {
        [Test]
        public void Given_NullConfigurator_When_Where_Then_Throw()
        {
            // Arrange
            var spec = (IExpectationConfigurator)new ExpectationSpecification();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => spec.Where(null));
        }

        [Test]
        public void Given_NullSpecification_When_Must_Then_Throw()
        {
            // Arrange
            var spec = (IExpectationConfigurator)new ExpectationSpecification();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => spec.Must(null));
        }

        [Test]
        public void Given_NullSpecification_When_FilterAddSpecification_Then_Throw()
        {

            // Arrange
            var spec = (IExpectationFilterConfigurator)new ExpectationSpecification();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => spec.AddSpecification(null));
        }
    }
}
