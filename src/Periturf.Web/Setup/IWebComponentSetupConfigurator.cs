using Microsoft.AspNetCore.Hosting;
using System;

namespace Periturf.Web.Setup
{
    public interface IWebComponentSetupConfigurator
    {
        void ConfigureBuilder(Action<IWebHostBuilder> config);
    }
}
