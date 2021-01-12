using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Periturf.Verify
{
    class ExpectationSpecification : IExpectationConfigurator
    {
        private ExpectationSpecification? _expectationSpecifications;
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
            _expectationSpecifications = spec;
        }

        public ExpectationEvaluator Build()
        {
            return new ExpectationEvaluator(
                _expectationConstraintSpecifications.Select(x => x.Build()).ToList(),
                _expectationSpecifications?.Build());
        }
    }
}
