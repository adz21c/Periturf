using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Periturf.Verify
{
    public interface IExpectationConfigurator
    {
        IExpectationConfigurator Description(string description);

        IExpectationConfigurator Where(Action<IExpectationFilterConfigurator> config);

        IExpectationConfigurator Must(Action<IExpectationCriteriaConfigurator> config);
    }
}
