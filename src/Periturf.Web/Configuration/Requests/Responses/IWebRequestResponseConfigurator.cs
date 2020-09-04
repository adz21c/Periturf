using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Periturf.Web.Configuration.Requests.Responses
{
    public interface IWebRequestResponseConfigurator
    {
        HttpStatusCode? StatusCode { get; set; }

        string? ContentType { get; set; }

        void AddHeader(string name, StringValues values);

        void AddCookie(string key, string value, CookieOptions? options = null);
        
        void SetBodySpecification(IWebRequestResponseBodySpecification spec);
    }
}
