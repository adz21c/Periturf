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
using Periturf.Events;
using Periturf.MT.Events;
using Periturf.Verify;
using Periturf.Verify.ComponentConditions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.MT.Verify
{
    class WhenMessagePublishedSpecification<TMessage> : ComponentMonitorSpecification, IWhenMessagePublishedConfigurator<TMessage>, IMtVerifySpecification where TMessage : class
    {
        private readonly IBusManager _busManager;
        private IVerificationHandle? _verificationHandle;
        private readonly List<Func<IMessageReceivedContext<TMessage>, bool>> _predicates = new List<Func<IMessageReceivedContext<TMessage>, bool>>();

        public WhenMessagePublishedSpecification(IBusManager busManager) : base()
        {
            _busManager = busManager;
            Description = typeof(TMessage).Name;
        }

        protected override async Task StartMonitorAsync(IConditionInstanceTimeSpanFactory timeSpanFactory, CancellationToken ct)
        {
            _verificationHandle = await _busManager.ApplyVerificationAsync(timeSpanFactory, this).ConfigureAwait(false);
        }

        protected override async Task StopMonitorAsync(CancellationToken ct)
        {
            if (_verificationHandle != null)
                await _verificationHandle.DisposeAsync().ConfigureAwait(false);
        }

        public IReadOnlyList<Func<IMessageReceivedContext<TMessage>, bool>> Predicates => _predicates;

        void IEventConfigurator<IMessageReceivedContext<TMessage>>.Predicate(Func<IMessageReceivedContext<TMessage>, bool> predicate)
        {
            _predicates.Add(predicate ?? throw new ArgumentNullException(nameof(predicate)));
        }

        void IMtVerifySpecification.Configure(IConditionInstanceTimeSpanFactory timeSpanFactory, IReceiveEndpointConfigurator configurator)
        {
            configurator.Consumer(() => new VerificationEventConsumer<TMessage>(timeSpanFactory, ConditionInstanceHandler, Predicates));
        }

    }
}
