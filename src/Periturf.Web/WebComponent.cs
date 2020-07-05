using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Periturf.Clients;
using Periturf.Components;
using Periturf.Configuration;
using Periturf.Events;
using Periturf.Verify;
using Periturf.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Periturf.Web
{
    class WebComponent : IComponent
    {
        private readonly List<WebConfiguration> _configurations = new List<WebConfiguration>();

        public IComponentClient CreateClient()
        {
            throw new NotImplementedException();
        }

        public TComponentConditionBuilder CreateConditionBuilder<TComponentConditionBuilder>() where TComponentConditionBuilder : IComponentConditionBuilder
        {
            throw new NotImplementedException();
        }

        public TSpecification CreateConfigurationSpecification<TSpecification>(IEventResponseContextFactory eventResponseContextFactory) where TSpecification : IConfigurationSpecification
        {
            return (TSpecification)(object)new WebComponentSpecification(_configurations);
        }

        public async Task ProcessAsync(HttpContext context)
        {
            var config = _configurations.FirstOrDefault(x => x.Matches(context.Request));
            if (config == null)
                return;

            await config.WriteResponse(context);

            await config.ExecuteHandlers(context.Request);
        }
    }

    class WebConfiguration
    {
        private readonly List<Func<HttpRequest, bool>> _predicates;
        private readonly Func<IWebResponse, Task> _responseFactory;
        private readonly List<Func<HttpRequest, Task>> _handlers;

        public WebConfiguration(
            List<Func<HttpRequest, bool>> predicates,
            Func<IWebResponse, Task> responseFactory,
            List<Func<HttpRequest, Task>> handlers)
        {
            Debug.Assert(predicates?.Any() == true, "predicates?.Any() == true");
            Debug.Assert(responseFactory != null, "responseFactory != null");
            Debug.Assert(handlers != null, "handlers != null");

            _predicates = predicates;
            _responseFactory = responseFactory;
            _handlers = handlers;
        }

        public bool Matches(HttpRequest request) => _predicates.Any(x => x(request));

        public async Task WriteResponse(HttpContext ctx)
        {
            await _responseFactory(new WebResponse(ctx.Response));
            await ctx.Response.CompleteAsync();
        }

        public Task ExecuteHandlers(HttpRequest request) => Task.WhenAll(_handlers.Select(x => x(request)));
    }

    class WebResponse : IWebResponse
    {
        private readonly HttpResponse _response;

        public WebResponse(HttpResponse response)
        {
            _response = response;
        }

        public HttpStatusCode StatusCode { set => _response.StatusCode = (int)value; }
        public string ContentType { set => _response.ContentType = value; }
        public long? ContentLength { set => _response.ContentLength = value; }

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

        public async Task SetBodyAsync(object body)
        {
            _response.ContentType = "application/json";
            await _response.StartAsync();
            await _response.WriteAsync(JsonConvert.SerializeObject(body));
        }

        public async Task WriteBodyAsync(string body)
        {
            if (!_response.HasStarted)
                await _response.StartAsync();

            await _response.WriteAsync(body);
        }
    }
}
