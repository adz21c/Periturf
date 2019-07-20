﻿/*
 *     Copyright 2019 Adam Burton (adz21c@gmail.com)
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
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Verify.Evaluators.Logical
{
    class AndConditionEvaluator : IConditionEvaluator
    {
        private readonly List<IConditionEvaluator> _evaluators;

        public AndConditionEvaluator(IEnumerable<IConditionEvaluator> evaluators)
        {
            _evaluators = evaluators.ToList();
        }

        public async Task<bool> EvaluateAsync(CancellationToken ct = default)
        {
            var evaluateTasks = _evaluators.Select(x => x.EvaluateAsync(ct)).ToList();
            await Task.WhenAll(evaluateTasks);
            return evaluateTasks.All(x => x.Result);
        }
    }
}
