namespace Periturf.Web.Configuration.Requests.Responses
{
    public interface IWebRequestResponseObjectConfigurator
    {
        void Object(object? value);

        void SetWriterSpecification(IWebWriterSpecification writerSpecification);
    }
}