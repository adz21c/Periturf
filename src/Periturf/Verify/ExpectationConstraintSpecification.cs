using System;
using System.Diagnostics;

namespace Periturf.Verify
{
    class ExpectationConstraintSpecification : IExpectationConstraintConfigurator
    {
        private ConditionIdentifier? _conditionIdentifier;

        public IExpectationConstraintConfigurator Condition(ConditionIdentifier conditionIdentifier)
        {
            _conditionIdentifier = conditionIdentifier;
            return this;
        }

        internal ExpectationConstraintEvaluator Build()
        {
            Debug.Assert(_conditionIdentifier != null, "_conditionIdentifier != null");

            return new ExpectationConstraintEvaluator(
                _conditionIdentifier);
        }
    }
}
