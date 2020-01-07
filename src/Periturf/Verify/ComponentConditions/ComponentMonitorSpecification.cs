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
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Verify.ComponentConditions
{
    /// <summary>
    /// Handles the complications of a specification managing multiple evaluator instances and distributing the condition instances.
    /// </summary>
    /// <seealso cref="IComponentConditionSpecification" />
    public abstract class ComponentMonitorSpecification : IComponentConditionSpecification
    {
        private readonly EvaluatorManager _manager;
        private bool _monitoring = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentMonitorSpecification"/> class.
        /// </summary>
        protected ComponentMonitorSpecification()
        {
            _manager = new EvaluatorManager((e, ct) => StopMonitorAsync(ct));
        }

        /// <summary>
        /// Gets the condition instance handler to pass condition instance's too. This will pass them on to all required evaluators.
        /// </summary>
        /// <value>
        /// The condition instance handler.
        /// </value>
        protected IConditionInstanceHandler ConditionInstanceHandler => _manager;

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; protected set; } = string.Empty;

        /// <summary>
        /// Constructs an <see cref="IComponentConditionEvaluator" />.
        /// When called multiple times builds a reference count to monitor the condition
        /// across multiple streams.
        /// </summary>
        /// <param name="timespanFactory">The <see cref="TimeSpan"/> factory.</param>
        /// <param name="ct">The cancellation token.</param>
        /// <returns></returns>
        public async Task<IComponentConditionEvaluator> BuildAsync(IConditionInstanceTimeSpanFactory timespanFactory, CancellationToken ct = default)
        {
            if (!_monitoring)
            {
                await StartMonitorAsync(timespanFactory, ct).ConfigureAwait(false);
                _monitoring = true;
            }

            return await _manager.CreateEvaluatorAsync(ct);
        }

        /// <summary>
        /// Called when creating the first evaluator to initialize monitoring for condition instances within the component.
        /// </summary>
        /// <param name="timeSpanFactory">The <see cref="TimeSpan"/> factory.</param>
        /// <param name="ct">The cancelation token.</param>
        /// <returns></returns>
        protected abstract Task StartMonitorAsync(IConditionInstanceTimeSpanFactory timeSpanFactory, CancellationToken ct);

        /// <summary>
        /// Called when the last evaluator is removed to allow the monitoring of the component to be removed.
        /// </summary>
        /// <param name="ct">The cancellation token.</param>
        /// <returns></returns>
        protected abstract Task StopMonitorAsync(CancellationToken ct);
    }
}
