//
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

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Periturf.Verify
{
    /// <summary>
    /// Extends <see cref="ConditionInstance"/> to include the condition identifier.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class FeedConditionInstance : IEquatable<FeedConditionInstance?>
    {
        internal FeedConditionInstance(ConditionIdentifier identifier, ConditionInstance instance)
        {
            Identifier = identifier;
            Instance = instance;
        }

        /// <summary>
        /// Gets the condition identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public ConditionIdentifier Identifier { get; }

        /// <summary>
        /// Gets the condition instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public ConditionInstance Instance { get; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.
        /// </returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as FeedConditionInstance);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(FeedConditionInstance? other)
        {
            return other != null &&
                   EqualityComparer<ConditionIdentifier>.Default.Equals(Identifier, other.Identifier) &&
                   EqualityComparer<ConditionInstance>.Default.Equals(Instance, other.Instance);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Identifier, Instance);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(FeedConditionInstance? left, FeedConditionInstance? right)
        {
            if (left is null)
                return right is null;
            else
            {
                if (right is null)
                    return false;
                else
                    return EqualityComparer<FeedConditionInstance>.Default.Equals(left, right);
            }
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(FeedConditionInstance? left, FeedConditionInstance? right)
        {
            return !(left == right);
        }
    }
}