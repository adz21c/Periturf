/*
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Verify.Evaluators.Logical
{
    class XorConditionSpecification : IConditionSpecification
    {
        private readonly List<IConditionSpecification> _conditions;

        public XorConditionSpecification(List<IConditionSpecification> conditions)
        {
            _conditions = conditions;
        }

        public async Task<IConditionEvaluator> BuildEvaluatorAsync(Guid verifierId, IConditionErasePlan erasePlan, CancellationToken ct = default)
        {
            var buildTasks = _conditions.Select(x => x.BuildEvaluatorAsync(verifierId, erasePlan, ct)).ToList();
            await Task.WhenAll(buildTasks);
            return new XorConditionEvaluator(buildTasks.Select(x => x.Result));
        }
    }
}
