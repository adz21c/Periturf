using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Periturf.Web.Configuration
{
    public interface IWebRequestResponseConfigurator
    {
        HttpStatusCode? StatusCode { set; }

        string? ContentType { set; }

        long? ContentLength { set; }

        void AddHeader(string name, StringValues values);

        void AddCookie(string key, string value, CookieOptions? options = null);

        void SetBody(object body);
        
        void SetBody(string body);

        void Dynamic(Func<IWebResponse, Task> writer);
    }
}
