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
using Periturf.Components;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Periturf.Setup;

namespace Periturf.Tests
{
    [TestFixture]
    class EnvironmentStartAndStopTests
    {
        [Test]
        public void Given_EnvironmentWithOneHost_When_Start_Then_HostStarts()
        {
            // Arrange
            var host = A.Fake<IHost>();
            A.CallTo(() => host.StartAsync(A<CancellationToken>._)).Returns(Task.CompletedTask);

            var hostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => hostSpec.Build()).Returns(host);

            var environment = Environment.Setup(s =>
            {
                s.AddHostSpecification(hostSpec);
            });

            // Act
            Assert.DoesNotThrowAsync(() => environment.StartAsync());

            // Assert
            A.CallTo(() => host.StartAsync(A<CancellationToken>._)).MustHaveHappened();
        }

        [Test]
        public void Given_EnvironmentWithMultipleHosts_When_Start_Then_HostsStart()
        {
            // Arrange
            var host = A.Fake<IHost>();
            A.CallTo(() => host.StartAsync(A<CancellationToken>._)).Returns(Task.CompletedTask);
            var hostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => hostSpec.Build()).Returns(host);

            var host2 = A.Fake<IHost>();
            A.CallTo(() => host2.StartAsync(A<CancellationToken>._)).Returns(Task.CompletedTask);
            var hostSpec2 = A.Fake<IHostSpecification>();
            A.CallTo(() => hostSpec2.Build()).Returns(host2);

            var environment = Environment.Setup(s =>
            {
                s.AddHostSpecification(hostSpec);
                s.AddHostSpecification(hostSpec2);
            });

            // Act
            Assert.DoesNotThrowAsync(() => environment.StartAsync());

            // Assert
            A.CallTo(() => host.StartAsync(A<CancellationToken>._)).MustHaveHappened();
            A.CallTo(() => host2.StartAsync(A<CancellationToken>._)).MustHaveHappened();
        }

        [Test]
        public void Given_EnvironmentWithMultipleHosts_When_StartFails_Then_Throw()
        {
            var startedHost = A.Fake<IHost>();
            A.CallTo(() => startedHost.StartAsync(A<CancellationToken>._)).Returns(Task.CompletedTask);
            var startedHostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => startedHostSpec.Build()).Returns(startedHost);

            var startedHost2 = A.Fake<IHost>();
            A.CallTo(() => startedHost2.StartAsync(A<CancellationToken>._)).Returns(Task.CompletedTask);
            var startedHost2Spec = A.Fake<IHostSpecification>();
            A.CallTo(() => startedHost2Spec.Build()).Returns(startedHost2);

            var failingHostException = new Exception();
            var failingHost = A.Fake<IHost>();
            // Throws immediately
            A.CallTo(() => failingHost.StartAsync(A<CancellationToken>._)).Throws(failingHostException);
            var failingHostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => failingHostSpec.Build()).Returns(failingHost);

            var failingHostException2 = new Exception();
            var failingHost2 = A.Fake<IHost>();
            // Throws via task
            A.CallTo(() => failingHost2.StartAsync(A<CancellationToken>._)).ThrowsAsync(failingHostException2);
            var failingHost2Spec = A.Fake<IHostSpecification>();
            A.CallTo(() => failingHost2Spec.Build()).Returns(failingHost2);

            var environment = Environment.Setup(x =>
            {
                x.AddHostSpecification(startedHostSpec);
                x.AddHostSpecification(startedHost2Spec);
                x.AddHostSpecification(failingHostSpec);
                x.AddHostSpecification(failingHost2Spec);
            });

            var exception = Assert.ThrowsAsync<EnvironmentStartException>(() => environment.StartAsync());

            Assert.That(exception.Details, Is.Not.Null);
            Assert.That(exception.Details.Length, Is.EqualTo(2));

            Assert.That(exception.Details.Any(x => x.Exception == failingHostException), $"{nameof(failingHost)} is missing from the exception details");
            Assert.That(exception.Details.Any(x => x.Exception == failingHostException2), $"{nameof(failingHost2)} is missing from the exception details");

            A.CallTo(() => startedHost.StartAsync(A<CancellationToken>._)).MustHaveHappened();
            A.CallTo(() => startedHost2.StartAsync(A<CancellationToken>._)).MustHaveHappened();
            A.CallTo(() => failingHost.StartAsync(A<CancellationToken>._)).MustHaveHappened();
            A.CallTo(() => failingHost2.StartAsync(A<CancellationToken>._)).MustHaveHappened();
        }

        [Test]
        public void Given_EnvironmentWithOneHost_When_Stop_Then_HostStops()
        {
            // Arrange
            var host = A.Fake<IHost>();
            A.CallTo(() => host.StopAsync(A<CancellationToken>._)).Returns(Task.CompletedTask);

            var hostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => hostSpec.Build()).Returns(host);

            var environment = Environment.Setup(s =>
            {
                s.AddHostSpecification(hostSpec);
            });

            // Act
            Assert.DoesNotThrowAsync(() => environment.StopAsync());

            // Assert
            A.CallTo(() => host.StopAsync(A<CancellationToken>._)).MustHaveHappened();
        }

        [Test]
        public void Given_EnvironmentWithMultipleHosts_When_Stop_Then_HostsStop()
        {
            // Arrange
            var host = A.Fake<IHost>();
            A.CallTo(() => host.StopAsync(A<CancellationToken>._)).Returns(Task.CompletedTask);

            var hostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => hostSpec.Build()).Returns(host);

            var host2 = A.Fake<IHost>();
            A.CallTo(() => host2.StopAsync(A<CancellationToken>._)).Returns(Task.CompletedTask);

            var hostSpec2 = A.Fake<IHostSpecification>();
            A.CallTo(() => hostSpec2.Build()).Returns(host2);

            var environment = Environment.Setup(s =>
            {
                s.AddHostSpecification(hostSpec);
                s.AddHostSpecification(hostSpec2);
            });

            // Act
            Assert.DoesNotThrowAsync(() => environment.StopAsync());

            // Assert
            A.CallTo(() => host.StopAsync(A<CancellationToken>._)).MustHaveHappened();
            A.CallTo(() => host2.StopAsync(A<CancellationToken>._)).MustHaveHappened();
        }

        [Test]
        public void Given_EnvironmentWithMultipleHosts_When_StopFails_Then_Throw()
        {
            var stoppedHost = A.Fake<IHost>();
            A.CallTo(() => stoppedHost.StopAsync(A<CancellationToken>._)).Returns(Task.CompletedTask);
            var stoppedHostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => stoppedHostSpec.Build()).Returns(stoppedHost);

            var stoppedHost2 = A.Fake<IHost>();
            A.CallTo(() => stoppedHost2.StopAsync(A<CancellationToken>._)).Returns(Task.CompletedTask);
            var stoppedHost2Spec = A.Fake<IHostSpecification>();
            A.CallTo(() => stoppedHost2Spec.Build()).Returns(stoppedHost2);

            var failingHostException = new Exception();
            var failingHost = A.Fake<IHost>();
            // Throws immediately
            A.CallTo(() => failingHost.StopAsync(A<CancellationToken>._)).Throws(failingHostException);
            var failingHostSpec = A.Fake<IHostSpecification>();
            A.CallTo(() => failingHostSpec.Build()).Returns(failingHost);

            var failingHostException2 = new Exception();
            var failingHost2 = A.Fake<IHost>();
            // Throws via a task
            A.CallTo(() => failingHost2.StopAsync(A<CancellationToken>._)).ThrowsAsync(failingHostException2);
            var failingHost2Spec = A.Fake<IHostSpecification>();
            A.CallTo(() => failingHost2Spec.Build()).Returns(failingHost2);

            var environment = Environment.Setup(x =>
            {
                x.AddHostSpecification(stoppedHostSpec);
                x.AddHostSpecification(stoppedHost2Spec);
                x.AddHostSpecification(failingHostSpec);
                x.AddHostSpecification(failingHost2Spec);
            });

            var exception = Assert.ThrowsAsync<EnvironmentStopException>(() => environment.StopAsync());

            Assert.That(exception.Details, Is.Not.Null);
            Assert.That(exception.Details.Length, Is.EqualTo(2));

            Assert.That(exception.Details.Any(x => x.Exception == failingHostException), $"{nameof(failingHost)} is missing from the exception details");
            Assert.That(exception.Details.Any(x => x.Exception == failingHostException2), $"{nameof(failingHost2)} is missing from the exception details");

            A.CallTo(() => stoppedHost.StopAsync(A<CancellationToken>._)).MustHaveHappened();
            A.CallTo(() => stoppedHost2.StopAsync(A<CancellationToken>._)).MustHaveHappened();
            A.CallTo(() => failingHost.StopAsync(A<CancellationToken>._)).MustHaveHappened();
            A.CallTo(() => failingHost2.StopAsync(A<CancellationToken>._)).MustHaveHappened();
        }
    }
}
