﻿/*
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Periturf.Tests
{
    [TestFixture]
    class DuplicateHostNameExceptionTests
    {
        [Test]
        public void Given_HostName_When_Ctor_Then_ExceptionCreated()
        {
            // Arrange
            const string hostName = "HostName";

            // Act
            var sut = new DuplicateHostNameException(hostName);

            // Assert
            Assert.IsNull(sut.InnerException);
            Assert.AreEqual(hostName, sut.HostName);
            // Has the default message
            Assert.IsNotNull(sut.Message);
            Assert.IsNotEmpty(sut.Message);
        }

        [Test]
        public void Given_CustomMessageAndHostName_When_Ctor_Then_ExceptionCreated()
        {
            // Arrange
            const string hostName = "HostName";
            const string message = "My Custom Mesage";

            // Act
            var sut = new DuplicateHostNameException(message, hostName);

            // Assert
            Assert.IsNull(sut.InnerException);
            Assert.AreEqual(hostName, sut.HostName);
            Assert.AreEqual(message, sut.Message);
        }

        [Test]
        public void Given_AnException_When_SeriaizedAndDeserialized_Then_DataMatchesTheOriginal()
        {
            // Arrange
            const string hostName = "HostName";
            var originalException = new DuplicateHostNameException(hostName);

            var buffer = new byte[4096];
            var ms = new MemoryStream(buffer);
            var ms2 = new MemoryStream(buffer);
            var formatter = new BinaryFormatter();

            // Act
            formatter.Serialize(ms, originalException);
            var deserializedException = (DuplicateHostNameException)formatter.Deserialize(ms2);

            // Assert
            Assert.AreEqual(hostName, deserializedException.HostName);
            Assert.AreEqual(originalException.Message, deserializedException.Message);
        }
    }
}
