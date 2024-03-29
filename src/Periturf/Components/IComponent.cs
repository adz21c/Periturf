﻿//
//   Copyright 2021 Adam Burton (adz21c@gmail.com)
//   
//   Licensed under the Apache License, Version 2.0 (the "License")
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//

using Periturf.Clients;
using Periturf.Configuration;
using Periturf.Events;
using Periturf.Verify;

namespace Periturf.Components
{
    /// <summary>
    /// Components run on hosts and are the pieces of the environment that the system under test will interact with.
    /// Configuration can be registered and unregistered with them.
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// Creates a component specific condition builder.
        /// </summary>
        /// <returns></returns>
        IConditionBuilder CreateConditionBuilder();

        /// <summary>
        /// Creates a component configuration specification.
        /// </summary>
        /// <typeparam name="TSpecification">The type of the specification.</typeparam>
        /// <param name="eventHandlerFactory">The event handler factory.</param>
        /// <returns></returns>
        TSpecification CreateConfigurationSpecification<TSpecification>(IEventHandlerFactory eventHandlerFactory)
            where TSpecification : IConfigurationSpecification;
        
        /// <summary>
        /// Creates a component client.
        /// </summary>
        /// <returns></returns>
        IComponentClient CreateClient();
    }
}
