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

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class ExpectationSpecificationDescriptionTests
    {
        private const string ConditionDescription = "Condition";
        private const string FilterDescription = "Filter";
        private const string CriteriaDescription = "Criteria";
        
        private TimeSpan _verifierTimeout;
        private IExpectationFilterSpecification _filterSpec;
        private IExpectationCriteriaEvaluator _criteria;
        private IExpectationCriteriaEvaluatorFactory _criteriaFactory;
        private IExpectationCriteriaSpecification _criteriaSpec;
        private MockComponentEvaluator _componentEvaluator;

        [OneTimeSetUp]
        public void InitialSetup()
        {
            _verifierTimeout = TimeSpan.FromMilliseconds(500);

            _filterSpec = A.Fake<IExpectationFilterSpecification>();
            A.CallTo(() => _filterSpec.Description).Returns(FilterDescription);

            _criteria = A.Fake<IExpectationCriteriaEvaluator>();
            _criteriaFactory = A.Fake<IExpectationCriteriaEvaluatorFactory>();
            A.CallTo(() => _criteriaFactory.CreateInstance()).Returns(_criteria);

            _criteriaSpec = A.Fake<IExpectationCriteriaSpecification>();
            A.CallTo(() => _criteriaSpec.Build()).Returns(_criteriaFactory);
            A.CallTo(() => _criteriaSpec.Timeout).Returns(TimeSpan.FromMilliseconds(500));
            A.CallTo(() => _criteriaSpec.Description).Returns(CriteriaDescription);

            _componentEvaluator = new MockComponentEvaluator(TimeSpan.FromMilliseconds(50), 8);
        }

        [Test]
        public void Given_BuiltInDescriptions_When_EvaluatorDescription_Then_DefaultDescription()
        {
            var spec = new ExpectationSpecification();
            var config = (IExpectationConfigurator)spec;

            config.Where(c => c.AddSpecification(_filterSpec))
                .Must(_criteriaSpec);

            var evaluator = spec.Build(_verifierTimeout, _componentEvaluator, ConditionDescription);

            Assert.That(evaluator.Description, Is.EqualTo($"{ConditionDescription} WHERE {FilterDescription} MUST {CriteriaDescription}"));
        }

        [Test]
        public void Given_MultipleDescriptions_When_EvaluatorDescription_Then_DefaultDescription()
        {
            const string FilterDescription2 = "Filter2";

            var spec = new ExpectationSpecification();
            var config = (IExpectationConfigurator)spec;

            var filterSpec2 = A.Fake<IExpectationFilterSpecification>();
            A.CallTo(() => filterSpec2.Description).Returns(FilterDescription2);

            config.Where(c => c.AddSpecification(_filterSpec))
                .Where(c => c.AddSpecification(filterSpec2))
                .Must(_criteriaSpec);

            var evaluator = spec.Build(_verifierTimeout, _componentEvaluator, ConditionDescription);

            Assert.That(evaluator.Description, Is.EqualTo($"{ConditionDescription} WHERE {FilterDescription}, {FilterDescription2} MUST {CriteriaDescription}"));
        }

        [Test]
        public void Given_FilterDescriptionOverride_When_EvaluatorDescription_Then_DescriptionOverride()
        {
            const string FilterDescriptionOverride = "Override";

            var spec = new ExpectationSpecification();
            var config = (IExpectationConfigurator)spec;

            config.Where(c =>
            {
                c.AddSpecification(_filterSpec);
                c.Description(FilterDescriptionOverride);
            })
            .Must(_criteriaSpec);

            var evaluator = spec.Build(_verifierTimeout, _componentEvaluator, ConditionDescription);

            Assert.That(evaluator.Description, Is.EqualTo($"{ConditionDescription} WHERE {FilterDescriptionOverride} MUST {CriteriaDescription}"));
        }

        [Test]
        public void Given_DescriptionOverride_When_EvaluatorDescription_Then_DescriptionOverride()
        {
            const string DescriptionOverride = "Override";

            var spec = new ExpectationSpecification();
            var config = (IExpectationConfigurator)spec;

            config.Description(DescriptionOverride)
                .Where(c => c.AddSpecification(_filterSpec))
                .Must(_criteriaSpec);

            var evaluator = spec.Build(_verifierTimeout, _componentEvaluator, ConditionDescription);

            Assert.That(evaluator.Description, Is.EqualTo(DescriptionOverride));
        }
    }
}
