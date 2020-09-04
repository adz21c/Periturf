using System;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Periturf.Web.Configuration.Requests.Responses
{
    class XmlWebWriterSpecification : IWebWriterSpecification, IXmlWebWriterConfigurator
    {
        public Func<IWebResponse, object?, Task> Build()
        {
            return (response, obj) =>
            {
                response.ContentType = "application/xml";
        
                if (obj == null)
                    return Task.CompletedTask;

                var serializer = new XmlSerializer(obj.GetType());
                
                serializer.Serialize(response.BodyStream, obj);

                return Task.CompletedTask;
            };
        }
    }
}
