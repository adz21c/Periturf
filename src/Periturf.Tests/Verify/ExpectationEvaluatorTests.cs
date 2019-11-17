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
using FakeItEasy;
using NUnit.Framework;
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ExpectationEvaluatorTests
    {
        private MockComponentEvaluator _componentEvaluator;
        private IExpectationCriteriaEvaluator _criteriaEvaluator;
        private IExpectationCriteriaEvaluatorFactory _criteriaFactory;
        private ExpectationEvaluator _sut;

        [SetUp]
        public void SetUp()
        {
            _componentEvaluator = new MockComponentEvaluator(TimeSpan.FromMilliseconds(50), 1);

            _criteriaEvaluator = A.Fake<IExpectationCriteriaEvaluator>();
            A.CallTo(() => _criteriaEvaluator.Met).Returns(true);

            _criteriaFactory = A.Fake<IExpectationCriteriaEvaluatorFactory>();
            A.CallTo(() => _criteriaFactory.CreateInstance()).Returns(_criteriaEvaluator);

            _sut = new ExpectationEvaluator(
                TimeSpan.FromMilliseconds(100),
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
            // Prepare for evaluator to take longer than cancelling
            _sut = new ExpectationEvaluator(
                TimeSpan.FromMilliseconds(1000),  // Cancel second
                new MockComponentEvaluator(TimeSpan.FromMilliseconds(1000), null),
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
            // Prepare for evaluator to take longer than cancelling
            _sut = new ExpectationEvaluator(
                TimeSpan.FromMilliseconds(100),  // Cancel first
                new MockComponentEvaluator(TimeSpan.FromMilliseconds(1000), null),
                new List<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>(),
                _criteriaFactory);

            // Go
            var tokenSource = new CancellationTokenSource();
            var evaluateTask = Task.Run(async () => await _sut.EvaluateAsync(tokenSource.Token));

            // Cancel second
            await Task.Delay(300);
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
                TimeSpan.FromMilliseconds(500),
                new MockComponentEvaluator(TimeSpan.FromMilliseconds(1000), 5),
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
            _componentEvaluator.ResetCalls();

            await _sut.DisposeAsync();

            Assert.IsFalse(_componentEvaluator.DisposeCalled);
        }

        [Test]
        [SuppressMessage("Blocker Code Smell", "S2699:Tests should include assertions", Justification = "Assertions in shared method")]
        public async Task Given_Evaluator_When_Dispose_Then_DisposeInternals()
        {
            await _sut.DisposeAsync();
            TestDependenciesCleanUp();
        }

        [Test]
        public async Task Given_AlreadyDisposed_When_Dispose_Then_Nothing()
        {
            await _sut.DisposeAsync();
            TestDependenciesCleanUp();

            _componentEvaluator.ResetCalls();
            await _sut.DisposeAsync();

            Assert.IsFalse(_componentEvaluator.DisposeCalled);
        }

        [Test]
        public async Task Given_Disposed_When_Evaluate_Then_Throw()
        {
            await _sut.DisposeAsync();
            Assert.ThrowsAsync<ObjectDisposedException>(() => _sut.EvaluateAsync());
        }

        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public async Task Given_CriteriaMetEarly_When_Evaluate_Then_StopCheckingInstances(int incompleteCount)
        {
            var results = new List<bool>();
            results.AddRange(Enumerable.Repeat(false, incompleteCount));
            results.Add(true);

            A.CallTo(() => _criteriaEvaluator.Evaluate(A<ConditionInstance>._)).ReturnsNextFromSequence(results.ToArray());

            _componentEvaluator = new MockComponentEvaluator(TimeSpan.FromMilliseconds(10), 5);
            _sut = new ExpectationEvaluator(
                TimeSpan.FromMilliseconds(1000),
                _componentEvaluator,
                new List<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>(),
                _criteriaFactory);

            await _sut.EvaluateAsync();

            Assert.AreEqual(incompleteCount + 1, _componentEvaluator.InstanceCount);
        }

        private void TestDependenciesCleanUp()
        {
            Assert.IsTrue(_componentEvaluator.DisposeCalled);
        }
    }
}
