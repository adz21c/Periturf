using Microsoft.AspNetCore.Http;
using Periturf.Events;
using Periturf.Web.Configuration.Requests.Predicates;
using Periturf.Web.Configuration.Requests.Responses;
using System;

namespace Periturf.Web.Configuration.Requests
{
    public interface IWebRequestEventConfigurator : IEventConfigurator<IWebRequest>
    {
        void AddPredicateSpecification(IWebRequestPredicateSpecification spec);

        void SetResponseSpecification(IWebRequestResponseSpecification spec);
    }
}
