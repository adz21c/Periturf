namespace Periturf.Verify
{
    public interface IExpectationConstraintConfigurator
    {
        IExpectationConstraintConfigurator Condition(ConditionIdentifier conditionIdentifier);
    }
}
