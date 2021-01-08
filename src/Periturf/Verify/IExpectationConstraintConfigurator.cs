namespace Periturf.Verify
{
    public interface IExpectationConstraintConfigurator
    {
        IExpectationConstraintConfigurator Not();

        //void SetTimeConstraintSpecification(IExpectationTimeConstraintSpecification spec);

        IExpectationConstraintConfigurator Condition(ConditionIdentifier conditionIdentifier);
    }
}
