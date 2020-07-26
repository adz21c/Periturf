using Periturf.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Periturf.Web.Configuration
{
    class WebRequestEventSpecification : EventSpecification<IWebRequest>, IWebRequestEventConfigurator
    {
        public WebRequestEventSpecification(IEventHandlerFactory eventHandlerFactory) : base(eventHandlerFactory)
        { }

        public void AddPredicateSpecification(IWebRequestPredicateSpecification spec)
        {
            Predicates.Add(spec ?? throw new ArgumentNullException(nameof(spec)));
        }

        public void SetResponseSpecification(IWebRequestResponseSpecification spec)
        {
            ResponseSpecification = spec;
        }

        public List<IWebRequestPredicateSpecification> Predicates { get; } = new List<IWebRequestPredicateSpecification>();

        public IWebRequestResponseSpecification? ResponseSpecification { get; private set; }

        public WebConfiguration Build()
        {
            Debug.Assert(ResponseSpecification != null, "ResponseSpecification != null");

            return new WebConfiguration(
                Predicates.Select(x => x.Build()).ToList(),
                ResponseSpecification.BuildFactory(),
                CreateHandler());
        }
    }
}
