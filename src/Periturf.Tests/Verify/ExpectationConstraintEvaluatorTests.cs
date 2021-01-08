using NUnit.Framework;
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Text;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ExpectationConstraintEvaluatorTests
    {
        private readonly ConditionIdentifier _condition1 = new ConditionIdentifier(Guid.NewGuid());
        private readonly ConditionIdentifier _condition2 = new ConditionIdentifier(Guid.NewGuid());

        [Test]
        public void Given_Inputs_When_Ctor_Then_Ready()
        {
            var sut = new ExpectationConstraintEvaluator(_condition1);

            Assert.That(sut.Met, Is.False);
        }

        [Test]
        public void Given_MatchingCondition_When_Evaluate_Then_Met()
        {
            var sut = new ExpectationConstraintEvaluator(_condition1);

            Assume.That(sut.Met, Is.False);

            var feedInstance = new FeedConditionInstance(
                _condition1,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));

            sut.Evaluate(feedInstance);

            Assert.That(sut.Met, Is.True);
        }

        [Test]
        public void Given_NotMatchingCondition_When_Evaluate_Then_Met()
        {
            var sut = new ExpectationConstraintEvaluator(_condition1);

            Assume.That(sut.Met, Is.False);

            var feedInstance = new FeedConditionInstance(
                _condition2,
                new ConditionInstance(TimeSpan.FromMilliseconds(100), "ID1"));

            sut.Evaluate(feedInstance);

            Assert.That(sut.Met, Is.False);
        }
    }
}
