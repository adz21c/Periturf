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
using FakeItEasy;
using IdentityServer4.Events;
using NUnit.Framework;
using Periturf.IdSvr4.Verify;
using System;

namespace Periturf.Tests.IdSvr4.Verify
{
    [TestFixture]
    class IdSvr4ConditionBuilderTests
    {
        [Test]
        public void Given_Builder_When_EventOccurred_Then_CreatedEventOccurredSpecification()
        {
            var eventMonitorSink = A.Fake<IEventMonitorSink>();
            var condition = A.Fake<Func<Event, bool>>();

            var builder = new IdSvr4ConditionBuilder(eventMonitorSink);

            // Act
            var spec = builder.EventOccurred(condition);

            // Assert
            Assert.IsNotNull(spec);
            Assert.AreEqual(typeof(EventOccurredConditionSpecification<Event>), spec.GetType());
        }
    }
}
