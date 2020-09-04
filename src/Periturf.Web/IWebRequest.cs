using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Collections.Immutable;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Web
{
    public interface IWebRequest
    {
        string Scheme { get; }
        
        bool IsHttps { get; }
        
        string Protocol { get; }
        
        HostString Host { get; }
        
        PathString PathBase { get; }
        
        PathString Path { get; }
        
        QueryString QueryString { get; }
        
        IQueryCollection Query { get; }
        
        ImmutableDictionary<string, StringValues> Headers { get; }

        string Method { get; }
        
        string ContentType { get; }
        
        long? ContentLength { get; }
        
        IRequestCookieCollection Cookies { get; }

        Stream Body { get; }

        PipeReader BodyReader { get; }

        Task<TContent> GetContentAsync<TContent>(CancellationToken ct = default);
    }
}
