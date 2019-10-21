using FakeItEasy;
using NUnit.Framework;
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ExpectationSpecificationFilterTests
    {
        private ExpectationEvaluator _expectationEvaluator;
        private Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>> _filter;
        private readonly ConditionInstance _conditionInstance = new ConditionInstance(TimeSpan.Zero, "ID");

        [OneTimeSetUp]
        public void SetUp()
        {
            var filterSpec = A.Fake<IExpectationFilterSpecification>();
            _filter = A.Dummy<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>();
            A.CallTo(() => filterSpec.Build()).Returns(_filter);

            var evaluator = A.Fake<IExpectationCriteriaEvaluator>();
            A.CallTo(() => evaluator.Completed).Returns(true);
            var criteriaSpec = A.Fake<IExpectationCriteriaSpecification>();
            A.CallTo(() => criteriaSpec.Build()).Returns(evaluator);

            var componentEvaluator = A.Fake<IComponentConditionEvaluator>();
            A.CallTo(() => componentEvaluator.GetInstances).Returns(new[] { _conditionInstance });

            var spec = new ExpectationSpecification();
            var configurator = (IExpectationConfigurator)spec;

            configurator.Where(x => x.AddSpecification(filterSpec));
            configurator.Must(criteriaSpec);

            _expectationEvaluator = spec.Build(componentEvaluator);
        }

        [Test]
        public async Task Given_NotFilterOut_When_Evaluator_Then_CriteriaMet()
        {
            A.CallTo(() => _filter.Invoke(A<IAsyncEnumerable<ConditionInstance>>._))
                .Returns(new[] { new ConditionInstance(TimeSpan.Zero, "") });
            A.CallTo(() => evaluator.).Returns(true);

            var result = await _expectationEvaluator.EvaluateAsync();

            Assert.IsNotNull(result);
            Assert.True(result.Met);
        }

        [Test]
        public async Task Given_FilterOut_When_Evaluator_Then_CriteriaMet()
        {
            A.CallTo(() => _filter.Invoke(A<IAsyncEnumerable<ConditionInstance>>._))
                .Returns(null);

            var result = await _expectationEvaluator.EvaluateAsync();

            Assert.IsNotNull(result);
            Assert.False(result.Met);
        }
    }
}
