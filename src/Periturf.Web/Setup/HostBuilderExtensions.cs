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
    public static class HostBuilderExtensions
    {
        [ExcludeFromCodeCoverage]
        public static void Web(this IPeriturfHostBuilder builder, Action<IWebComponentSetupConfigurator> config)
        {
            builder.Web("Web", config);
        }

        public static void Web(this IPeriturfHostBuilder builder, string name, Action<IWebComponentSetupConfigurator> config)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));

            if (config == null)
                throw new ArgumentNullException(nameof(config));

            var spec = new WebComponentSetupSpecification();
            config(spec);

            WebComponent? component = null;
            builder.ConfigureWebHostDefaults(b =>
            {
                component = spec.Apply(b);
            });

            Debug.Assert(component != null, "component != null");
            builder.AddComponent(name, component);
        }
    }
}
