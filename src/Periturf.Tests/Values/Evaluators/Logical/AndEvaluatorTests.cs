//
//   Copyright 2021 Adam Burton (adz21c@gmail.com)
//   
//   Licensed under the Apache License, Version 2.0 (the "License")
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//  
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
//  
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
using NUnit.Framework;
using Periturf.Values.Evaluators;
using Periturf.Values.Evaluators.Logical;

namespace Periturf.Tests.Values.Evaluators.Logical
{
    class AndEvaluatorTests
    {
        [TestCase(true, true, true)]
        [TestCase(true, false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, false, false)]
        public async Task Given_Criterias_When_And_Then_Result(bool criteriaOneResult, bool criteriaTwoResult, bool sutResult)
        {
            var criteriaOne = A.Fake<Func<object, ValueTask<bool>>>();
            A.CallTo(() => criteriaOne.Invoke(A<object>._)).Returns(criteriaOneResult);
            var criteriaOneSpec = A.Fake<IValueEvaluatorSpecification<object>>();
            A.CallTo(() => criteriaOneSpec.Build()).Returns(criteriaOne);

            var criteriaTwo = A.Fake<Func<object, ValueTask<bool>>>();
            A.CallTo(() => criteriaTwo.Invoke(A<object>._)).Returns(criteriaTwoResult);
            var criteriaTwoSpec = A.Fake<IValueEvaluatorSpecification<object>>();
            A.CallTo(() => criteriaTwoSpec.Build()).Returns(criteriaTwo);

            var spec = new AndEvaluatorSpecification<object>(criteriaOneSpec, criteriaTwoSpec);
            var sut = spec.Build();

            var result = await sut(new object());

            Assert.That(result, Is.EqualTo(sutResult));

        }
    }
}
