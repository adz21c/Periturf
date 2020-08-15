using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipelines;
using System.Net;
using System.Threading.Tasks;

namespace Periturf.Web
{
    public interface IWebResponse
    {
        HttpStatusCode StatusCode { get; set; }

        string ContentType { get; set; }

        void AddHeader(string name, IEnumerable<object> values);

        void AddCookie(string key, string value, CookieOptions? options = null);

        Stream BodyStream { get; }

        PipeWriter BodyWriter { get; }

        Task WriteBodyAsync(string body);
    }
}
