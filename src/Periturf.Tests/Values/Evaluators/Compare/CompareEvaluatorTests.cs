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
using Periturf.Values;

namespace Periturf.Tests.Values.Evaluators.Compare
{
    class CompareEvaluatorTests
    {
        [TestCase(1, 0, false)]
        [TestCase(1, 1, true)]
        [TestCase(1, 2, false)]
        [TestCase(2, 1, false)]
        public async Task Given_Values_When_EqualTo_Then_Result(int leftVal, int rightVal, bool sutResult)
        {
            var left = A.Fake<Func<object, ValueTask<int>>>();
            A.CallTo(() => left.Invoke(A<object>._)).Returns(leftVal);
            var leftSpec = A.Fake<IValueProviderSpecification<object, int>>();
            A.CallTo(() => leftSpec.Build()).Returns(left);

            var right = A.Fake<Func<object, ValueTask<int>>>();
            A.CallTo(() => right.Invoke(A<object>._)).Returns(rightVal);
            var rightSpec = A.Fake<IValueProviderSpecification<object, int>>();
            A.CallTo(() => rightSpec.Build()).Returns(right);

            var spec = leftSpec.EqualTo(rightSpec);
            var sut = spec.Build();

            var result = await sut(new object());

            Assert.That(result, Is.EqualTo(sutResult));
        }

        [TestCase(1, 0, false)]
        [TestCase(1, 1, false)]
        [TestCase(1, 2, true)]
        [TestCase(2, 1, false)]
        public async Task Given_Values_When_LessThan_Then_Result(int leftVal, int rightVal, bool sutResult)
        {
            var left = A.Fake<Func<object, ValueTask<int>>>();
            A.CallTo(() => left.Invoke(A<object>._)).Returns(leftVal);
            var leftSpec = A.Fake<IValueProviderSpecification<object, int>>();
            A.CallTo(() => leftSpec.Build()).Returns(left);

            var right = A.Fake<Func<object, ValueTask<int>>>();
            A.CallTo(() => right.Invoke(A<object>._)).Returns(rightVal);
            var rightSpec = A.Fake<IValueProviderSpecification<object, int>>();
            A.CallTo(() => rightSpec.Build()).Returns(right);

            var spec = leftSpec.LessThan(rightSpec);
            var sut = spec.Build();

            var result = await sut(new object());

            Assert.That(result, Is.EqualTo(sutResult));
        }
    }
}
