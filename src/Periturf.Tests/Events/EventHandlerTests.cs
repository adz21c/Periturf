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
using FakeItEasy;
using NUnit.Framework;
using Periturf.Components;
using Periturf.Configuration;
using Periturf.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Periturf.Setup;

namespace Periturf.Tests.Events
{
    [TestFixture]
    class EventHandlerTests
    {
        [Test]
        public async Task Given_EventHandlers_When_Execute_Then_HandlersAreCalled()
        {
            var handler1 = A.Dummy<Func<IEventContext<object>, CancellationToken, Task>>();
            var handlerSpec1 = A.Fake<IEventHandlerSpecification<object>>();
            A.CallTo(() => handlerSpec1.Build()).Returns(handler1);

            var handler2 = A.Dummy<Func<IEventContext<object>, CancellationToken, Task>>();
            var handlerSpec2 = A.Fake<IEventHandlerSpecification<object>>();
            A.CallTo(() => handlerSpec2.Build()).Returns(handler2);

            var testConfig = new TestConfigurationSpecification(new List<IEventHandlerSpecification<object>> { handlerSpec1, handlerSpec2 });

            var eventData = A.Dummy<object>();

            // Arrange
            var component = A.Dummy<IComponent>();
            A.CallTo(() => component.CreateConfigurationSpecification<TestConfigurationSpecification>(A<IEventHandlerFactory>._))
                .ReturnsLazily((IEventHandlerFactory f) =>
                {
                    testConfig.Factory = f;
                    return testConfig;
                });

            var host = A.Dummy<IHost>();
            A.CallTo(() => host.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { "component", component } }));

            var hostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => hostSpec.Build()).Returns(host);

            // Act
            var environment = Environment.Setup(x =>
            {
                x.AddHostSpecification(hostSpec);
            });

            await using (await environment.ConfigureAsync(c =>
            {
                c.AddSpecification(c.CreateComponentConfigSpecification<TestConfigurationSpecification>(nameof(component)));
            }))
            {
                await testConfig.ExecuteEvent(eventData);
            }

            A.CallTo(() => handler1.Invoke(A<IEventContext<object>>.That.NullCheckedMatches(
                e => e.Data == eventData,
                w => w.Write("Handler1")), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
            A.CallTo(() => handler2.Invoke(A<IEventContext<object>>.That.NullCheckedMatches(
                e => e.Data == eventData,
                w => w.Write("Handler2")), A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        class TestConfigurationSpecification : IConfigurationSpecification
        {
            private IEventHandler<object> _eventHandler;
            private readonly List<IEventHandlerSpecification<object>> _eventHandlerSpecifications;

            public TestConfigurationSpecification(List<IEventHandlerSpecification<object>> eventHandlerSpecifications)
            {
                _eventHandlerSpecifications = eventHandlerSpecifications;
            }

            public IEventHandlerFactory Factory { get; set; }

            public Task<IConfigurationHandle> ApplyAsync(CancellationToken ct = default)
            {
                _eventHandler = Factory.Create<object>(_eventHandlerSpecifications);
                return Task.FromResult(A.Dummy<IConfigurationHandle>());
            }

            public async Task ExecuteEvent(object eventData)
            {
                await _eventHandler.ExecuteHandlersAsync(eventData, CancellationToken.None);
            }
        }
    }
}
