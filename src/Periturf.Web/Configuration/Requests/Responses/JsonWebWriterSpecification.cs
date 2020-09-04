using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Periturf.Web.Configuration.Requests.Responses
{
    class JsonWebWriterSpecification : IWebWriterSpecification, IJsonWebWriterConfigurator
    {
        private JsonSerializerOptions? _options = null;

        public void Options(JsonSerializerOptions? options)
        {
            _options = options;
        }

        public Func<IWebResponse, object?, Task> Build()
        {
            return async (response, obj) =>
            {
                response.ContentType = "application/json";
                var serialized = _options == null ? JsonSerializer.Serialize(obj) : JsonSerializer.Serialize(obj, _options);
                await response.WriteBodyAsync(serialized);
            };
        }
    }
}
