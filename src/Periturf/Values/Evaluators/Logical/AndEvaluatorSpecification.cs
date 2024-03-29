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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Periturf.Values.Evaluators.Logical
{
    class AndEvaluatorSpecification<TInput> : IValueEvaluatorSpecification<TInput>
    {
        private readonly IValueEvaluatorSpecification<TInput>[] _conditions;

        public AndEvaluatorSpecification(params IValueEvaluatorSpecification<TInput>[] conditions)
        {
            _conditions = conditions;
        }

        public Func<TInput, ValueTask<bool>> Build()
        {
            var builtConditions = _conditions.Select(x => x.Build()).ToList();

            return async i =>
            {
                foreach (var condition in builtConditions)
                {
                    var conditionResult = await condition(i);
                    if (!conditionResult)
                        return false;
                }
                return true;
            };
        }
    }
}
