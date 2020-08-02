using System.Text.Json;

namespace Periturf.Web.Configuration.Requests.Responses
{
    public interface IJsonWebWriterConfigurator
    {
        void Options(JsonSerializerOptions options);
    }
}