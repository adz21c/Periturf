using Microsoft.AspNetCore.Http;

namespace Periturf.Web.Configuration
{
    class WebCookie
    {
        public string Key { get; internal set; }
        public string Value { get; internal set; }
        public CookieOptions Options { get; internal set; }
    }
}
