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
    class NotEvaluatorTests
    {
        [TestCase(true, false)]
        [TestCase(false, true)]
        public async Task Given_Response_When_Not_Then_ResultFlipped(bool nextResult, bool sutResult)
        {
            var next = A.Fake<Func<object, ValueTask<bool>>>();
            A.CallTo(() => next.Invoke(A<object>._)).Returns(nextResult);
            var nextSpec = A.Fake<IValueEvaluatorSpecification<object>>();
            A.CallTo(() => nextSpec.Build()).Returns(next);

            var builder = A.Dummy<IValueEvaluatorBuilder<object>>();

            var notBuilder = builder.Not();
            notBuilder.AddNextEvaluatorSpecification(nextSpec);
            var sut = notBuilder.Build();

            var result = await sut(new object());

            Assert.That(result, Is.EqualTo(sutResult));
            A.CallTo(() => builder.AddNextEvaluatorSpecification(notBuilder)).MustHaveHappened();
        }
    }
}
