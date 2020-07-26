using System;
using System.Threading.Tasks;

namespace Periturf.Web.Configuration.Requests.Responses
{
    public interface IWebRequestResponseSpecification
    {
        Func<IWebResponse, Task> BuildFactory();
    }
}