using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Periturf.Web.Configuration;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Web
{
    [ExcludeFromCodeCoverage]
    class WebRequest : IWebRequest
    {
        public WebRequest(HttpRequest request)
        {
            Scheme = request.Scheme;
            IsHttps = request.IsHttps;
            Protocol = request.Protocol;
            Host = request.Host;
            PathBase = request.PathBase;
            Path = request.Path;
            QueryString = request.QueryString;
            Query = request.Query;
            Headers = request.Headers.ToImmutableDictionary();
            Method = request.Method;
            ContentType = request.ContentType;
            ContentLength = request.ContentLength;
            Cookies = request.Cookies;
            Body = request.Body;
            BodyReader = request.BodyReader;
        }

        public string Scheme { get; }

        public bool IsHttps { get; }

        public string Protocol { get; }

        public HostString Host { get; }

        public PathString PathBase { get; }

        public PathString Path { get; }

        public QueryString QueryString { get; }

        public IQueryCollection Query { get; }

        public ImmutableDictionary<string, StringValues> Headers { get; }

        public string Method { get; }

        public string ContentType { get; }

        public long? ContentLength { get; }

        public IRequestCookieCollection Cookies { get; }

        public Stream Body { get; }

        public PipeReader BodyReader { get; }

        public Task<TContent> GetContentAsync<TContent>(CancellationToken ct = default)
        {
            throw new System.NotImplementedException();
        }
    }
}
