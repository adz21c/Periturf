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
using Periturf.MT.InMemory;
using Periturf.MT.Setup;

namespace Periturf
{
    /// <summary>
    /// 
    /// </summary>
    public static class InMemoryHostExtensions
    {
        /// <summary>
        /// Configures an in memory bus.
        /// </summary>
        /// <param name="busConfigurator">The bus configurator.</param>
        public static void InMemoryHost(this IBusConfigurator busConfigurator)
        {
            busConfigurator.SetBusManager(new InMemoryBusManager());
        }
    }
}
