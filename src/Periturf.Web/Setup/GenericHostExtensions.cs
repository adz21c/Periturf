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
using Microsoft.Extensions.Hosting;
using Periturf.Hosting.Setup;
using Periturf.Web;
using Periturf.Web.Setup;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Periturf
{
    /// <summary>
    /// 
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class GenericHostExtensions
    {
        public static void Web(this IGenericHostConfigurator configurator, Action<IWebComponentSetupConfigurator> config)
        {
            configurator.Web("Web", config);
        }

        public static void Web(this IGenericHostConfigurator configurator, string name, Action<IWebComponentSetupConfigurator> config)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            var spec = new WebComponentSetupSpecification(name);
            config?.Invoke(spec);

            configurator.AddComponentSpecification(spec);
        }
    }
}
