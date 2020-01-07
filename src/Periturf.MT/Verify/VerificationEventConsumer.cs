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
using MassTransit;
using Periturf.MT.Events;
using Periturf.Verify;
using Periturf.Verify.ComponentConditions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Periturf.MT.Verify
{
    class VerificationEventConsumer<TMessage> : IConsumer<TMessage> where TMessage : class
    {
        private readonly IConditionInstanceTimeSpanFactory _timeSpanFactory;
        private readonly IReadOnlyList<Func<IMessageReceivedContext<TMessage>, bool>> _predicates;
        private readonly IConditionInstanceHandler _conditionInstanceHandler;

        public VerificationEventConsumer(
            IConditionInstanceTimeSpanFactory timeSpanFactory,
            IConditionInstanceHandler conditionInstanceHandler,
            IReadOnlyList<Func<IMessageReceivedContext<TMessage>, bool>> predicates)
        {
            _timeSpanFactory = timeSpanFactory;
            _predicates = predicates;
            _conditionInstanceHandler = conditionInstanceHandler;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var adapter = new ConsumeContextToMessageReceivedContextAdapter<TMessage>(context);
            if (_predicates.Any() && !_predicates.Any(x => x(adapter)))
                return;


            Debug.Assert(context.SentTime.HasValue);
            
            await _conditionInstanceHandler.HandleInstanceAsync(
                new ConditionInstance(
                    _timeSpanFactory.Create(context.SentTime.Value),
                    "Something"),
                context.CancellationToken);
        }
    }
}
