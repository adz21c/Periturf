using System;
using System.Collections.Generic;
using System.Text;

namespace Periturf.Verify
{
    class VerificationContext : IVerificationContext
    {
        private readonly List<(IComponentConditionSpecification ComponentSpec, ExpectationSpecification ExpectationSpec)> _specs = new List<(IComponentConditionSpecification, ExpectationSpecification)>();

        public void Expect(IComponentConditionSpecification conditionSpecification, Action<IExpectationConfigurator> config)
        {
            var expecationSpec = new ExpectationSpecification();
            config(expecationSpec);

            _specs.Add(
                (
                    conditionSpecification ?? throw new ArgumentNullException(nameof(conditionSpecification)),
                    expecationSpec
                ));
        }

        public TComponentConditionBuilder GetComponentConditionBuilder<TComponentConditionBuilder>(string componentName) where TComponentConditionBuilder : IComponentConditionBuilder
        {
            throw new NotImplementedException();
        }
    }

    class ExpectationSpecification : IExpectationConfigurator, IExpectationCriteriaConfigurator, IExpectationFilterConfigurator
    {
        private readonly List<IExpectationCriteriaSpecification> _criteriaSpecifications = new List<IExpectationCriteriaSpecification>();
        private readonly List<IExpectationFilterSpecification> _filterSpecifications = new List<IExpectationFilterSpecification>();
        private string? _description = null;

        IExpectationConfigurator IExpectationConfigurator.Description(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentOutOfRangeException(nameof(description));
            
            _description = description;
            return this;
        }

        IExpectationConfigurator IExpectationConfigurator.Must(Action<IExpectationCriteriaConfigurator> config)
        {
            config(this);
            return this;
        }

        IExpectationConfigurator IExpectationConfigurator.Where(Action<IExpectationFilterConfigurator> config)
        {
            config(this);
            return this;
        }

        void IExpectationCriteriaConfigurator.AddSpecification(IExpectationCriteriaSpecification specification)
        {
            _criteriaSpecifications.Add(specification ?? throw new ArgumentNullException(nameof(specification)));
        }

        void IExpectationFilterConfigurator.AddSpecification(IExpectationFilterSpecification specification)
        {
            _filterSpecifications.Add(specification ?? throw new ArgumentNullException(nameof(specification)));
        }
    }
}
