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
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Periturf.IdSvr4.Setup
{
    class IdSvr4SetupSpecification : IIdSvr4SetupConfigurator
    {
        void IIdSvr4SetupConfigurator.Configure(Action<IApplicationBuilder> config)
        {
            AppConfigCallback = config;
        }

        public Action<IApplicationBuilder>? AppConfigCallback { get; private set; }

        void IIdSvr4SetupConfigurator.Services(Action<IIdentityServerBuilder> config)
        {
            ServicesCallback = config;
        }

        public Action<IIdentityServerBuilder>? ServicesCallback { get; private set; }
    }
}