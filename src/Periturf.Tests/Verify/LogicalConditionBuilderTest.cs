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
using Periturf.Verify.Evaluators;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Periturf.Tests.Verify
{
    [TestFixture]
    class LogicalConditionBuilderTest
    {
        [Test]
        public async Task Given_ChildConditions_When_And_Then_AndSpecificationCreated()
        {
            // Arrange
            var context = A.Dummy<IConditionContext>();

            var spec1 = A.Dummy<IConditionSpecification>();
            var config1 = A.Dummy<Func<IConditionContext, IConditionSpecification>>();
            A.CallTo(() => config1.Invoke(context)).Returns(spec1);

            var spec2 = A.Dummy<IConditionSpecification>();
            var config2 = A.Dummy<Func<IConditionContext, IConditionSpecification>>();
            A.CallTo(() => config2.Invoke(context)).Returns(spec2);

            // Act
            var parentSpec = context.And(config1, config2);

            // Assert
            Assert.IsNotNull(parentSpec);
            Assert.AreEqual(typeof(AndConditionSpecification), parentSpec.GetType());
            A.CallTo(() => config1.Invoke(context)).MustHaveHappened();
            A.CallTo(() => config2.Invoke(context)).MustHaveHappened();
        }
    }
}
