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
            Assert.Throws<ArgumentNullException>(() => spec.Build(null));
        }

        [Test]
        public void Given_NotConfigured_When_Build_Then_Throw()
        {

            // Arrange
            var spec = new ExpectationSpecification();
            var component = A.Dummy<IComponentConditionEvaluator>();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => spec.Build(component));
        }
    }
}
