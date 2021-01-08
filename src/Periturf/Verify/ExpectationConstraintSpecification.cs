namespace Periturf.Verify
{
    class ExpectationConstraintSpecification : IExpectationConstraintConfigurator
    {
        private bool _not = false;
        //private IExpectationTimeConstraintSpecification? _timeConstraintSpecification;
        private ConditionIdentifier? _conditionIdentifier;

        public IExpectationConstraintConfigurator Not()
        {
            _not = true;
            return this;
        }

        //public IExpectationConstraintConfigurator SetTimeConstraintSpecification(IExpectationTimeConstraintSpecification spec)
        //{
        //    _timeConstraintSpecification = spec;
        //    return this;
        //}

        public IExpectationConstraintConfigurator Condition(ConditionIdentifier conditionIdentifier)
        {
            _conditionIdentifier = conditionIdentifier;
            return this;
        }
    }
}
