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
using NUnit.Framework;
using Periturf.Verify.Criterias;

namespace Periturf.Tests.Verify.Criterias.RangeCount
{
    [TestFixture]
    class RangeCountCriteriaEvaluatorFactoryTests
    {
        [Test]
        public void Given_Factory_When_CreateInstance_Then_ConfigurationPassedOn()
        {
            const int minimum = 1;
            const int maximum = 2;
            var evaluatorFactory = new RangeCountCriteriaEvaluatorFactory(minimum, maximum);

            var evaluator = evaluatorFactory.CreateInstance();

            Assert.IsNotNull(evaluator);
            
            var typedEvaluator = evaluator as RangeCountCriteriaEvaluator;
            Assert.IsNotNull(typedEvaluator);

            Assert.AreEqual(minimum, typedEvaluator.Minimum);
            Assert.AreEqual(maximum, typedEvaluator.Maximum);
        }

        [Test]
        public void Given_Factory_When_CreateInstanceTwice_Then_MultipleInstances()
        {
            const int minimum = 1;
            const int maximum = 2;
            var evaluatorFactory = new RangeCountCriteriaEvaluatorFactory(minimum, maximum);

            var evaluator = evaluatorFactory.CreateInstance();
            var evaluator2 = evaluatorFactory.CreateInstance();

            Assert.IsNotNull(evaluator);
            Assert.IsNotNull(evaluator2);
            Assert.AreNotSame(evaluator, evaluator2);
        }
    }
}
