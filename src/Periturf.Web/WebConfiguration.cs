using Microsoft.AspNetCore.Http;
using Periturf.Events;
using Periturf.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Web
{
    class WebConfiguration
    {
        private readonly List<Func<IWebRequest, bool>> _predicates;
        private readonly Func<IWebResponse, Task> _responseFactory;
        private readonly IEventHandler<IWebRequest> _handlers;

        public WebConfiguration(
            List<Func<IWebRequest, bool>> predicates,
            Func<IWebResponse, Task> responseFactory,
            IEventHandler<IWebRequest> handlers)
        {
            Debug.Assert(predicates?.Any() == true, "predicates?.Any() == true");
            Debug.Assert(responseFactory != null, "responseFactory != null");
            Debug.Assert(handlers != null, "handlers != null");

            _predicates = predicates;
            _responseFactory = responseFactory;
            _handlers = handlers;
        }

        public bool Matches(HttpContext ctx) => _predicates.Any(x => x(new WebRequest(ctx.Request)));

        public async Task WriteResponse(HttpContext ctx)
        {
            await _responseFactory(new WebResponse(ctx.Response));
            await ctx.Response.CompleteAsync();
        }

        public Task ExecuteHandlers(HttpContext ctx) => _handlers.ExecuteHandlersAsync(new WebRequest(ctx.Request), CancellationToken.None);
    }
}
