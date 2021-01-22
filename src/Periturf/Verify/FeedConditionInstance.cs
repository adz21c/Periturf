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
    public sealed class FeedConditionInstance : IEquatable<FeedConditionInstance?>
    {
        internal FeedConditionInstance(ConditionIdentifier identifier, ConditionInstance instance)
        {
            Identifier = identifier;
            Instance = instance;
        }

        public ConditionIdentifier Identifier { get; }

        public ConditionInstance Instance { get; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as FeedConditionInstance);
        }

        public bool Equals(FeedConditionInstance? other)
        {
            return other != null &&
                   EqualityComparer<ConditionIdentifier>.Default.Equals(Identifier, other.Identifier) &&
                   EqualityComparer<ConditionInstance>.Default.Equals(Instance, other.Instance);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Identifier, Instance);
        }

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

        public static bool operator !=(FeedConditionInstance? left, FeedConditionInstance? right)
        {
            return !(left == right);
        }
    }
}