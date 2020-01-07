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
using Periturf.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace Periturf.Tests.Configuration
{
    [TestFixture]
    class ConfigurationRemovalExceptionTests
    {
        [Test]
        public void Given_NoComponentErrors_When_Ctor_Then_ExceptionCreated()
        {
            // Arrange
            var id = Guid.NewGuid();

            // Act
            var sut = new ConfigurationRemovalException(id);

            // Assert
            Assert.That(sut.InnerException, Is.Null);
            Assert.That(sut.Id, Is.EqualTo(id));
            // Not host details
            Assert.That(sut.Details, Is.Not.Null);
            Assert.That(sut.Details, Is.Empty);
            // Has the default message
            Assert.That(sut.Message, Is.Not.Null);
            Assert.That(sut.Message, Is.Not.Empty);
        }

        [Test]
        public void Given_ComponentErrors_When_Ctor_Then_ExceptionCreated()
        {
            // Arrange
            var id = Guid.NewGuid();
            var componentDetails = new[] { new ComponentExceptionDetails("MyComponent", new Exception()) };

            // Act
            var sut = new ConfigurationRemovalException(id, componentDetails);

            // Assert
            Assert.That(sut.InnerException, Is.Null);
            // host details
            Assert.That(sut.Details, Is.EqualTo(componentDetails));
            Assert.That(sut.Id, Is.EqualTo(id));
            // Has the default message
            Assert.That(sut.Message, Is.Not.Null);
            Assert.That(sut.Message, Is.Not.Empty);
        }
        [Test]
        public void Given_CustomMessageAndNoComponentErrors_When_Ctor_Then_ExceptionCreated()
        {
            // Arrange
            var id = Guid.NewGuid();
            const string message = "My Custom Error Message";

            // Act
            var sut = new ConfigurationRemovalException(message, id);

            // Assert
            Assert.That(sut.InnerException, Is.Null);
            Assert.That(sut.Id, Is.EqualTo(id));
            // Not host details
            Assert.That(sut.Details, Is.Not.Null);
            Assert.That(sut.Details, Is.Empty);
            Assert.That(sut.Message, Is.EqualTo(message));
        }

        [Test]
        public void Given_CustomMessageAndComponentErrors_When_Ctor_Then_ExceptionCreated()
        {
            // Arrange
            const string message = "My Custom Error Message";
            var id = Guid.NewGuid();
            var componentDetails = new[] { new ComponentExceptionDetails("MyComponent", new Exception()) };

            // Act
            var sut = new ConfigurationRemovalException(message, id, componentDetails);

            // Assert
            Assert.That(sut.InnerException, Is.Null);
            Assert.That(sut.Id, Is.EqualTo(id));
            // host details
            Assert.That(sut.Details, Is.EqualTo(componentDetails));
            Assert.That(sut.Message, Is.EqualTo(message));
        }

        [Test]
        public void Given_AnException_When_SeriaizedAndDeserialized_Then_DataMatchesTheOriginal()
        {
            // Arrange
            var id = Guid.NewGuid();
            var componentDetails = new[] { new ComponentExceptionDetails("MyComponent", new Exception("MyMessage")) };
            var originalException = new ConfigurationRemovalException(id, componentDetails);

            var buffer = new byte[4096];
            var ms = new MemoryStream(buffer);
            var ms2 = new MemoryStream(buffer);
            var formatter = new BinaryFormatter();

            // Act
            formatter.Serialize(ms, originalException);
            var deserializedException = (ConfigurationRemovalException)formatter.Deserialize(ms2);

            // Assert
            Assert.That(deserializedException.Details, Is.Not.Null);
            Assert.That(deserializedException.Details, Is.Not.Empty);

            var orignalComponentExceptionDetails = originalException.Details.Single();
            var deserializedComponentExceptionDetails = deserializedException.Details.Single();
            Assert.That(deserializedComponentExceptionDetails.ComponentName, Is.EqualTo(orignalComponentExceptionDetails.ComponentName));
            Assert.That(deserializedComponentExceptionDetails.Exception, Is.Not.Null);
            Assert.That(deserializedComponentExceptionDetails.Exception.Message, Is.EqualTo(orignalComponentExceptionDetails.Exception.Message));

            Assert.That(deserializedException.Message, Is.EqualTo(originalException.Message));
        }
    }
}
