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
using NUnit.Framework;
using Periturf.Verify;
using Periturf.Verify.ComponentConditions;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify.ComponentConditions
{
    [TestFixture]
    class ComponentMonitorSpecificationTests
    {
        private IConditionInstanceTimeSpanFactory _timeSpanFactory;
        private ComponentMonitorSpecification _sut;

        [SetUp]
        public void SetUp()
        {
            _timeSpanFactory = A.Dummy<IConditionInstanceTimeSpanFactory>();
            _sut = A.Fake<ComponentMonitorSpecification>();
        }

        [Test]
        public async Task Given_Spec_When_EvaluatorCreated_Then_MonitorStarted()
        {
            var evaluator = await _sut.BuildAsync(_timeSpanFactory);

            Assert.That(evaluator, Is.Not.Null);
            A.CallTo(_sut).Where(x => x.Method.Name == "StartMonitorAsync").MustHaveHappenedOnceExactly();

            await evaluator.DisposeAsync();
        }

        [Test]
        public async Task Given_Evaluator_When_EvaluatorDisposed_Then_MonitorStopped()
        {
            var evaluator = await _sut.BuildAsync(_timeSpanFactory);
            Assume.That(evaluator, Is.Not.Null);
            await evaluator.DisposeAsync();
            await Task.Delay(50);

            A.CallTo(_sut).Where(x => x.Method.Name == "StopMonitorAsync").MustHaveHappenedOnceExactly();
        }

        [Test]
        public async Task Given_Spec_When_MultipleEvaluatorCreated_Then_MonitorStartedOnceOnFirst()
        {
            var evaluator1 = await _sut.BuildAsync(_timeSpanFactory);
            A.CallTo(_sut).Where(x => x.Method.Name == "StartMonitorAsync").MustHaveHappenedOnceExactly();
            
            var evaluator2 = await _sut.BuildAsync(_timeSpanFactory);
            A.CallTo(_sut).Where(x => x.Method.Name == "StartMonitorAsync").MustHaveHappenedOnceExactly();

            Assert.That(evaluator1, Is.Not.Null);
            Assert.That(evaluator2, Is.Not.Null);

            await evaluator1.DisposeAsync();
            await evaluator2.DisposeAsync();
        }



        [Test]
        public async Task Given_Spec_When_MultipleEvaluatorDisposed_Then_MonitorStopppedOnceOnLast()
        {
            var evaluator1 = await _sut.BuildAsync(_timeSpanFactory);
            var evaluator2 = await _sut.BuildAsync(_timeSpanFactory);

            Assume.That(evaluator1, Is.Not.Null);
            Assume.That(evaluator2, Is.Not.Null);

            await evaluator1.DisposeAsync();
            A.CallTo(_sut).Where(x => x.Method.Name == "StopMonitorAsync").MustNotHaveHappened();

            await evaluator2.DisposeAsync();
            await Task.Delay(100);
            A.CallTo(_sut).Where(x => x.Method.Name == "StopMonitorAsync").MustHaveHappenedOnceExactly();
        }
    }
}
