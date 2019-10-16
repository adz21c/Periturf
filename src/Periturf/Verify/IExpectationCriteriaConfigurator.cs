namespace Periturf.Verify
{
    public interface IExpectationCriteriaConfigurator
    {
        void AddSpecification(IExpectationCriteriaSpecification specification);
    }
}