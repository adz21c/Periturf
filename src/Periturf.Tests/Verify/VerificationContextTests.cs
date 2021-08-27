//
//   Copyright 2021 Adam Burton (adz21c@gmail.com)
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

using FakeItEasy;
using NUnit.Framework;
using Periturf.Components;
using Periturf.Setup;
using Periturf.Verify;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class VerificationContextTests
    {
        private const string ComponentName = "Component";
        private Environment _sut;
        private IComponent _component;

        [SetUp]
        public void SetUp()
        {
            _component = A.Fake<IComponent>();
            var host = A.Fake<IHost>();
            A.CallTo(() => host.Components).Returns(new Dictionary<string, IComponent> { { ComponentName, _component } });
            var hostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => hostSpec.Build()).Returns(host);

            _sut = Environment.Setup(s => s.AddHostSpecification(hostSpec));
        }

        [Test]
        public void Given_Env_When_Verify_Then_VerifierCreated()
        {
            var conditionSpec = A.Dummy<IConditionSpecification>();
            var conditionConfig = A.Fake<Func<IConditionConfigurator, IConditionSpecification>>();
            A.CallTo(() => conditionConfig.Invoke(A<IConditionConfigurator>._))
                .Invokes((IConditionConfigurator c) => c.GetConditionBuilder(ComponentName))
                .Returns(conditionSpec);
            ConditionIdentifier conditionIdentifier = null;

            var expectationConfig = A.Fake<Action<IExpectationConfigurator>>();
            A.CallTo(() => expectationConfig.Invoke(A<IExpectationConfigurator>._)).Invokes((IExpectationConfigurator e) =>
            {
                e.Constraint(c => c.Condition(conditionIdentifier));
            });

            var verifier = _sut.Verify(ctx =>
            {
                conditionIdentifier = ctx.Condition(conditionConfig);
                ctx.Expect(expectationConfig);
            });

            Assert.That(verifier, Is.Not.Null);
            Assert.That(conditionIdentifier, Is.Not.Null);
            A.CallTo(() => conditionConfig.Invoke(A<IConditionConfigurator>._)).MustHaveHappened();
            A.CallTo(() => expectationConfig.Invoke(A<IExpectationConfigurator>._)).MustHaveHappened();
            A.CallTo(() => _component.CreateConditionBuilder()).MustHaveHappened();
        }

        [Test]
        public void Given_ConditionForNonExistentComponent_When_Verify_Then_Throws()
        {
            var conditionConfig = A.Fake<Func<IConditionConfigurator, IConditionSpecification>>();
            A.CallTo(() => conditionConfig.Invoke(A<IConditionConfigurator>._))
                .Invokes((IConditionConfigurator c) => c.GetConditionBuilder("NonComponentName"));

            Assert.That(() => _sut.Verify(ctx => ctx.Condition(conditionConfig)), Throws.TypeOf<ComponentLocationFailedException>());
        }
    }
}
