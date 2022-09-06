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
using NUnit.Framework;
using Periturf.Values;

namespace Periturf.Tests.Values
{
    class ConstantValueProviderTests
    {
        [Test]
        public void Given_Value_When_Invoke_Then_ValueReturned()
        {
            const int value = 56;

            var provider = new ConstantValueProviderSpecification<object, int>(value).Build();

            Assert.That(provider, Is.Not.Null);

            var result = provider(new object());

            Assert.That(result, Is.EqualTo(value));
        }

        class TestModel
        {
            public int SomeProperty { get; set; }
        }
    }
}
