using Periturf.Web.Configuration;

namespace Periturf.Web
{
    public interface IWebRequestEvent
    {
        string TraceIdentifier { get; }

        IWebRequest Request { get; }
    }
}