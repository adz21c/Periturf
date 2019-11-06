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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ExpectationSpecificationTimeoutTests
    {
        private ExpectationSpecification _spec;
        private IComponentConditionEvaluator _componentEvaluator;
        private IExpectationCriteriaSpecification _criteriaSpec;

        [SetUp]
        public void SetUp()
        {
            _spec = new ExpectationSpecification();
            var config = (IExpectationConfigurator)_spec;

            var criteriaFactory = A.Fake<IExpectationCriteriaEvaluatorFactory>();

            _criteriaSpec = A.Fake<IExpectationCriteriaSpecification>();
            A.CallTo(() => _criteriaSpec.Build()).Returns(criteriaFactory);

            config.Must(_criteriaSpec);

            _componentEvaluator = A.Dummy<IComponentConditionEvaluator>();
        }

        [Test]
        public void Given_HasCriteriaTimeout_When_Build_Then_TimeoutIsCriteriaTimeout()
        {
            var verifierTimeout = TimeSpan.FromMilliseconds(1000);
            var criteriaTimeout = TimeSpan.FromMilliseconds(500);
            A.CallTo(() => _criteriaSpec.Timeout).Returns(criteriaTimeout);

            var evaluator = _spec.Build(verifierTimeout, _componentEvaluator, string.Empty);

            Assert.AreEqual(criteriaTimeout, evaluator.Timeout);
            Assert.AreNotEqual(verifierTimeout, evaluator.Timeout);
        }

        [Test]
        public void Given_NoCriteriaTimeout_When_Build_Then_TimeoutIsVerifierTimeout()
        {
            var verifierTimeout = TimeSpan.FromMilliseconds(1000);

            var evaluator = _spec.Build(verifierTimeout, _componentEvaluator, string.Empty);

            Assert.AreEqual(verifierTimeout, evaluator.Timeout);
        }
    }
}
