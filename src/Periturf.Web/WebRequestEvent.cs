using Periturf.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Periturf.Web
{
    [ExcludeFromCodeCoverage]
    class WebRequestEvent : IWebRequestEvent
    {
        public WebRequestEvent(string traceIdentifier, IWebRequest requestHeader)
        {
            TraceIdentifier = traceIdentifier;
            Request = requestHeader;
        }

        public string TraceIdentifier { get; }

        public IWebRequest Request { get; }
    }
}
