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
            Assert.IsNull(sut.InnerException);
            Assert.AreEqual(id, sut.Id);
            // Not host details
            Assert.IsNotNull(sut.Details);
            Assert.IsEmpty(sut.Details);
            // Has the default message
            Assert.IsNotNull(sut.Message);
            Assert.IsNotEmpty(sut.Message);
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
            Assert.IsNull(sut.InnerException);
            // host details
            Assert.AreEqual(componentDetails, sut.Details);
            Assert.AreEqual(id, sut.Id);
            // Has the default message
            Assert.IsNotNull(sut.Message);
            Assert.IsNotEmpty(sut.Message);
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
            Assert.IsNull(sut.InnerException);
            Assert.AreEqual(id, sut.Id);
            // Not host details
            Assert.IsNotNull(sut.Details);
            Assert.IsEmpty(sut.Details);
            Assert.AreEqual(message, sut.Message);
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
            Assert.IsNull(sut.InnerException);
            Assert.AreEqual(id, sut.Id);
            // host details
            Assert.AreEqual(componentDetails, sut.Details);
            Assert.AreEqual(message, sut.Message);
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
            Assert.IsNotNull(deserializedException.Details);
            Assert.IsNotEmpty(deserializedException.Details);

            var orignalComponentExceptionDetails = originalException.Details.Single();
            var deserializedComponentExceptionDetails = deserializedException.Details.Single();
            Assert.AreEqual(orignalComponentExceptionDetails.ComponentName, deserializedComponentExceptionDetails.ComponentName);
            Assert.IsNotNull(deserializedComponentExceptionDetails.Exception);
            Assert.AreEqual(orignalComponentExceptionDetails.Exception.Message, deserializedComponentExceptionDetails.Exception.Message);

            Assert.AreEqual(originalException.Message, deserializedException.Message);
        }
    }
}
