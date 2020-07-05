using Microsoft.AspNetCore.Http;
using System;

namespace Periturf.Web.Configuration
{
    class WebRequestEventSpecification : IWebRequestEventConfigurator
    {
        public void Predicate(Func<HttpRequest, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public void Response(Action<IWebRequestResponseConfigurator> config)
        {
            ResponseSpecification = new WebRequestResponseSpecification();
            config?.Invoke(ResponseSpecification);
        }

        public WebRequestResponseSpecification? ResponseSpecification { get; private set; }
    }
}
