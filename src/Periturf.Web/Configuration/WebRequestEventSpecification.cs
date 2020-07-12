using Periturf.Events;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Periturf.Web.Configuration
{
    class WebRequestEventSpecification : EventSpecification<IWebRequest>, IWebRequestEventConfigurator
    {
        public WebRequestEventSpecification(IEventHandlerFactory eventHandlerFactory) : base(eventHandlerFactory)
        { }

        public void Predicate(Func<IWebRequest, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            Predicates.Add(predicate);
        }

        public void Response(Action<IWebRequestResponseConfigurator> config)
        {
            ResponseSpecification = new WebRequestResponseSpecification();
            config?.Invoke(ResponseSpecification);
        }

        public List<Func<IWebRequest, bool>> Predicates { get; } = new List<Func<IWebRequest, bool>>();

        public WebRequestResponseSpecification? ResponseSpecification { get; private set; }
    }
}
