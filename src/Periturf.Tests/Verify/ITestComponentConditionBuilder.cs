using Periturf.Verify;

namespace Periturf.Tests.Verify
{
    public interface ITestComponentConditionBuilder : IComponentConditionBuilder
    {
        IComponentConditionSpecification CreateCondition();
    }
}