using System;
using System.Collections.Generic;
using System.Text;

namespace Periturf.Verify
{
    class ExpectationSpecification : IExpectationConfigurator
    {
        private readonly List<ExpectationSpecification> _expectationSpecifications = new List<ExpectationSpecification>();
        private readonly List<ExpectationConstraintSpecification> _expectationConstraintSpecifications = new List<ExpectationConstraintSpecification>();

        public void Constraint(Action<IExpectationConstraintConfigurator> config)
        {
            var spec = new ExpectationConstraintSpecification();
            config(spec);
            _expectationConstraintSpecifications.Add(spec);
        }

        public void Then(Action<IExpectationConfigurator> config)
        {
            var spec = new ExpectationSpecification();
            config(spec);
            _expectationSpecifications.Add(spec);
        }
    }
}
