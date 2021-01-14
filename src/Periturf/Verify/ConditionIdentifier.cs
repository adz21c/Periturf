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
    [ExcludeFromCodeCoverage]
    public sealed class ConditionIdentifier : IEquatable<ConditionIdentifier?>
    {
        internal ConditionIdentifier(string componentName, string description, Guid id)
        {
            ComponentName = componentName;
            Description = description;
            Id = id;
        }

        public string ComponentName { get; }

        public string Description { get; }

        public Guid Id { get; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ConditionIdentifier);
        }

        public bool Equals(ConditionIdentifier? other)
        {
            return other != null &&
                   ComponentName == other.ComponentName &&
                   Description == other.Description &&
                   Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ComponentName, Description, Id);
        }

        public static bool operator ==(ConditionIdentifier? left, ConditionIdentifier? right)
        {
            // Supressed. Code generated. Generator probably doesn't take nullability into account
#pragma warning disable CS8604 // Possible null reference argument.
            return EqualityComparer<ConditionIdentifier>.Default.Equals(left, right);
#pragma warning restore CS8604 // Possible null reference argument.
        }

        public static bool operator !=(ConditionIdentifier? left, ConditionIdentifier? right)
        {
            return !(left == right);
        }
    }
}
