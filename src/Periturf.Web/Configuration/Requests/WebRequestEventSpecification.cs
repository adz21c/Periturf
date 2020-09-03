using Periturf.Events;
using Periturf.Web.Configuration.Requests.Predicates;
using Periturf.Web.Configuration.Requests.Responses;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Periturf.Web.Configuration.Requests
{
    class WebRequestEventSpecification : EventSpecification<IWebRequest>, IWebRequestEventConfigurator
    {
        private readonly List<IWebRequestPredicateSpecification> _predicates = new List<IWebRequestPredicateSpecification>();
        private IWebRequestResponseSpecification? _responseSpecification;

        public WebRequestEventSpecification(IEventHandlerFactory eventHandlerFactory) : base(eventHandlerFactory)
        { }

        public void AddPredicateSpecification(IWebRequestPredicateSpecification spec)
        {
            _predicates.Add(spec ?? throw new ArgumentNullException(nameof(spec)));
        }

        public void SetResponseSpecification(IWebRequestResponseSpecification spec)
        {
            _responseSpecification = spec ?? throw new ArgumentNullException(nameof(spec));
        }

        public WebConfiguration Build()
        {
            Debug.Assert(_responseSpecification != null, "ResponseSpecification != null");

            return new WebConfiguration(
                _predicates.Select(x => x.Build()).ToList(),
                _responseSpecification.BuildFactory(),
                CreateHandler());
        }
    }
}
