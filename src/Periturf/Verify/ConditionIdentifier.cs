/*
 *     Copyright 2021 Adam Burton (adz21c@gmail.com)
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
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Periturf.Verify
{
    /// <summary>
    /// Identifier for a verification condition.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public sealed class ConditionIdentifier : IEquatable<ConditionIdentifier?>
    {
        internal ConditionIdentifier(string componentName, string description, Guid id)
        {
            ComponentName = componentName;
            Description = description;
            Id = id;
        }

        /// <summary>Gets the name of the component.</summary>
        /// <value>The name of the component.</value>
        public string ComponentName { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public Guid Id { get; }

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns>
        ///   <see langword="true" /> if the specified object  is equal to the current object; otherwise, <see langword="false" />.
        /// </returns>
        public override bool Equals(object? obj)
        {
            return Equals(obj as ConditionIdentifier);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(ConditionIdentifier? other)
        {
            return other != null &&
                   ComponentName == other.ComponentName &&
                   Description == other.Description &&
                   Id.Equals(other.Id);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(ComponentName, Description, Id);
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ConditionIdentifier? left, ConditionIdentifier? right)
        {
            // Supressed. Code generated. Generator probably doesn't take nullability into account
#pragma warning disable CS8604 // Possible null reference argument.
            return EqualityComparer<ConditionIdentifier>.Default.Equals(left, right);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(ConditionIdentifier? left, ConditionIdentifier? right)
        {
            return !(left == right);
        }
    }
}
