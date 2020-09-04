using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;

namespace Periturf.Web.Configuration.Requests.Responses
{
    class WebRequestResponseSpecification : IWebRequestResponseSpecification, IWebRequestResponseConfigurator
    {
        private IWebRequestResponseBodySpecification? _bodySpecification;

        public HttpStatusCode? StatusCode { get; set; }
        public string? ContentType { get; set; }
        public List<WebCookie> Cookies { get; } = new List<WebCookie>();
        public Dictionary<string, StringValues> Headers { get; } = new Dictionary<string, StringValues>();
        
        public void AddCookie(string key, string value, CookieOptions? options = null)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            Cookies.Add(new WebCookie(key, value ?? string.Empty)
            {
                Options = options
            });
        }

        public void AddHeader(string name, StringValues values)
        {
            Headers[name] = values;
        }

        public void SetBodySpecification(IWebRequestResponseBodySpecification spec)
        {
            _bodySpecification = spec ?? throw new ArgumentNullException(nameof(spec));
        }

        public Func<IWebResponse, Task> BuildFactory()
        {
            Debug.Assert(_bodySpecification != null, "_bodySpecification != null");
            var bodyWriter = _bodySpecification.Build();

            return async response =>
            {
                if (StatusCode.HasValue)
                    response.StatusCode = StatusCode.Value;
                if (ContentType != null)
                    response.ContentType = ContentType;

                foreach (var header in Headers)
                    response.AddHeader(header.Key, header.Value);

                foreach (var cookie in Cookies)
                    response.AddCookie(cookie.Key, cookie.Value, cookie.Options);

                await bodyWriter(response);
            };
        }
    }
}
