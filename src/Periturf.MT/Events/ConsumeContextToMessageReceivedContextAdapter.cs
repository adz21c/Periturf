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
using System;
using System.Diagnostics.CodeAnalysis;

namespace Periturf.MT.Events
{
    [ExcludeFromCodeCoverage]
    class ConsumeContextToMessageReceivedContextAdapter<TMessage> : IMessageReceivedContext<TMessage> where TMessage : class
    {
        private readonly ConsumeContext<TMessage> _consumeContext;

        public ConsumeContextToMessageReceivedContextAdapter(ConsumeContext<TMessage> consumeContext)
        {
            _consumeContext = consumeContext;
        }

        public TMessage Message => _consumeContext.Message;

        public Guid? MessageId => _consumeContext.MessageId;

        public Guid? RequestId => _consumeContext.RequestId;

        public Guid? CorrelationId => _consumeContext.CorrelationId;

        public Guid? ConversationId => _consumeContext.ConversationId;

        public Guid? InitiatorId => _consumeContext.InitiatorId;

        public DateTime? ExpirationTime => _consumeContext.ExpirationTime;

        public Uri SourceAddress => _consumeContext.SourceAddress;

        public Uri DestinationAddress => _consumeContext.DestinationAddress;

        public Uri ResponseAddress => _consumeContext.ResponseAddress;

        public Uri FaultAddress => _consumeContext.FaultAddress;

        public DateTime? SentTime => _consumeContext.SentTime;

        public Headers Headers => _consumeContext.Headers;

        public HostInfo Host => _consumeContext.Host;
    }
}
