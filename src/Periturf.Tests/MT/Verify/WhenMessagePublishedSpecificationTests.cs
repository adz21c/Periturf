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
using MassTransit;
using NUnit.Framework;
using Periturf.MT;
using Periturf.MT.Events;
using Periturf.MT.Verify;
using Periturf.Verify;
using System;
using System.Threading.Tasks;

namespace Periturf.Tests.MT.Verify
{
    [TestFixture]
    class WhenMessagePublishedSpecificationTests
    {
        private IBusManager _busManager;
        private Func<IMessageReceivedContext<ITestMessage>, bool> _condition;
        private WhenMessagePublishedSpecification<ITestMessage> _sut;

        [SetUp]
        public void SetUp()
        {
            _busManager = A.Fake<IBusManager>();
            _condition = A.Fake<Func<IMessageReceivedContext<ITestMessage>, bool>>();
            _sut = new WhenMessagePublishedSpecification<ITestMessage>(_busManager);
            ((IWhenMessagePublishedConfigurator<ITestMessage>)_sut).Predicate(_condition);
        }

        [Test]
        public void Given_Spec_When_Configure_Then_EndpointConfigured()
        {
            var timeSpanFactory = A.Fake<IConditionInstanceTimeSpanFactory>();
            var configurator = A.Fake<IReceiveEndpointConfigurator>();
            var _verifySut = (IMtVerifySpecification)_sut;
            _verifySut.Configure(timeSpanFactory, configurator);
            A.CallTo(() => configurator.AddEndpointSpecification(A<IReceiveEndpointSpecification>._)).MustHaveHappened();
        }

        [Test]
        public async Task Given_Spec_When_BuildAndEvaluatorDisposed_Then_MonitoringManaged()
        {
            var handle = A.Fake<IVerificationHandle>();
            var timeSpanFactory = A.Fake<IConditionInstanceTimeSpanFactory>();
            A.CallTo(() => _busManager.ApplyVerificationAsync(timeSpanFactory, A<IMtVerifySpecification>._)).Returns(handle);

            await using (var evaluator = await _sut.BuildAsync(timeSpanFactory))
            {
                A.CallTo(() => _busManager.ApplyVerificationAsync(timeSpanFactory, A<IMtVerifySpecification>._)).MustHaveHappenedOnceExactly();
                A.CallTo(() => handle.DisposeAsync()).MustNotHaveHappened();
            }
            await Task.Delay(50);
            A.CallTo(() => handle.DisposeAsync()).MustHaveHappenedOnceExactly();
        }
    }
}
