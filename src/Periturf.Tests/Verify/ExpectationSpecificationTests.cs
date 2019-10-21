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
        public async System.Threading.Tasks.Task Given__When__Then_Async()
        {
            // Arrange
            var filterSpec = A.Fake<IExpectationFilterSpecification>();
            var filter = A.Dummy<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>();
            A.CallTo(() => filterSpec.Build()).Returns(filter);

            var criteriaSpec = A.Fake<IExpectationCriteriaSpecification>();
            var evaluator = A.Fake<IExpectationCriteriaEvaluator>();
            A.CallTo(() => criteriaSpec.Build()).Returns(evaluator);

            var componentEvaluator = A.Fake<IComponentConditionEvaluator>();

            var spec = new ExpectationSpecification();
            var configurator = (IExpectationConfigurator)spec;

            // Act
            configurator.Where(x => x.AddSpecification(filterSpec));
            configurator.Must(criteriaSpec);

            var expectationEvaluator = spec.Build(componentEvaluator);
            Assert.IsNotNull(expectationEvaluator);

            var result = await expectationEvaluator.EvaluateAsync();

            Assert.IsNotNull(result);
            Assert.True(result.Met);
        }
    }
}
