using System;
using System.Threading.Tasks;

namespace Periturf.Web.Configuration
{
    public interface IWebRequestResponseSpecification
    {
        Func<IWebResponse, Task> BuildFactory();
    }
}