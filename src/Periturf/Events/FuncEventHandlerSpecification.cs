/*
 *     Copyright 2020 Adam Burton (adz21c@gmail.com)
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

namespace Periturf.Events
{
    class FuncEventHandlerSpecification<TEventData> : IEventHandlerSpecification<TEventData>
    {
        private readonly Func<IEventContext<TEventData>, CancellationToken, Task> _handler;

        public FuncEventHandlerSpecification(Func<IEventContext<TEventData>, CancellationToken, Task> handler)
        {
            _handler = handler;
        }

        public Func<IEventContext<TEventData>, CancellationToken, Task> Build()
        {
            return _handler;
        }
    }
}
