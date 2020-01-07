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
using Periturf.Clients;
using Periturf.Events;

namespace Periturf.MT.Events
{
    class ConsumerEventResponseContext<TReceivedMessage> : IEventResponseContext<IMessageReceivedContext<TReceivedMessage>>
    {
        private readonly string _interceptComponentName;
        private readonly ConsumeContext _consumeContext;
        private readonly IEventResponseContext<IMessageReceivedContext<TReceivedMessage>> _inner;

        public ConsumerEventResponseContext(
            string interceptComponentName,
            ConsumeContext consumeContext,
            IEventResponseContext<IMessageReceivedContext<TReceivedMessage>> inner)
        {
            _interceptComponentName = interceptComponentName;
            _consumeContext = consumeContext;
            _inner = inner;
        }

        public IMessageReceivedContext<TReceivedMessage> Data => _inner.Data;

        public IComponentClient CreateComponentClient(string componentName)
        {
            if (componentName == _interceptComponentName)
                return new ConsumerMtClient(_consumeContext);

            return _inner.CreateComponentClient(componentName);
        }
    }
}
