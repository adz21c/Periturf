using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace Periturf.Web.Configuration
{
    public interface IWebRequest
    {
        PathString Path { get; }

        string Method { get; }

        IDictionary<string, StringValues> Headers { get; }
    }
}
