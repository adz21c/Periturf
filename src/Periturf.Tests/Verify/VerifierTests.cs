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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class VerifierTests
    {
        private MockComponentEvaluator _componentEvaluator1;
        private IExpectationCriteriaEvaluator _criteriaEvaluator1;
        private IExpectationCriteriaEvaluatorFactory _criteriaFactory1;
        private ExpectationEvaluator _expectation1;
        private MockComponentEvaluator _componentEvaluator2;
        private IExpectationCriteriaEvaluator _criteriaEvaluator2;
        private IExpectationCriteriaEvaluatorFactory _criteriaFactory2;
        private ExpectationEvaluator _expectation2;

        [SetUp]
        public void SetUp()
        {
            _componentEvaluator1 = new MockComponentEvaluator(TimeSpan.FromMilliseconds(1000), null);

            _criteriaEvaluator1 = A.Fake<IExpectationCriteriaEvaluator>();
            A.CallTo(() => _criteriaEvaluator1.Met).Returns(true);

            _criteriaFactory1 = A.Fake<IExpectationCriteriaEvaluatorFactory>();
            A.CallTo(() => _criteriaFactory1.CreateInstance()).Returns(_criteriaEvaluator1);

            _expectation1 = new ExpectationEvaluator(
                TimeSpan.FromMilliseconds(500),
                _componentEvaluator1,
                new List<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>(),
                _criteriaFactory1);


            _componentEvaluator2 = new MockComponentEvaluator(TimeSpan.FromMilliseconds(1000), null);

            _criteriaEvaluator2 = A.Fake<IExpectationCriteriaEvaluator>();
            A.CallTo(() => _criteriaEvaluator2.Met).Returns(true);

            _criteriaFactory2 = A.Fake<IExpectationCriteriaEvaluatorFactory>();
            A.CallTo(() => _criteriaFactory2.CreateInstance()).Returns(_criteriaEvaluator2);

            _expectation2 = new ExpectationEvaluator(
                TimeSpan.FromMilliseconds(500),
                _componentEvaluator2,
                new List<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>(),
                _criteriaFactory2);
        }

        [Test]
        public async Task Given_AllExpectationMet_When_Verify_Then_ExpectationsMet()
        {
            var sut = new Verifier(new List<ExpectationEvaluator> { _expectation1, _expectation2 });

            var result = await sut.VerifyAsync();
            
            Assert.NotNull(result);
            Assert.True(result.ExpectationsMet);
            Assert.IsNotEmpty(result.ExpectationResults);
            Assert.IsTrue(result.ExpectationResults.All(x => x.Met ?? false));
            Assert.IsTrue(result.ExpectationResults.All(x => x.Completed));
        }

        [Test]
        public async Task Given_AllExpectationNotMet_When_Verify_Then_ExpectationsNotMet()
        {
            A.CallTo(() => _criteriaEvaluator1.Met).Returns(false);
            A.CallTo(() => _criteriaEvaluator2.Met).Returns(false);
            var sut = new Verifier(new List<ExpectationEvaluator> { _expectation1, _expectation2 });

            var result = await sut.VerifyAsync();

            Assert.NotNull(result);
            Assert.IsFalse(result.ExpectationsMet);
            Assert.IsNotEmpty(result.ExpectationResults);
            Assert.IsTrue(result.ExpectationResults.All(x => !(x.Met ?? false)));
            Assert.IsTrue(result.ExpectationResults.All(x => x.Completed));
        }

        [Test]
        public async Task Given_AnExpectationNotMet_When_Verify_Then_ExpectationsNotMet()
        {
            A.CallTo(() => _criteriaEvaluator1.Met).Returns(false);
            var sut = new Verifier(new List<ExpectationEvaluator> { _expectation1, _expectation2 });

            var result = await sut.VerifyAsync();

            Assert.NotNull(result);
            Assert.IsFalse(result.ExpectationsMet);
            Assert.IsNotEmpty(result.ExpectationResults);
            Assert.IsFalse(result.ExpectationResults.All(x => x.Met ?? false));
            Assert.IsTrue(result.ExpectationResults.Any(x => x.Met ?? false));
            Assert.IsTrue(result.ExpectationResults.All(x => x.Completed));
        }

        [Test]
        public async Task Given_AllExpectationMetWhileShortCircuit_When_Verify_Then_ExpectationsNotMet()
        {
            _expectation1 = new ExpectationEvaluator(
                TimeSpan.FromMilliseconds(100),
                _componentEvaluator1,
                new List<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>(),
                _criteriaFactory1);


            _expectation2 = new ExpectationEvaluator(
                TimeSpan.FromMilliseconds(200),
                _componentEvaluator2,
                new List<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>(),
                _criteriaFactory2);

            var sut = new Verifier(new List<ExpectationEvaluator> { _expectation1, _expectation2 }, shortCircuit: true);

            var result = await sut.VerifyAsync();

            Assert.NotNull(result);
            Assert.True(result.ExpectationsMet);
            Assert.IsNotEmpty(result.ExpectationResults);
            Assert.IsTrue(result.ExpectationResults.All(x => x.Met ?? false));
            Assert.IsTrue(result.ExpectationResults.All(x => x.Completed));
        }

        [Test]
        public async Task Given_AnExpectationNotMetWhileShortCircuit_When_Verify_Then_ExpectationsNotMet()
        {
            _expectation1 = new ExpectationEvaluator(
                TimeSpan.FromMilliseconds(100),
                _componentEvaluator1,
                new List<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>(),
                _criteriaFactory1);


            _expectation2 = new ExpectationEvaluator(
                TimeSpan.FromMilliseconds(200),
                _componentEvaluator2,
                new List<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>(),
                _criteriaFactory2);

            A.CallTo(() => _criteriaEvaluator1.Met).Returns(false);

            var sut = new Verifier(new List<ExpectationEvaluator> { _expectation1, _expectation2 }, shortCircuit: true);

            var result = await sut.VerifyAsync();

            Assert.NotNull(result);
            Assert.False(result.ExpectationsMet);
            Assert.IsNotEmpty(result.ExpectationResults);
            Assert.IsFalse(result.ExpectationResults.All(x => x.Met ?? false));
            Assert.IsTrue(result.ExpectationResults.Any(x => x.Met == false));
            Assert.IsFalse(result.ExpectationResults.All(x => x.Completed));
            Assert.IsTrue(result.ExpectationResults.Any(x => x.Completed));
        }

        [Test]
        public async Task Given_VerifierAlreadyVerified_When_Verify_Then_SameResult()
        {
            var sut = new Verifier(new List<ExpectationEvaluator> { _expectation1, _expectation2 });

            var result = await sut.VerifyAsync();

            var result2 = await sut.VerifyAsync();

            Assert.AreSame(result, result2);
        }

        [Test]
        public async Task Given_Verifying_When_Verify_Then_Throw()
        {
            var sut = new Verifier(new List<ExpectationEvaluator> { _expectation1, _expectation2 });

            var result = Task.Run(async () => await sut.VerifyAsync());
            await Task.Delay(100);

            Assert.ThrowsAsync<InvalidOperationException>(() => sut.VerifyAsync());

            await result;   // Unaffected, let it finish to clean up
        }

        [Test]
        public async Task Given_Verified_When_Dispose_Then_DependenciesNotRedisposed()
        {
            var sut = new Verifier(new List<ExpectationEvaluator> { _expectation1, _expectation2 });

            await sut.VerifyAsync();

            Assume.That(_componentEvaluator1.DisposeCalled);
            Assume.That(_componentEvaluator2.DisposeCalled);
            _componentEvaluator1.ResetCalls();
            _componentEvaluator2.ResetCalls();

            await sut.DisposeAsync();


            Assert.IsFalse(_componentEvaluator1.DisposeCalled);
            Assert.IsFalse(_componentEvaluator2.DisposeCalled);
        }

        [Test]
        public async Task Given_Verifier_When_Dispose_Then_ExpectationsDisposed()
        {
            var sut = new Verifier(new List<ExpectationEvaluator> { _expectation1, _expectation2 });

            await sut.DisposeAsync();

            Assert.IsTrue(_componentEvaluator1.DisposeCalled);
            Assert.IsTrue(_componentEvaluator2.DisposeCalled);
        }

        [Test]
        public async Task Given_VerifierAlreadyDisposed_When_Dispose_Then_Nothing()
        {
            var sut = new Verifier(new List<ExpectationEvaluator> { _expectation1, _expectation2 });

            await sut.DisposeAsync();
            _componentEvaluator1.ResetCalls();
            _componentEvaluator2.ResetCalls();

            await sut.DisposeAsync();
            Assert.IsFalse(_componentEvaluator1.DisposeCalled);
            Assert.IsFalse(_componentEvaluator2.DisposeCalled);
        }

        [Test]
        public async Task Given_VerifierAlreadyDisposed_When_Verify_Then_Throws()
        {
            var sut = new Verifier(new List<ExpectationEvaluator> { _expectation1, _expectation2 });

            await sut.DisposeAsync();
            _componentEvaluator1.ResetCalls();
            _componentEvaluator2.ResetCalls();

            Assert.ThrowsAsync<ObjectDisposedException>(() => sut.VerifyAsync());
        }
    }
}
