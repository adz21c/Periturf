using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.Web.Configuration.Requests.Responses
{
    public interface IWebRequestResponseBodySpecification
    {
        Func<IWebResponse, Task> Build();
    }
}
