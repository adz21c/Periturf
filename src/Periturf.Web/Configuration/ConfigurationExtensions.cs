/*
 *     Copyright 2019 Adam Burton (adz21c@gmail.com)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using Periturf.Configuration;
using Periturf.Web;
using Periturf.Web.Configuration;
using Periturf.Web.Configuration.Requests;
using Periturf.Web.Configuration.Requests.Predicates;
using Periturf.Web.Configuration.Requests.Responses;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Periturf
{
    [ExcludeFromCodeCoverage]
    public static class ConfigurationExtensions
    {
        public static void Web(this IConfigurationContext builder, Action<IWebComponentConfigurator> config)
        {
            builder.Web("Web", config);
        }

        public static void Web(this IConfigurationContext builder, string name, Action<IWebComponentConfigurator> config)
        {
            var spec = builder.CreateComponentConfigSpecification<WebComponentSpecification>(name);
            config(spec);
            builder.AddSpecification(spec);
        }

        public static void Predicate(this IWebRequestEventConfigurator configurator, Func<IWebRequestEvent, bool> predicate)
        {
            configurator.AddPredicateSpecification(new WebRequestPredicateSpecification(predicate));
        }

        public static void Response(this IWebRequestEventConfigurator configurator, Action<IWebRequestResponseConfigurator> config)
        {
            var spec = new WebRequestResponseSpecification();
            config?.Invoke(spec);
            configurator.SetResponseSpecification(spec);
        }
    }
}
