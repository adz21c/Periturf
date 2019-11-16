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
using System;

namespace Periturf.Tests.Verify.Criterias.RangeCount
{
    [TestFixture]
    class RangeCountCriteriaEvaluatorTests
    {
        // Minimum
        [TestCase(0, null, 0, true, ExpectedResult = true, TestName = "Given_Minimum0_When_NoEvaluate_Then_True")]
        [TestCase(0, null, 1, true, ExpectedResult = true, TestName = "Given_Minimum0_When_Evaluate_Then_True")]
        [TestCase(1, null, 0, false, ExpectedResult = false, TestName = "Given_Minimum1_When_NoEvaluate_Then_False")]
        [TestCase(1, null, 1, true, ExpectedResult = true, TestName = "Given_Minimum1_When_Evaluate_Then_True")]
        [TestCase(1, null, 2, true, ExpectedResult = true, TestName = "Given_Minimum1_When_EvaluateTwice_Then_True")]
        [TestCase(2, null, 0, false, ExpectedResult = false, TestName = "Given_Minimum2_When_NoEvaluate_Then_False")]
        [TestCase(2, null, 1, false, ExpectedResult = false, TestName = "Given_Minimum2_When_Evaluate_Then_False")]
        [TestCase(2, null, 2, true, ExpectedResult = true, TestName = "Given_Minimum2_When_EvaluateTwice_Then_True")]
        [TestCase(2, null, 3, true, ExpectedResult = true, TestName = "Given_Minimum2_When_EvaluateThreeTimes_Then_True")]
        // Maximum
        [TestCase(null, 0, 0, false, ExpectedResult = true, TestName = "Given_Maximum0_When_NoEvaluate_Then_True")]
        [TestCase(null, 0, 1, true, ExpectedResult = false, TestName = "Given_Maximum0_When_Evaluate_Then_False")]
        [TestCase(null, 1, 0, false, ExpectedResult = true, TestName = "Given_Maximum1_When_NoEvaluate_Then_True")]
        [TestCase(null, 1, 1, false, ExpectedResult = true, TestName = "Given_Maximum1_When_Evaluate_Then_True")]
        [TestCase(null, 1, 2, true, ExpectedResult = false, TestName = "Given_Maximum1_When_EvaluateTwice_Then_False")]
        [TestCase(null, 2, 0, false, ExpectedResult = true, TestName = "Given_Maximum2_When_NoEvaluate_Then_True")]
        [TestCase(null, 2, 1, false, ExpectedResult = true, TestName = "Given_Maximum2_When_Evaluate_Then_True")]
        [TestCase(null, 2, 2, false, ExpectedResult = true, TestName = "Given_Maximum2_When_EvaluateTwice_Then_True")]
        [TestCase(null, 2, 3, true, ExpectedResult = false, TestName = "Given_Maximum2_When_EvaluateThreeTimes_Then_False")]
        // Range
        [TestCase(2, 4, 0, false, ExpectedResult = false, TestName = "Given_2To4_When_0_Then_False")]
        [TestCase(2, 4, 1, false, ExpectedResult = false, TestName = "Given_2To4_When_1_Then_False")]
        [TestCase(2, 4, 2, false, ExpectedResult = true, TestName = "Given_2To4_When_2_Then_True")]
        [TestCase(2, 4, 3, false, ExpectedResult = true, TestName = "Given_2To4_When_3_Then_True")]
        [TestCase(2, 4, 4, false, ExpectedResult = true, TestName = "Given_2To4_When_4_Then_True")]
        [TestCase(2, 4, 5, true, ExpectedResult = false, TestName = "Given_2To4_When_5_Then_False")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Blocker Code Smell", "S2699:Tests should include assertions", Justification = "<Pending>")]
        public bool? Given_Config_When_EvaluateXTimes_Then_Result(int? minimum, int? maximum, int numberOfInstances, bool completed)
        {
            var start = DateTime.Now;
            var evaluator = new RangeCountCriteriaEvaluator(minimum, maximum);

            var hasCompleted = completed;
            for (int i = 0; i < numberOfInstances; ++i)
                hasCompleted = evaluator.Evaluate(
                    new ConditionInstance(
                        DateTime.Now - start,
                        i.ToString()));

            Assert.AreEqual(completed, hasCompleted);
            return evaluator.Met;
        }
    }
}
