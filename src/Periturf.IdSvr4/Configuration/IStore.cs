using System;

namespace Periturf.IdSvr4.Configuration
{
    interface IStore
    {
        void RegisterConfiguration(Guid id, ConfigurationRegistration config);
        void UnregisterConfiguration(Guid id);
    }
}