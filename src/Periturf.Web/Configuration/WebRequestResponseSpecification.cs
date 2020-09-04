using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.Web.Configuration
{
    class WebRequestResponseSpecification : IWebRequestResponseConfigurator
    {
        public HttpStatusCode? StatusCode { get; set; }
        public string? ContentType { get; set; }
        public long? ContentLength { get; set; }
        public List<WebCookie> Cookies { get; } = new List<WebCookie>();
        public Dictionary<string, StringValues> Headers { get; } = new Dictionary<string, StringValues>();
        public string? StringBody { get; private set; }
        public object? ObjectBody { get; private set; }
        public Func<IWebResponse, Task> DynamicWriter { get; private set; }

        public void AddCookie(string key, string value, CookieOptions? options = null)
        {
            Cookies.Add(new WebCookie
            {
                Key = key,
                Value = value,
                Options = options
            });
        }

        public void AddHeader(string name, StringValues values)
        {
            Headers[name] = values;
        }

        public void SetBody(object body)
        {
            StringBody = null;
            ObjectBody = body;
        }

        public void SetBody(string body)
        {
            StringBody = body;
            ObjectBody = null;
        }

        public void Dynamic(Func<IWebResponse, Task> writer)
        {
            DynamicWriter = writer;
        }

        public Func<IWebResponse, Task> BuildFactory()
        {
            return async response =>
            {
                if (StatusCode.HasValue)
                    response.StatusCode = StatusCode.Value;
                if (ContentLength.HasValue)
                    response.ContentLength = ContentLength.Value;
                if (ContentType != null)
                    response.ContentType = ContentType;

                foreach (var header in Headers)
                    response.AddHeader(header.Key, header.Value);

                foreach (var cookie in Cookies)
                    response.AddCookie(cookie.Key, cookie.Value, cookie.Options);

                if (DynamicWriter != null)
                    await DynamicWriter(response);

                if (ObjectBody != null)
                    await response.SetBodyAsync(ObjectBody);
                else if (StringBody != null)
                    await response.WriteBodyAsync(StringBody);
            };
        }
    }
}
