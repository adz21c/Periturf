using Microsoft.AspNetCore.Http;
using Periturf.Clients;
using Periturf.Components;
using Periturf.Configuration;
using Periturf.Events;
using Periturf.Verify;
using Periturf.Web.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public TSpecification CreateConfigurationSpecification<TSpecification>(IEventHandlerFactory eventHandlerFactory) where TSpecification : IConfigurationSpecification
        {
            return (TSpecification)(object)new WebComponentSpecification(_configurations, eventHandlerFactory);
        }

        public async Task ProcessAsync(HttpContext context)
        {
            var request = new WebRequest(context.Request);
            var config = ((IEnumerable<WebConfiguration>)_configurations)
                .Reverse()
                .FirstOrDefault(x => x.Matches(request));
            if (config == null)
            {
                context.Response.StatusCode = 404;
                return;
            }

            var response = new WebResponse(context.Response);
            await config.WriteResponse(response);
            await context.Response.CompleteAsync();

            await config.ExecuteHandlers(request);
        }
    }
}
