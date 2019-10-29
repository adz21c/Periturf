using FakeItEasy;
using NUnit.Framework;
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ExpectationSpecificationAndEvaluatorFilterAndCriteriaTests
    {
        private ExpectationEvaluator _expectationEvaluator;
        private Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>> _filter;
        private readonly ConditionInstance _conditionInstance = new ConditionInstance(TimeSpan.Zero, "ID");
        private readonly IAsyncEnumerable<ConditionInstance> _conditionInstances;
        private IExpectationCriteriaEvaluator _evaluator;

        public ExpectationSpecificationAndEvaluatorFilterAndCriteriaTests()
        {
            _conditionInstances = new[] { _conditionInstance }.AsAsyncEnumerable();
        }

        [SetUp]
        public void SetUp()
        {
            var filterSpec = A.Fake<IExpectationFilterSpecification>();
            _filter = A.Dummy<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>();
            A.CallTo(() => filterSpec.Build()).Returns(_filter);

            _evaluator = A.Fake<IExpectationCriteriaEvaluator>();
            A.CallTo(() => _evaluator.Completed).Returns(true);
            var criteriaSpec = A.Fake<IExpectationCriteriaSpecification>();
            A.CallTo(() => criteriaSpec.Build()).Returns(_evaluator);

            var componentEvaluator = A.Fake<IComponentConditionEvaluator>();
            A.CallTo(() => componentEvaluator.GetInstances()).Returns(_conditionInstances);

            var spec = new ExpectationSpecification();
            var configurator = (IExpectationConfigurator)spec;

            configurator.Where(x => x.AddSpecification(filterSpec));
            configurator.Must(criteriaSpec);

            _expectationEvaluator = spec.Build(componentEvaluator);
        }

        [Test]
        public async Task Given_NotFilterOut_When_Evaluator_Then_PassedToCriteria()
        {
            A.CallTo(() => _filter.Invoke(A<IAsyncEnumerable<ConditionInstance>>._)).Returns(_conditionInstances);
            A.CallTo(() => _evaluator.Met).Returns(true);

            var result = await _expectationEvaluator.EvaluateAsync();

            Assert.IsNotNull(result);
            A.CallTo(() => _filter.Invoke(_conditionInstances)).MustHaveHappened();
            A.CallTo(() => _evaluator.Evaluate(_conditionInstance)).MustHaveHappened();
        }

        [Test]
        public async Task Given_FilterOut_When_Evaluator_Then_NotPassedToCriteria()
        {
            A.CallTo(() => _filter.Invoke(A<IAsyncEnumerable<ConditionInstance>>._)).Returns(Enumerable.Empty<ConditionInstance>().AsAsyncEnumerable());
            A.CallTo(() => _evaluator.Met).Returns(true);

            var result = await _expectationEvaluator.EvaluateAsync();

            Assert.IsNotNull(result);
            A.CallTo(() => _filter.Invoke(_conditionInstances)).MustHaveHappened();
            A.CallTo(() => _evaluator.Evaluate(_conditionInstance)).MustNotHaveHappened();
        }

        [Test]
        public async Task Given_TrueCriteria_When_Evaluator_Then_CriteriaMet()
        {
            A.CallTo(() => _filter.Invoke(A<IAsyncEnumerable<ConditionInstance>>._)).Returns(_conditionInstances);
            A.CallTo(() => _evaluator.Met).Returns(true);

            var result = await _expectationEvaluator.EvaluateAsync();

            Assert.IsNotNull(result);
            Assert.True(result.Met);
            A.CallTo(() => _evaluator.Evaluate(_conditionInstance)).MustHaveHappened();
        }

        [Test]
        public async Task Given_FalseCriteria_When_Evaluator_Then_CriteriaNotMet()
        {
            A.CallTo(() => _filter.Invoke(A<IAsyncEnumerable<ConditionInstance>>._)).Returns(_conditionInstances);
            A.CallTo(() => _evaluator.Met).Returns(false);

            var result = await _expectationEvaluator.EvaluateAsync();

            Assert.IsNotNull(result);
            Assert.False(result.Met);
            A.CallTo(() => _evaluator.Evaluate(_conditionInstance)).MustHaveHappened();
        }
    }
}
