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
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Periturf.Tests
{
    [TestFixture]
    class EnvironmentStartExceptionTests
    {
        [Test]
        public void Given_NoHostErrors_When_Ctor_Then_ExceptionCreated()
        {
            // Act
            var sut = new EnvironmentStartException();

            // Assert
            Assert.That(sut.InnerException, Is.Null);
            // Not host details
            Assert.That(sut.Details, Is.Not.Null);
            Assert.That(sut.Details, Is.Empty);
            // Has the default message
            Assert.That(sut.Message, Is.Not.Null);
            Assert.That(sut.Message, Is.Not.Empty);
        }

        [Test]
        public void Given_HostErrors_When_Ctor_Then_ExceptionCreated()
        {
            // Arrange
            var hostDetails = new[] { new HostExceptionDetails(new Exception()) };

            // Act
            var sut = new EnvironmentStartException(hostDetails);

            // Assert
            Assert.That(sut.InnerException, Is.Null);
            // host details
            Assert.That(sut.Details, Is.EqualTo(hostDetails));
            // Has the default message
            Assert.That(sut.Message, Is.Not.Null);
            Assert.That(sut.Message, Is.Not.Empty);
        }

        [Test]
        public void Given_CustomMessageAndNoHostErrors_When_Ctor_Then_ExceptionCreated()
        {
            // Arrange
            const string message = "My Custom Error Message";

            // Act
            var sut = new EnvironmentStartException(message);

            // Assert
            Assert.That(sut.InnerException, Is.Null);
            // Not host details
            Assert.That(sut.Details, Is.Not.Null);
            Assert.That(sut.Details, Is.Empty);
            Assert.That(sut.Message, Is.EqualTo(message));
        }

        [Test]
        public void Given_CustomMessageAndHostErrors_When_Ctor_Then_ExceptionCreated()
        {
            // Arrange
            const string message = "My Custom Error Message";
            var hostDetails = new[] { new HostExceptionDetails(new Exception()) };

            // Act
            var sut = new EnvironmentStartException(message, hostDetails);

            // Assert
            Assert.That(sut.InnerException, Is.Null);
            // Not host details
            Assert.That(sut.Details, Is.EqualTo(hostDetails));
            Assert.That(sut.Message, Is.EqualTo(message));
        }

        [Test]
        public void Given_AnException_When_SeriaizedAndDeserialized_Then_DataMatchesTheOriginal()
        {
            // Arrange
            var hostDetails = new[] { new HostExceptionDetails(new Exception("MyMessage")) };
            var originalException = new EnvironmentStartException(hostDetails);

            var buffer = new byte[4096];
            var ms = new MemoryStream(buffer);
            var ms2 = new MemoryStream(buffer);
            var formatter = new BinaryFormatter();

            // Act
            formatter.Serialize(ms, originalException);
            var deserializedException = (EnvironmentStartException)formatter.Deserialize(ms2);

            // Assert
            Assert.That(deserializedException.Details, Is.Not.Null);
            Assert.That(deserializedException.Details, Is.Not.Empty);

            var orignalHostExceptionDetails = originalException.Details.Single();
            var deserializedHostExceptionDetails = deserializedException.Details.Single();
            Assert.That(deserializedHostExceptionDetails.Exception, Is.Not.Null);
            Assert.That(deserializedHostExceptionDetails.Exception.Message, Is.EqualTo(orignalHostExceptionDetails.Exception.Message));

            Assert.That(deserializedException.Message, Is.EqualTo(originalException.Message));
        }
    }
}
