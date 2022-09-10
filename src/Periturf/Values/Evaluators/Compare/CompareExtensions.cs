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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Periturf.Values;
using Periturf.Values.Evaluators;
using Periturf.Values.Evaluators.Compare;

namespace Periturf
{
    internal static class CompareExtensions
    {
        public static IValueEvaluatorSpecification<TInput> EqualTo<TInput, TValue>(this IValueProviderSpecification<TInput, TValue> left, IValueProviderSpecification<TInput, TValue> right)
            where TValue : IComparable<TValue>
        {
            return new ComparatorSpecification<TInput, TValue>(left, right, x => x == 0);
        }

        public static IValueEvaluatorSpecification<TInput> EqualTo<TInput, TValue>(this IValueProviderSpecification<TInput, TValue> left, TValue right)
            where TValue : IComparable<TValue>
        {
            return left.EqualTo(new ConstantValueProviderSpecification<TInput, TValue>(right));
        }

        public static IValueEvaluatorSpecification<TInput> LessThan<TInput, TValue>(this IValueProviderSpecification<TInput, TValue> left, IValueProviderSpecification<TInput, TValue> right)
            where TValue : IComparable<TValue>
        {
            return new ComparatorSpecification<TInput, TValue>(left, right, x => x < 0);
        }

        public static IValueEvaluatorSpecification<TInput> LessThan<TInput, TValue>(this IValueProviderSpecification<TInput, TValue> left, TValue right)
            where TValue : IComparable<TValue>
        {
            return left.LessThan(new ConstantValueProviderSpecification<TInput, TValue>(right));
        }

        public static IValueEvaluatorSpecification<TInput> LessThanOrEqualTo<TInput, TValue>(this IValueProviderSpecification<TInput, TValue> left, IValueProviderSpecification<TInput, TValue> right)
            where TValue : IComparable<TValue>
        {
            return new ComparatorSpecification<TInput, TValue>(left, right, x => x <= 0);
        }

        public static IValueEvaluatorSpecification<TInput> LessThanOrEqualTo<TInput, TValue>(this IValueProviderSpecification<TInput, TValue> left, TValue right)
            where TValue : IComparable<TValue>
        {
            return left.LessThanOrEqualTo(new ConstantValueProviderSpecification<TInput, TValue>(right));
        }

        public static IValueEvaluatorSpecification<TInput> GreaterThan<TInput, TValue>(this IValueProviderSpecification<TInput, TValue> left, IValueProviderSpecification<TInput, TValue> right)
            where TValue : IComparable<TValue>
        {
            return new ComparatorSpecification<TInput, TValue>(left, right, x => x > 0);
        }

        public static IValueEvaluatorSpecification<TInput> GreaterThan<TInput, TValue>(this IValueProviderSpecification<TInput, TValue> left, TValue right)
            where TValue : IComparable<TValue>
        {
            return left.GreaterThan(new ConstantValueProviderSpecification<TInput, TValue>(right));
        }

        public static IValueEvaluatorSpecification<TInput> GreaterThanOrEqualTo<TInput, TValue>(this IValueProviderSpecification<TInput, TValue> left, IValueProviderSpecification<TInput, TValue> right)
            where TValue : IComparable<TValue>
        {
            return new ComparatorSpecification<TInput, TValue>(left, right, x => x >= 0);
        }

        public static IValueEvaluatorSpecification<TInput> GreaterThanOrEqualTo<TInput, TValue>(this IValueProviderSpecification<TInput, TValue> left, TValue right)
            where TValue : IComparable<TValue>
        {
            return left.GreaterThanOrEqualTo(new ConstantValueProviderSpecification<TInput, TValue>(right));
        }
    }
}
