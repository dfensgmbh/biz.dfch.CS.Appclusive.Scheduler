/**
 * Copyright 2016 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Utilities.Testing;

namespace biz.dfch.CS.Appclusive.Scheduler.Public.Tests
{
    [TestClass]
    public class DictionaryParametersTest
    {
        class ArbitraryObject : BaseDto
        {
            public string StringProperty { get; set; }
            public int IntProperty { get; set; }
            public NetworkCredential NetworkCredentialProperty { get; set; }
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ConvertToDictionaryParametersThrowsContractException()
        {
            // Arrange
            var arbitraryObject = default(ArbitraryObject);

            var sut = new DictionaryParameters();

            // Act
            var result = sut.Convert(arbitraryObject);

            // Assert
            // N/A
        }

        [TestMethod]
        public void ConvertToDictionaryParametersSucceeds()
        {
            // Arrange
            var stringProperty = "arbitrary-string";
            var intProperty = 42;
            var networkCredentialProperty = new NetworkCredential("arbitrary-user", "arbitrary-password", "arbitrary-domain");

            var arbitraryObject = new ArbitraryObject();
            arbitraryObject.StringProperty = stringProperty;
            arbitraryObject.IntProperty = intProperty;
            arbitraryObject.NetworkCredentialProperty = networkCredentialProperty;

            var sut = new DictionaryParameters();

            // Act
            var result = sut.Convert(arbitraryObject);

            // Assert
            Assert.AreEqual(arbitraryObject.StringProperty, result["StringProperty"]);
            Assert.AreEqual(arbitraryObject.IntProperty, result["IntProperty"]);
            Assert.AreEqual(arbitraryObject.NetworkCredentialProperty, result["NetworkCredentialProperty"]);
        }

        [TestMethod]
        public void ConvertToDictionaryParametersAndBackSucceeds()
        {
            // Arrange
            var stringProperty = "arbitrary-string";
            var intProperty = 42;
            var networkCredentialProperty = new NetworkCredential("arbitrary-user", "arbitrary-password", "arbitrary-domain");

            var arbitraryObject = new ArbitraryObject();
            arbitraryObject.StringProperty = stringProperty;
            arbitraryObject.IntProperty = intProperty;
            arbitraryObject.NetworkCredentialProperty = networkCredentialProperty;

            var sut = new DictionaryParameters();

            // Act
            var resultDictionaryParameters = sut.Convert(arbitraryObject);
            var result = resultDictionaryParameters.Convert<ArbitraryObject>();

            // Assert
            Assert.AreEqual(arbitraryObject.StringProperty, result.StringProperty);
            Assert.AreEqual(arbitraryObject.IntProperty, result.IntProperty);
            Assert.AreEqual(arbitraryObject.NetworkCredentialProperty, result.NetworkCredentialProperty);
        }

    }
}
