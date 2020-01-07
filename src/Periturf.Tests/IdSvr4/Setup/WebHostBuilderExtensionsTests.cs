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
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Periturf.AspNetCore.Setup;
using Periturf.IdSvr4.Setup;
using System;

namespace Periturf.Tests.IdSvr4.Setup
{
    [TestFixture]
    class WebHostBuilderExtensionsTests
    {
        [Test]
        public void Given_Config_When_SetupIdSvr4_Then_IdentityServerIsSetup()
        {
            // Arrange
            var builder = A.Fake<IPeriturfWebHostBuilder>();
            A.CallTo(() => builder.GetSetting("urls")).Returns("http://localhost:5000/");

            var config = A.Fake<Action<IIdSvr4SetupConfigurator>>();
            A.CallTo(() => config(A<IIdSvr4SetupConfigurator>._))
                .Invokes((IIdSvr4SetupConfigurator c) =>
                {
                    c.Configure(s => { });
                    c.Services(s => { });
                });

            const string componentName = "IdSvr4Test";

            // Act
            builder.IdSvr4(componentName, config);

            // Assert
            A.CallTo(() => builder.ConfigureServices(A<Action<IServiceCollection>>._)).MustHaveHappenedOnceOrMore();
            A.CallTo(() => config.Invoke(A<IdSvr4SetupSpecification>._)).MustHaveHappenedOnceExactly();
        }

        [Test]
        public void Given_NoConfig_When_SetupIdSvr4_Then_IdentityServerIsSetup()
        {
            // Arrange
            var builder = A.Fake<IPeriturfWebHostBuilder>();
            A.CallTo(() => builder.GetSetting("urls")).Returns("http://localhost:5000/");

            const string componentName = "IdSvr4Test";

            // Act
            builder.IdSvr4(componentName, null);

            // Assert
            A.CallTo(() => builder.ConfigureServices(A<Action<IServiceCollection>>._)).MustHaveHappenedOnceOrMore();
        }
    }
}
