﻿//
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

namespace Periturf.Tests.Values
{
    public class ExpressionValueProviderTests
    {
        [Test]
        public async Task Given_Expression_When_Invoke_Then_ExpressionEvaluated()
        {
            var testModel = new TestModel { SomeProperty = 127 };

            var context = A.Fake<IValueContext<TestModel>>();

            var provider = context.Value(x => x.SomeProperty).Build();

            Assert.That(provider, Is.Not.Null);

            var result = await provider(testModel);

            Assert.That(result, Is.EqualTo(testModel.SomeProperty));
        }

        public class TestModel
        {
            public int SomeProperty { get; set; }
        }
    }
}
