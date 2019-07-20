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
using Periturf.Verify.Evaluators.Logical;

namespace Periturf.Tests.Verify.Evaluators.Logical
{
    [TestFixture]
    class LogicalConditionBuilderTest
    {
        [Test]
        public void Given_ChildConditions_When_And_Then_AndSpecificationCreated()
        {
            // Arrange
            var context = A.Dummy<IConditionContext>();

            var spec1 = A.Dummy<IConditionSpecification>();
            var spec2 = A.Dummy<IConditionSpecification>();

            // Act
            var parentSpec = context.And(spec1, spec2);

            // Assert
            Assert.IsNotNull(parentSpec);
            Assert.AreEqual(typeof(AndConditionSpecification), parentSpec.GetType());
        }

        [Test]
        public void Given_ChildConditions_When_Or_Then_OrSpecificationCreated()
        {
            // Arrange
            var context = A.Dummy<IConditionContext>();

            var spec1 = A.Dummy<IConditionSpecification>();
            var spec2 = A.Dummy<IConditionSpecification>();

            // Act
            var parentSpec = context.Or(spec1, spec2);

            // Assert
            Assert.IsNotNull(parentSpec);
            Assert.AreEqual(typeof(OrConditionSpecification), parentSpec.GetType());
        }

        [Test]
        public void Given_ChildConditions_When_Xor_Then_OrSpecificationCreated()
        {
            // Arrange
            var context = A.Dummy<IConditionContext>();

            var spec1 = A.Dummy<IConditionSpecification>();
            var spec2 = A.Dummy<IConditionSpecification>();

            // Act
            var parentSpec = context.Xor(spec1, spec2);

            // Assert
            Assert.IsNotNull(parentSpec);
            Assert.AreEqual(typeof(XorConditionSpecification), parentSpec.GetType());
        }

        [Test]
        public void Given_ChildConditions_When_Not_Then_OrSpecificationCreated()
        {
            // Arrange
            var context = A.Dummy<IConditionContext>();

            var spec = A.Dummy<IConditionSpecification>();

            // Act
            var parentSpec = context.Not(spec);

            // Assert
            Assert.IsNotNull(parentSpec);
            Assert.AreEqual(typeof(NotConditionSpecification), parentSpec.GetType());
        }
    }
}
