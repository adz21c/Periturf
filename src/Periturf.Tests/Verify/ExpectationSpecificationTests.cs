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

        [Test]
        public void Given_NullComponent_When_Build_Then_Throw()
        {

            // Arrange
            var spec = new ExpectationSpecification();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => spec.Build(null));
        }

        [Test]
        public void Given_NotConfigured_When_Build_Then_Throw()
        {

            // Arrange
            var spec = new ExpectationSpecification();
            var component = A.Dummy<IComponentConditionEvaluator>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => spec.Build(component));
        }
    }
}
