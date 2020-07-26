using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Periturf.Web.Configuration;
using System.Collections.Generic;

namespace Periturf.Web
{
    class WebRequest : IWebRequest
    {
        private readonly HttpRequest _request;

        public WebRequest(HttpRequest request)
        {
            _request = request;
        }

        public PathString Path => _request.Path;

        public string Method => _request.Method;

        public IDictionary<string, StringValues> Headers => _request.Headers;
    }
}
