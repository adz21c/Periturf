using Periturf.Web.Configuration.Requests.Responses;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Periturf
{
    [ExcludeFromCodeCoverage]
    public static class WebRequestResponseExtensions
    {
        public static void ObjectBody(this IWebRequestResponseConfigurator configurator, Action<IWebRequestResponseObjectConfigurator>? config)
        {
            var spec = new WebRequestResponseObjectSpecification();
            config?.Invoke(spec);
            configurator.SetBodySpecification(spec);
        }

        public static void JsonSerializer(this IWebRequestResponseObjectConfigurator configurator, Action<IJsonWebWriterConfigurator>? config = null)
        {
            var spec = new JsonWebWriterSpecification();
            config?.Invoke(spec);
            configurator.SetWriterSpecification(spec);
        }

        public static void XmlSerializer(this IWebRequestResponseObjectConfigurator configurator, Action<IXmlWebWriterConfigurator>? config = null)
        {
            var spec = new XmlWebWriterSpecification();
            config?.Invoke(spec);
            configurator.SetWriterSpecification(spec);
        }
    }
}
