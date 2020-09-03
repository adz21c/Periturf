using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Periturf.Web.Configuration;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Periturf.Web
{
    [ExcludeFromCodeCoverage]
    class WebResponse : IWebResponse
    {
        private readonly HttpResponse _response;

        public WebResponse(HttpResponse response)
        {
            _response = response;
        }

        public HttpStatusCode StatusCode { get => (HttpStatusCode) _response.StatusCode; set => _response.StatusCode = (int)value; }
        public string ContentType { get => _response.ContentType; set => _response.ContentType = value; }
        public long? ContentLength { get => _response.ContentLength; set => _response.ContentLength = value; }


        public void AddCookie(string key, string value, CookieOptions? options = null)
        {
            if (options == null)
                _response.Cookies.Append(key, value);
            else
                _response.Cookies.Append(key, value, options);
        }

        public void AddHeader(string name, IEnumerable<object> values)
        {
            _response.Headers.AppendList(name, values.ToList());
        }

        public Stream BodyStream => _response.Body;

        public PipeWriter BodyWriter => _response.BodyWriter;

        public async Task WriteBodyAsync(string body)
        {
            if (!_response.HasStarted)
                await _response.StartAsync();

            await _response.WriteAsync(body);
        }
    }
}
