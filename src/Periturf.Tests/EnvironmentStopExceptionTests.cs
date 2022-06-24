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

using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;

namespace Periturf.Tests
{
    [TestFixture]
    class EnvironmentStopExceptionTests
    {
        [Test]
        public void Given_NoHostErrors_When_Ctor_Then_ExceptionCreated()
        {
            // Act
            var sut = new EnvironmentStopException();

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
            var hostDetails = new[] { new HostExceptionDetails(new[] { new Exception() }) };

            // Act
            var sut = new EnvironmentStopException(hostDetails);

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
            var sut = new EnvironmentStopException(message);

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
            var hostDetails = new[] { new HostExceptionDetails(new[] { new Exception() }) };

            // Act
            var sut = new EnvironmentStopException(message, hostDetails);

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
            var hostDetails = new[] { new HostExceptionDetails(new[] { new Exception("MyMessage") }) };
            var originalException = new EnvironmentStopException(hostDetails);

            var buffer = new byte[4096];
            var ms = new MemoryStream(buffer);
            var ms2 = new MemoryStream(buffer);
            var formatter = new BinaryFormatter();

            // Act
#pragma warning disable SYSLIB0011 // Type or member is obsolete
            formatter.Serialize(ms, originalException);
            var deserializedException = (EnvironmentStopException)formatter.Deserialize(ms2);
#pragma warning restore SYSLIB0011 // Type or member is obsolete

            // Assert
            Assert.That(deserializedException.Details, Is.Not.Null);
            Assert.That(deserializedException.Details, Is.Not.Empty);

            var orignalHostExceptionDetails = originalException.Details.Single();
            var deserializedHostExceptionDetails = deserializedException.Details.Single();
            Assert.That(deserializedHostExceptionDetails.Exceptions, Is.Not.Empty);
            Assert.That(deserializedHostExceptionDetails.Exceptions.First().Message, Is.EqualTo(orignalHostExceptionDetails.Exceptions.First().Message));

            Assert.That(deserializedException.Message, Is.EqualTo(originalException.Message));
        }
    }
}
