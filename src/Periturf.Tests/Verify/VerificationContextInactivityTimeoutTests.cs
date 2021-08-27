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
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class VerificationContextInactivityTimeoutTests
    {
        private const string ComponentName = "Component";
        private const int InactivityTimeout = 300;
        private Environment _sut;
        private IConditionSpecification _conditionSpec;

        [SetUp]
        public void SetUp()
        {
            var component = A.Fake<IComponent>();
            var host = A.Fake<IHost>();
            A.CallTo(() => host.Components).Returns(new Dictionary<string, IComponent> { { ComponentName, component } });
            var hostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => hostSpec.Build()).Returns(host);

            _sut = Environment.Setup(s =>
            {
                s.VerifyInactivityTimeout = TimeSpan.FromMilliseconds(InactivityTimeout);
                s.AddHostSpecification(hostSpec);
            });

            var conditionFeed = A.Fake<IConditionFeed>();
            A.CallTo(() => conditionFeed.WaitForInstancesAsync(A<CancellationToken>._))
                .ReturnsLazily(async () => 
                {
                    var delay = TimeSpan.FromMilliseconds(InactivityTimeout + 100);
                    await Task.Delay(delay);
                    return new List<ConditionInstance>() { new ConditionInstance(delay, "ID1") };
                });

            _conditionSpec = A.Fake<IConditionSpecification>();
            A.CallTo(() => _conditionSpec.BuildAsync(A<IConditionInstanceFactory>._, A<CancellationToken>._)).Returns(conditionFeed);
            A.CallTo(() => _conditionSpec.ComponentName).Returns(A.Dummy<string>());
            A.CallTo(() => _conditionSpec.ConditionDescription).Returns(A.Dummy<string>());
        }

        [Test]
        public async Task Given_DefautTimeout_When_VerifyWithNoActivity_Then_CompletesBasedOnTimeout()
        {
            var verifier = _sut.Verify(ctx =>
                ctx.Expect(e =>
                    e.Constraint(ec =>
                        ec.Condition(
                            ctx.Condition(c => _conditionSpec)))));

            Assume.That(verifier, Is.Not.Null);

            var stopwatch = Stopwatch.StartNew();
            var result = await verifier.VerifyAsync();
            stopwatch.Stop();

            Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(InactivityTimeout));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AsExpected, Is.False);
        }

        [Test]
        public async Task Given_OverrideTimeout_When_VerifyWithNoActivity_Then_CompletesBasedOnOverrideTimeout()
        {
            const int OverrideTimeout = 100;
            
            var verifier = _sut.Verify(ctx =>
            {
                ctx.InactivityTimeout = TimeSpan.FromMilliseconds(OverrideTimeout);
                ctx.Expect(e =>
                    e.Constraint(ec =>
                        ec.Condition(
                            ctx.Condition(c => _conditionSpec))));
            });

            Assume.That(verifier, Is.Not.Null);

            var stopwatch = Stopwatch.StartNew();
            var result = await verifier.VerifyAsync();
            stopwatch.Stop();

            Assert.That(stopwatch.ElapsedMilliseconds, Is.GreaterThanOrEqualTo(OverrideTimeout));
            Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(InactivityTimeout));
            Assert.That(result, Is.Not.Null);
            Assert.That(result.AsExpected, Is.False);
        }
    }
}
