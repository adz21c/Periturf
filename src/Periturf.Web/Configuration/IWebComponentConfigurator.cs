using System;

namespace Periturf.Web.Configuration
{
    public interface IWebComponentConfigurator
    {
        void OnRequest(Action<IWebRequestEventConfigurator> config);
    }
}
