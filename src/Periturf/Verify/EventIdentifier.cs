﻿//
//   Copyright 2023 Adam Burton (adz21c@gmail.com)
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Periturf.Verify
{
    /// <summary>
    /// Identifier for a verification condition.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed record EventIdentifier
    {
        internal EventIdentifier(string componentName, Guid id)
        {
            ComponentName = componentName;
            Id = id;
        }

        /// <summary>Gets the name of the component.</summary>
        /// <value>The name of the component.</value>
        public string ComponentName { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; }
    }
}