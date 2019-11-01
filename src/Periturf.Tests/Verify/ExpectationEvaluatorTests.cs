using FakeItEasy;
using NUnit.Framework;
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ExpectationEvaluatorTests
    {
        private IComponentConditionEvaluator _componentEvaluator;
        private IExpectationCriteriaEvaluator _criteriaEvaluator;
        private IExpectationCriteriaEvaluatorFactory _criteriaFactory;
        private ExpectationEvaluator _sut;

        [SetUp]
        public void SetUp()
        {
            _componentEvaluator = A.Fake<IComponentConditionEvaluator>();

            _criteriaEvaluator = A.Fake<IExpectationCriteriaEvaluator>();
            A.CallTo(() => _criteriaEvaluator.Met).Returns(true);
            A.CallTo(() => _criteriaEvaluator.Completed).Returns(true);

            _criteriaFactory = A.Fake<IExpectationCriteriaEvaluatorFactory>();
            A.CallTo(() => _criteriaFactory.CreateInstance()).Returns(_criteriaEvaluator);

            _sut = new ExpectationEvaluator(
                _componentEvaluator,
                new List<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>(),
                _criteriaFactory);
        }

        [TearDown]
        public void TearDown()
        {
            _componentEvaluator = null;
            _criteriaEvaluator = null;
            _criteriaFactory = null;
            _sut = null;
        }

        [Test]
        public async Task Given_Evaluator_When_Evaluate_Then_ResultAndDisposeDependencies()
        {
            var result = await _sut.EvaluateAsync();

            Assert.NotNull(result);
            Assert.IsTrue(result.Met);
            Assert.IsTrue(result.Completed);

            TestDependenciesCleanUp();
        }

        [Test]
        public async Task Given_Evaluator_When_CancelToken_Then_ResultIsInconclusive()
        {
            A.CallTo(() => _criteriaFactory.Timeout).Returns(TimeSpan.FromMilliseconds(500)); // Cancel second

            // Prepare for evaluator to take longer than cancelling
            _sut = new ExpectationEvaluator(
                new TimeoutEvaluator(TimeSpan.FromMilliseconds(1000), null),
                new List<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>(),
                _criteriaFactory);

            // Go
            var tokenSource = new CancellationTokenSource();
            var evaluateTask = Task.Run(async () => await _sut.EvaluateAsync(tokenSource.Token));

            // Cancel first
            await Task.Delay(250);
            tokenSource.Cancel();

            Assert.ThrowsAsync<TaskCanceledException>(() => evaluateTask);
        }

        [Test]
        public async Task Given_Evaluator_When_Timeout_Then_ResultCompleted()
        {
            A.CallTo(() => _criteriaFactory.Timeout).Returns(TimeSpan.FromMilliseconds(250)); // Cancel first

            // Prepare for evaluator to take longer than cancelling
            _sut = new ExpectationEvaluator(
                new TimeoutEvaluator(TimeSpan.FromMilliseconds(1000), null),
                new List<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>(),
                _criteriaFactory);

            // Go
            var tokenSource = new CancellationTokenSource();
            var evaluateTask = Task.Run(async () => await _sut.EvaluateAsync(tokenSource.Token));

            // Cancel second
            await Task.Delay(500);
            tokenSource.Cancel();

            var result = await evaluateTask;

            Assert.NotNull(result);
            Assert.True(result.Completed);
            Assert.NotNull(result.Met);
        }

        [Test]
        public async Task Given_AlreadyEvaluating_When_Evaluate_Then_Throws()
        {
            // For an await with a timeout evaluator
            _sut = new ExpectationEvaluator(
                new TimeoutEvaluator(TimeSpan.FromMilliseconds(1000), 5),
                new List<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>(),
                _criteriaFactory);

            var evaluatorTask = _sut.EvaluateAsync();
            Assert.ThrowsAsync<InvalidOperationException>(() => _sut.EvaluateAsync());
            await evaluatorTask;    // Allow to finish
        }

        [Test]
        public async Task Given_AlreadyEvaluated_When_Evaluate_Then_SameResult()
        {
            var firstResult = await _sut.EvaluateAsync();
            var secondResult = await _sut.EvaluateAsync();

            Assert.NotNull(firstResult);
            Assert.NotNull(secondResult);
            Assert.AreSame(firstResult, secondResult);
        }

        [Test]
        public async Task Given_AlreadyEvaluated_When_Dispose_Then_DependenciesNotDisposedTwice()
        {
            var result = await _sut.EvaluateAsync();

            Assume.That(result != null);
            Assume.That(result.Completed);
            Assume.That(result.Met == true);

            TestDependenciesCleanUp();
            Fake.ClearRecordedCalls(_componentEvaluator);

            await _sut.DisposeAsync();

            A.CallTo(() => _componentEvaluator.DisposeAsync()).MustNotHaveHappened();
        }

        [Test]
        [SuppressMessage("Blocker Code Smell", "S2699:Tests should include assertions", Justification = "Assertions in shared method")]
        public async Task Given_Evaluator_When_Dispose_Then_DisposeInternals()
        {
            await _sut.DisposeAsync();
            TestDependenciesCleanUp();
        }

        [Test]
        public async Task Given_Disposed_When_Evaluate_Then_Throw()
        {
            await _sut.DisposeAsync();
            Assert.ThrowsAsync<ObjectDisposedException>(() => _sut.EvaluateAsync());
        }

        private void TestDependenciesCleanUp()
        {
            A.CallTo(() => _componentEvaluator.DisposeAsync()).MustHaveHappened();
        }

        class TimeoutEvaluator : IComponentConditionEvaluator
        {
            private readonly TimeSpan _delays;
            private readonly int? _numberOfInstances;

            public TimeoutEvaluator(TimeSpan delays, int? numberOfInstances)
            {
                _delays = delays;
                _numberOfInstances = numberOfInstances;
            }

            public ValueTask DisposeAsync()
            {
                return new ValueTask();
            }

            public async IAsyncEnumerable<ConditionInstance> GetInstancesAsync([EnumeratorCancellation] CancellationToken ect = default)
            {
                for(int i = 0; !ect.IsCancellationRequested && (!_numberOfInstances.HasValue || i < _numberOfInstances); ++i)
                {
                    await Task.Delay(_delays, ect);
                    yield return new ConditionInstance(TimeSpan.FromMilliseconds(1), "ID");
                }
            }
        }
    }
}
