using NUnit.Framework;
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Text;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ExpectationEvaluatorMultiConstraintTests
    {
        private readonly ConditionIdentifier _condition1 = new ConditionIdentifier(Guid.NewGuid());
        private readonly ConditionIdentifier _condition2 = new ConditionIdentifier(Guid.NewGuid());
        private ExpectationEvaluator _sut;

        [SetUp]
        public void SetUp()
        {
            _sut = new ExpectationEvaluator(
                new List<ExpectationConstraintEvaluator>
                {
                    new ExpectationConstraintEvaluator(_condition1),
                    new ExpectationConstraintEvaluator(_condition2)
                },
                null);
        }

        [Test]
        public void Given_MatchingInputs_When_Evaluate_Then_NotMet()
        {
            var feedInstance = new FeedConditionInstance(
                _condition1,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));

            var result = _sut.Evaluate(feedInstance);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Met, Is.False);
        }

        [Test]
        public void Given_MultipleConstraint_When_Evaluate_Then_MetWhenAllMet()
        {
            var feedInstance = new FeedConditionInstance(
                _condition2,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID2"));

            var result = _sut.Evaluate(feedInstance);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Met, Is.False);

            var feedInstance2 = new FeedConditionInstance(
                _condition1,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));

            var result2 = _sut.Evaluate(feedInstance2);

            Assert.That(result2, Is.Not.Null);
            Assert.That(result2.Met, Is.True);
        }
    }
}
