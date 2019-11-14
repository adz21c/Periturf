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
    class ExpectationSpecificationTests
    {
        [Test]
        public void Given_NullConfigurator_When_Where_Then_Throw()
        {
            // Arrange
            var spec = (IExpectationConfigurator)new ExpectationSpecification();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => spec.Where(null));
        }

        [Test]
        public void Given_NullSpecification_When_Must_Then_Throw()
        {
            // Arrange
            var spec = (IExpectationConfigurator)new ExpectationSpecification();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => spec.Must(null));
        }

        [Test]
        public void Given_NullSpecification_When_FilterAddSpecification_Then_Throw()
        {

            // Arrange
            var spec = (IExpectationFilterConfigurator)new ExpectationSpecification();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => spec.AddSpecification(null));
        }

        [Test]
        public void Given_NullComponent_When_Build_Then_Throw()
        {

            // Arrange
            var spec = new ExpectationSpecification();

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => spec.Build(TimeSpan.FromMilliseconds(1), null, string.Empty));
        }

        [Test]
        public void Given_NotConfigured_When_Build_Then_Throw()
        {

            // Arrange
            var spec = new ExpectationSpecification();
            var component = A.Dummy<IComponentConditionEvaluator>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => spec.Build(TimeSpan.FromMilliseconds(1), component, string.Empty));
        }

        [Test]
        public async Task Given_Configured_When_Build_Then_EvaluatorBuilt()
        {
            // Arrange
            var verifierTimeout = TimeSpan.FromMilliseconds(500);

            var spec = new ExpectationSpecification();
            var config = (IExpectationConfigurator)spec;

            async IAsyncEnumerable<ConditionInstance> FilterImp(IAsyncEnumerable<ConditionInstance> enumerable)
            {
                await foreach (var item in enumerable)
                {
                    if (item.When >= TimeSpan.FromMilliseconds(200))
                        continue;

                    yield return item;
                }
            }

            var filterSpec = A.Fake<IExpectationFilterSpecification>();
            A.CallTo(() => filterSpec.Build()).Returns(FilterImp);

            bool failed = false;
            var criteria = A.Fake<IExpectationCriteriaEvaluator>();
            A.CallTo(() => criteria.Evaluate(A<ConditionInstance>._)).Invokes((ConditionInstance c) =>
            {
                if (c.When >= TimeSpan.FromMilliseconds(200))
                    failed = true;
            });
            A.CallTo(() => criteria.Met).ReturnsLazily(() => !failed);

            var criteriaFactory = A.Fake<IExpectationCriteriaEvaluatorFactory>();
            A.CallTo(() => criteriaFactory.CreateInstance()).Returns(criteria);

            var criteriaSpec = A.Fake<IExpectationCriteriaSpecification>();
            A.CallTo(() => criteriaSpec.Build()).Returns(criteriaFactory);
            A.CallTo(() => criteriaSpec.Timeout).Returns(TimeSpan.FromMilliseconds(500));

            var componentEvaluator = new MockComponentEvaluator(TimeSpan.FromMilliseconds(50), 8);

            config.Where(c => c.AddSpecification(filterSpec))
                .Must(criteriaSpec);

            var evaluator = spec.Build(verifierTimeout, componentEvaluator, string.Empty);

            var result = await evaluator.EvaluateAsync();

            Assert.NotNull(result);
            Assert.IsTrue(result.Met);
            Assert.IsTrue(result.Completed);
        }
    }
}
