using FakeItEasy;
using NUnit.Framework;
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ExpectationEvaluatorTests
    {
        [Test]
        public async Task Given_Verifier_When_Dispose_Then_DisposeInternals()
        {
            var conditionComponentEvaluator = A.Dummy<IComponentConditionEvaluator>();

            var evaluator = new ExpectationEvaluator(
                conditionComponentEvaluator,
                A.CollectionOfDummy<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>(0).ToList(),
                A.Dummy<IExpectationCriteriaEvaluator>());

            await evaluator.DisposeAsync();

            A.CallTo(() => conditionComponentEvaluator.DisposeAsync()).MustHaveHappened();
        }

        [Test]
        public async Task Given_Disposed_When_Evaluate_Then_Throw()
        {
            var conditionComponentEvaluator = A.Dummy<IComponentConditionEvaluator>();

            var evaluator = new ExpectationEvaluator(
                conditionComponentEvaluator,
                A.CollectionOfDummy<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>(0).ToList(),
                A.Dummy<IExpectationCriteriaEvaluator>());

            await evaluator.DisposeAsync();

            Assert.ThrowsAsync<ObjectDisposedException>(() => evaluator.EvaluateAsync());
        }
    }
}
