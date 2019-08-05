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
using Periturf.Tests.Verify;

namespace Periturf.Tests.IdSvr4.Verify
{
    [TestFixture]
    class ConditionContextTests
    {
        [Test]
        public void Given_Context_When_GetComponentConditionBuilder_Then_ReturnsBuilder()
        {
            const string componentName = "Component";
            var componentBuilder = A.Dummy<IIdSvr4ConditionBuilder>();
            var context = A.Fake<IConditionContext>();
            A.CallTo(() => context.GetComponentConditionBuilder<IIdSvr4ConditionBuilder>(componentName))
                .Returns(componentBuilder);
            
            // Act
            var builder = context.IdSvr4(componentName);

            // Assert
            Assert.IsNotNull(builder);
            Assert.AreEqual(componentBuilder, builder);
            A.CallTo(() => context.GetComponentConditionBuilder<IIdSvr4ConditionBuilder>(componentName)).MustHaveHappened();
        }
    }
}
