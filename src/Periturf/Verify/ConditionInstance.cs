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
    /// A moment when a component condition matched
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class ConditionInstance : IEquatable<ConditionInstance?>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionInstance"/> class.
        /// </summary>
        /// <param name="when">The when.</param>
        /// <param name="id">The identifier.</param>
        public ConditionInstance(TimeSpan when, string id)
        {
            When = when;
            ID = id ?? string.Empty;
        }

        /// <summary>
        /// When the condition instance occurred relative to the time of definition.
        /// </summary>
        /// <value>
        /// The when.
        /// </value>
        public TimeSpan When { get; }

        /// <summary>
        /// Component specific identifier for the condition instance.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string ID { get; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.
        /// </returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as ConditionInstance);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(ConditionInstance? other)
        {
            return other != null &&
                   When.Equals(other.When) &&
                   ID == other.ID;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(When, ID);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ConditionInstance? left, ConditionInstance? right)
        {
            if (left is null)
                return right is null;
            else
            {
                if (right is null)
                    return false;
                else
                    return EqualityComparer<ConditionInstance>.Default.Equals(left, right);
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
        public static bool operator !=(ConditionInstance? left, ConditionInstance? right)
        {
            return !(left == right);
        }
    }
}
