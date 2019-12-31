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
using Periturf.MT.Configuration;
using Periturf.Verify;
using System;

namespace Periturf.MT.Verify
{
    /// <summary>
    /// MassTransit condition builder.
    /// </summary>
    /// <seealso cref="IComponentConditionBuilder" />
    public interface IMtConditionBuilder : IComponentConditionBuilder
    {
        /// <summary>
        /// Verify when a message is published to the bus.
        /// </summary>
        /// <typeparam name="TMessage">The type of the message.</typeparam>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        IComponentConditionSpecification WhenMessagePublished<TMessage>(Func<IMessageReceivedContext<TMessage>, bool> condition) where TMessage : class;
    }
}
