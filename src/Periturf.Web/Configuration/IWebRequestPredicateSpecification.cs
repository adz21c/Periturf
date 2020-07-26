using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.Web.Configuration
{
    public interface IWebRequestPredicateSpecification
    {
        Func<IWebRequest, bool> Build();
    }
}
