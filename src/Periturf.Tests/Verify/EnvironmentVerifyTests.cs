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
        public void Given_Component_When_GetEventBuilder_Then_BuilderRetrieved()
        {
            var eventBuilder = A.Dummy<IEventBuilder>();
            A.CallTo(() => _component.CreateEventBuilder()).Returns(eventBuilder);

            IEventBuilder foundEventBuilder = null;
            _environment.Verify(c =>
            {
                c.Event(e => foundEventBuilder = e.GetEventBuilder<IEventBuilder>(nameof(_component)));
            });

            Assert.That(foundEventBuilder, Is.Not.Null.And.SameAs(eventBuilder));
            A.CallTo(() => _component.CreateEventBuilder()).MustHaveHappened();
        }

        [Test]
        public void Given_Component_When_GetEventBuilderWithInvalidName_Then_Throws()
        {
            Assert.That(
                () => _environment.Verify(c => c.Event(e => e.GetEventBuilder<IEventBuilder>("Bob"))),
                Throws.TypeOf<ComponentLocationFailedException>());
        }
    }
}
