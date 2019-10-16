using System;

namespace Periturf.Verify
{
    public interface IExpectationFilterConfigurator
    {
        void AddSpecification(IExpectationFilterSpecification specification);
    }
}