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
using MassTransit.Definition;
using MassTransit.RabbitMqTransport;
using Periturf.Configuration;
using Periturf.Events;
using Periturf.MT.Configuration;
using Periturf.MT.Verify;
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Periturf.MT.RabbitMQ
{
    [ExcludeFromCodeCoverage]
    class RabbitMQBusManager : IBusManager
    {
        private readonly Action<IRabbitMqHostConfigurator> _hostConfigurator;
        private readonly Uri _hostUri;

        private IRabbitMqHost? _host;

        public RabbitMQBusManager(
            Action<IRabbitMqHostConfigurator> hostConfigurator,
            Uri hostUri)
        {
            _hostConfigurator = hostConfigurator;
            _hostUri = hostUri;
        }

        public IBusControl? BusControl { get; private set; }

        public void Setup(IMtSpecification specification, IEventResponseContextFactory eventResponseContextFactory)
        {
            BusControl = Bus.Factory.CreateUsingRabbitMq(b =>
            {
                _host = b.Host(_hostUri, _hostConfigurator);

                foreach (var endpoint in specification.WhenMessagePublishedSpecifications)
                {
                    b.ReceiveEndpoint(new TemporaryEndpointDefinition(), r => endpoint.Configure(r, eventResponseContextFactory));
                }
            });
        }

        public async Task<IConfigurationHandle> ApplyConfigurationAsync(IMtSpecification specification, IEventResponseContextFactory eventResponseContextFactory)
        {
            Debug.Assert(_host != null);

            var receiveEndpointHandles = new List<HostReceiveEndpointHandle>();

            foreach (var endpoint in specification.WhenMessagePublishedSpecifications)
            {
                var handle = _host.ConnectReceiveEndpoint(
                    new TemporaryEndpointDefinition(),
                    DefaultEndpointNameFormatter.Instance,
                    r => endpoint.Configure(r, eventResponseContextFactory));
                receiveEndpointHandles.Add(handle);
            }

            await Task.WhenAll(receiveEndpointHandles.Select(x => x.Ready)).ConfigureAwait(false);

            return new MtHandle(receiveEndpointHandles);
        }

        public async Task<IVerificationHandle> ApplyVerificationAsync(IConditionInstanceTimeSpanFactory timeSpanFactory, IMtVerifySpecification specification)
        {
            Debug.Assert(_host != null);

            var receiveEndpointHandle = _host.ConnectReceiveEndpoint(
                new TemporaryEndpointDefinition(),
                DefaultEndpointNameFormatter.Instance,
                r => specification.Configure(timeSpanFactory, r));

            await receiveEndpointHandle.Ready.ConfigureAwait(false);

            return new MtHandle(new List<HostReceiveEndpointHandle> { receiveEndpointHandle });
        }
    }
}
