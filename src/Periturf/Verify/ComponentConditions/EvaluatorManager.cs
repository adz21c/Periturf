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
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Periturf.Verify.ComponentConditions
{
    class EvaluatorManager : IConditionInstanceHandler
    {
        private readonly Channel<ProducerCommandDto> _producerChannel = Channel.CreateUnbounded<ProducerCommandDto>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = true
        });

        private readonly List<ComponentConditionEvaluatorInstance> _evaluators = new List<ComponentConditionEvaluatorInstance>();
        private readonly List<ConditionInstance> _history = new List<ConditionInstance>();

        private readonly Func<IComponentConditionEvaluator, CancellationToken, Task>? _onEvaluatorRemoved;

        public EvaluatorManager(Func<IComponentConditionEvaluator, CancellationToken, Task>? onEvaluatorRemoved = null)
        {
            _onEvaluatorRemoved = onEvaluatorRemoved;

            Task.Run(async () => await HandleProducerChannel());
        }

        public bool HasActiveEvaluators => _evaluators.Any();

        public async ValueTask<IComponentConditionEvaluator> CreateEvaluatorAsync(CancellationToken ct = default)
        {
            var evaluator = new ComponentConditionEvaluatorInstance(this);
            await _producerChannel.Writer.WriteAsync(
                new ProducerCommandDto
                {
                    Command = ProduceCommands.AddEvaluator,
                    Evaluator = evaluator
                },
                ct).ConfigureAwait(false);
            return evaluator;
        }

        private async ValueTask RemoveEvaluatorAsync(ComponentConditionEvaluatorInstance evaluator, CancellationToken ct = default)
        {
            await _producerChannel.Writer.WriteAsync(
                new ProducerCommandDto
                {
                    Command = ProduceCommands.RemoveEvaluator,
                    Evaluator = evaluator
                },
                ct).ConfigureAwait(false);
        }

        private async Task HandleProducerChannel()
        {
            while (await _producerChannel.Reader.WaitToReadAsync())
            {
                if (_producerChannel.Reader.TryRead(out var command))
                {
                    switch (command.Command)
                    {
                        case ProduceCommands.AddEvaluator:
                            Debug.Assert(command.Evaluator != null);
                            
                            _evaluators.Add(command.Evaluator);
                            foreach (var instance in _history)
                                await command.Evaluator.ThreadingChannel.Writer.WriteAsync(instance).ConfigureAwait(false);
                            break;
                        case ProduceCommands.RemoveEvaluator:
                            Debug.Assert(command.Evaluator != null);

                            _evaluators.Remove(command.Evaluator);
                            
                            if (!_evaluators.Any())
                                _onEvaluatorRemoved?.Invoke(command.Evaluator, default(CancellationToken));
                            break;
                        case ProduceCommands.RegisterInstance:
                            Debug.Assert(command.ConditionInstance != null);

                            _history.Add(command.ConditionInstance);
                            await Task.WhenAll(
                                _evaluators
                                    .Select(x => x
                                        .ThreadingChannel
                                        .Writer
                                        .WriteAsync(command.ConditionInstance)
                                        .AsTask())).ConfigureAwait(false);
                            break;
                    }
                }
            }
        }

        async ValueTask IConditionInstanceHandler.HandleInstanceAsync(ConditionInstance instance, CancellationToken ct)
        {
            await _producerChannel.Writer.WriteAsync(
                new ProducerCommandDto
                {
                    Command = ProduceCommands.RegisterInstance,
                    ConditionInstance = instance
                },
                ct).ConfigureAwait(false);
        }

        enum ProduceCommands
        {
            AddEvaluator,
            RemoveEvaluator,
            RegisterInstance
        }

        class ProducerCommandDto
        {
            public ProduceCommands Command { get; set; }

            public ConditionInstance? ConditionInstance { get; set; }

            public ComponentConditionEvaluatorInstance? Evaluator { get; set; }
        }

        class ComponentConditionEvaluatorInstance : IComponentConditionEvaluator
        {
            private readonly EvaluatorManager _manager;
            
            public ComponentConditionEvaluatorInstance(EvaluatorManager manager)
            {
                _manager = manager;
            }

            public Channel<ConditionInstance> ThreadingChannel { get; } = Channel.CreateUnbounded<ConditionInstance>(
                new UnboundedChannelOptions
                {
                    SingleReader = true,
                    SingleWriter = true
                });

            public async IAsyncEnumerable<ConditionInstance> GetInstancesAsync([EnumeratorCancellation] CancellationToken ect = default)
            {
                while (await ThreadingChannel.Reader.WaitToReadAsync(ect))
                {
                    if (ThreadingChannel.Reader.TryRead(out var instance))
                        yield return instance;
                }
            }

            public async ValueTask DisposeAsync()
            {
                ThreadingChannel.Writer.Complete();
                await _manager.RemoveEvaluatorAsync(this).ConfigureAwait(false);
            }
        }
    }
}
