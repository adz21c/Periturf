using Microsoft.AspNetCore.Http;

namespace Periturf.Web.Configuration
{
    class WebCookie
    {
        public WebCookie(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public string Key { get; }
        public string Value { get; }
        public CookieOptions? Options { get; internal set; }
    }
}
