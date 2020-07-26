﻿using Microsoft.AspNetCore.Http;
using Periturf.Events;
using System;

namespace Periturf.Web.Configuration
{
    public interface IWebRequestEventConfigurator : IEventConfigurator<IWebRequest>
    {
        void AddPredicateSpecification(IWebRequestPredicateSpecification spec);

        void SetResponseSpecification(IWebRequestResponseSpecification spec);
    }
}
