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
        private IExpectationCriteriaEvaluator _criteriaEvaluator1;
        private IExpectationCriteriaEvaluatorFactory _criteriaFactory1;
        private ExpectationEvaluator _expectation1;
        private IExpectationCriteriaEvaluator _criteriaEvaluator2;
        private IExpectationCriteriaEvaluatorFactory _criteriaFactory2;
        private ExpectationEvaluator _expectation2;

        [SetUp]
        public void SetUp()
        {
            var componentEvaluator1 = new MockComponentEvaluator(TimeSpan.FromMilliseconds(1000), null);

            _criteriaEvaluator1 = A.Fake<IExpectationCriteriaEvaluator>();
            A.CallTo(() => _criteriaEvaluator1.Met).Returns(true);
            A.CallTo(() => _criteriaEvaluator1.Completed).Returns(true);

            _criteriaFactory1 = A.Fake<IExpectationCriteriaEvaluatorFactory>();
            A.CallTo(() => _criteriaFactory1.CreateInstance()).Returns(_criteriaEvaluator1);
            A.CallTo(() => _criteriaFactory1.Timeout).Returns(TimeSpan.FromMilliseconds(500));

            _expectation1 = new ExpectationEvaluator(
                componentEvaluator1,
                new List<Func<IAsyncEnumerable<ConditionInstance>, IAsyncEnumerable<ConditionInstance>>>(),
                _criteriaFactory1);


            var componentEvaluator2 = new MockComponentEvaluator(TimeSpan.FromMilliseconds(1000), null);

            _criteriaEvaluator2 = A.Fake<IExpectationCriteriaEvaluator>();
            A.CallTo(() => _criteriaEvaluator2.Met).Returns(true);
            A.CallTo(() => _criteriaEvaluator2.Completed).Returns(true);

            _criteriaFactory2 = A.Fake<IExpectationCriteriaEvaluatorFactory>();
            A.CallTo(() => _criteriaFactory2.CreateInstance()).Returns(_criteriaEvaluator2);
            A.CallTo(() => _criteriaFactory2.Timeout).Returns(TimeSpan.FromMilliseconds(500));

            _expectation2 = new ExpectationEvaluator(
                componentEvaluator2,
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
            A.CallTo(() => _criteriaFactory1.Timeout).Returns(TimeSpan.FromMilliseconds(100));
            A.CallTo(() => _criteriaFactory2.Timeout).Returns(TimeSpan.FromMilliseconds(200));
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
            A.CallTo(() => _criteriaEvaluator1.Met).Returns(false);
            A.CallTo(() => _criteriaEvaluator1.Completed).Returns(true);
            A.CallTo(() => _criteriaFactory1.Timeout).Returns(TimeSpan.FromMilliseconds(100));
            A.CallTo(() => _criteriaEvaluator2.Completed).Returns(false);
            A.CallTo(() => _criteriaFactory2.Timeout).Returns(TimeSpan.FromMilliseconds(200));
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
    }
}
