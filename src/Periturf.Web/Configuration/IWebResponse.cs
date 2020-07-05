using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Periturf.Web.Configuration
{
    public interface IWebResponse
    {
        HttpStatusCode StatusCode { set; }

        string ContentType { set; }

        long? ContentLength { set; }

        void AddHeader(string name, IEnumerable<object> values);

        void AddCookie(string key, string value, CookieOptions? options = null);

        Task SetBodyAsync(object body);

        Task WriteBodyAsync(string body);
    }
}
