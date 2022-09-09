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

namespace Periturf.Values.Evaluators.Compare
{
    class ComparatorSpecification<TInput, TValue> : IValueEvaluatorSpecification<TInput>
        where TValue : IComparable<TValue>
    {
        private readonly IValueProviderSpecification<TInput, TValue> _left;
        private readonly IValueProviderSpecification<TInput, TValue> _right;

        public ComparatorSpecification(IValueProviderSpecification<TInput, TValue> left, IValueProviderSpecification<TInput, TValue> right)
        {
            _left = left;
            _right = right;
        }

        public Func<TInput, ValueTask<bool>> Build()
        {
            var comparer = Comparer<TValue>.Default;
            var leftProvider = _left.Build();
            var rightProvider = _right.Build();

            return async i =>
            {
                var left = await leftProvider(i);
                var right = await rightProvider(i);

                return comparer.Compare(left, right) == 0;
            };
        }
    }
}
