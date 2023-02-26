//
//   Copyright 2023 Adam Burton (adz21c@gmail.com)
//   
//   Licensed under the Apache License, Version 2.0 (the "License")
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Periturf.Components;
using Periturf.Events;
using Periturf.Setup;
using Periturf.Verify;

namespace Periturf.Tests.Verify
{
    class EnvironmentVerifyTests
    {
        private IComponent _component;
        private Environment _environment;

        [SetUp]
        public void SetUp()
        {
            _component = A.Fake<IComponent>();
            var host = A.Fake<IHost>();
            A.CallTo(() => host.Components).Returns(new ReadOnlyDictionary<string, IComponent>(new Dictionary<string, IComponent> { { nameof(_component), _component } }));
            var hostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => hostSpec.Build()).Returns(host);

            _environment = Environment.Setup(x => x.AddHostSpecification(hostSpec));
        }

        [Test]
        public async Task Given_Component_When_GetEventBuilder_Then_BuilderRetrieved()
        {
            var eventSpec = A.Dummy<IEventSpecification>();
            var eventBuilder = A.Dummy<IEventBuilder>();
            A.CallTo(() => _component.CreateEventBuilder()).Returns(eventBuilder);

            IEventBuilder foundEventBuilder = null;
            await _environment.VerifyAsync(c =>
            {
                c.Event(e =>
                {
                    foundEventBuilder = e.GetEventBuilder<IEventBuilder>(nameof(_component));
                    return eventSpec;
                });
            });

            Assert.That(foundEventBuilder, Is.Not.Null.And.SameAs(eventBuilder));
            A.CallTo(() => _component.CreateEventBuilder()).MustHaveHappened();
        }

        [Test]
        public void Given_Component_When_GetEventBuilderWithInvalidName_Then_Throws()
        {
            var eventSpec = A.Dummy<IEventSpecification>();
            Assert.That(
                () => _environment.VerifyAsync(c => c.Event(e =>
                {
                    e.GetEventBuilder<IEventBuilder>("Bob");
                    return eventSpec;
                })),
                Throws.TypeOf<ComponentLocationFailedException>());
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task Given_Events_When_VerifierStart_Then_SpecBuilt(int numberOfEvents)
        {
            var ct = A.Dummy<CancellationToken>();
            var eventSpecs = Enumerable.Range(1, numberOfEvents).Select(x => A.Fake<IEventSpecification>()).ToList();

            var verifier = _environment.VerifyAsync(c =>
            {
                foreach(var spec in eventSpecs)
                    c.Event(e => spec);
            });

            // Cancellation token passed through
            foreach (var spec in eventSpecs)
                A.CallTo(() => spec.BuildAsync(A<CancellationToken>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Given_BuildingVerifier_When_ErrorDuringEventBuild_Then_AllEventFeedsDisposedOrCancelled()
        {
            var ct = A.Dummy<CancellationToken>();
            
            var completeEventFeed = A.Fake<IEventFeed>();
            var completeEventSpec = A.Fake<IEventSpecification>();
            A.CallTo(() => completeEventSpec.BuildAsync(A<CancellationToken>._)).Returns(completeEventFeed);
            
            var incompleteEventSpec = A.Fake<IEventSpecification>();
            A.CallTo(() => incompleteEventSpec.BuildAsync(A<CancellationToken>._)).Invokes((CancellationToken ct) => Task.Delay(150, ct));

            var ex = new Exception("Broke");
            var errorEventSpec = A.Fake<IEventSpecification>();
            A.CallTo(() => errorEventSpec.BuildAsync(A<CancellationToken>._)).Invokes((CancellationToken ct) => Task.Delay(50, ct)).ThrowsAsync(ex);

            Assert.That(() => _environment.VerifyAsync(c =>
            {
                c.Event(e => completeEventSpec);
                c.Event(e => incompleteEventSpec);
                c.Event(e => errorEventSpec);
            }), Throws.Exception);

            A.CallTo(() => completeEventSpec.BuildAsync(A<CancellationToken>._)).MustHaveHappened().Then(
                A.CallTo(() => completeEventFeed.DisposeAsync()).MustHaveHappened());
            A.CallTo(() => incompleteEventSpec.BuildAsync(A<CancellationToken>._)).MustHaveHappened();
            A.CallTo(() => errorEventSpec.BuildAsync(A<CancellationToken>._)).MustHaveHappened();
        }
    }
}
