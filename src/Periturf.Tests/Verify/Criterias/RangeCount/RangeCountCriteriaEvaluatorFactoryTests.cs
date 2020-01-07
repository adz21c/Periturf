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
using Periturf.Verify.Criterias.RangeCount;

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

            Assert.That(evaluator, Is.Not.Null);
            
            var typedEvaluator = evaluator as RangeCountCriteriaEvaluator;
            Assert.That(typedEvaluator, Is.Not.Null);

            Assert.That(typedEvaluator.Minimum, Is.EqualTo(minimum));
            Assert.That(typedEvaluator.Maximum, Is.EqualTo(maximum));
        }

        [Test]
        public void Given_Factory_When_CreateInstanceTwice_Then_MultipleInstances()
        {
            const int minimum = 1;
            const int maximum = 2;
            var evaluatorFactory = new RangeCountCriteriaEvaluatorFactory(minimum, maximum);

            var evaluator = evaluatorFactory.CreateInstance();
            var evaluator2 = evaluatorFactory.CreateInstance();

            Assert.That(evaluator, Is.Not.Null);
            Assert.That(evaluator2, Is.Not.Null);
            Assert.AreNotSame(evaluator, evaluator2);
        }
    }
}
