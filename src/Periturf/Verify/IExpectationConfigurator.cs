using System;

namespace Periturf.Verify
{
    public interface IExpectationConfigurator
    {
        void Constraint(Action<IExpectationConstraintConfigurator> config);

        void Then(Action<IExpectationConfigurator> config);
    }
}
