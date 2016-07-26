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

using System.Data.Services.Client;
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Utilities.Testing;

namespace biz.dfch.CS.Appclusive.Scheduler.Public.Tests
{
    [TestClass]
    public class AppclusiveEndpointsTest
    {
        [TestMethod]
        public void AppclusiveEndpointsSucceeds()
        {
            // Arrange
            var baseUri = new Uri("http://www.example.com/Appclusive/api");
            var credential = default(ICredentials);

            // Act
            var sut = new AppclusiveEndpoints(baseUri, credential);

            // Assert
            Assert.IsNotNull(sut.Diagnostics);
            Assert.IsNotNull(sut.Core);
            Assert.IsNotNull(sut.Infrastructure);
            Assert.IsNotNull(sut.Csm);
            Assert.IsNotNull(sut.Cmp);

            Assert.AreEqual(CredentialCache.DefaultNetworkCredentials, sut.Diagnostics.Credentials);
            Assert.AreEqual(CredentialCache.DefaultNetworkCredentials, sut.Core.Credentials);
            Assert.AreEqual(CredentialCache.DefaultNetworkCredentials, sut.Infrastructure.Credentials);
            Assert.AreEqual(CredentialCache.DefaultNetworkCredentials, sut.Csm.Credentials);
            Assert.AreEqual(CredentialCache.DefaultNetworkCredentials, sut.Cmp.Credentials);

            Assert.IsTrue(sut.Diagnostics.BaseUri.AbsoluteUri.StartsWith(baseUri.AbsoluteUri));
            Assert.IsTrue(sut.Diagnostics.BaseUri.AbsoluteUri.Contains("Diagnostics"));
            Assert.IsTrue(sut.Core.BaseUri.AbsoluteUri.StartsWith(baseUri.AbsoluteUri));
            Assert.IsTrue(sut.Core.BaseUri.AbsoluteUri.Contains("Core"));
            Assert.IsTrue(sut.Infrastructure.BaseUri.AbsoluteUri.StartsWith(baseUri.AbsoluteUri));
            Assert.IsTrue(sut.Infrastructure.BaseUri.AbsoluteUri.Contains("Infrastructure"));
            Assert.IsTrue(sut.Csm.BaseUri.AbsoluteUri.StartsWith(baseUri.AbsoluteUri));
            Assert.IsTrue(sut.Csm.BaseUri.AbsoluteUri.Contains("Csm"));
            Assert.IsTrue(sut.Cmp.BaseUri.AbsoluteUri.StartsWith(baseUri.AbsoluteUri));
            Assert.IsTrue(sut.Cmp.BaseUri.AbsoluteUri.Contains("Cmp"));
        }

        [TestMethod]
        public void AppclusiveEndpointsWithExplicitCredentialsSucceeds()
        {
            // Arrange
            var baseUri = new Uri("http://www.example.com/Appclusive/api");
            var credential = new NetworkCredential("arbitrary-user", "arbitrary-password");

            // Act
            var sut = new AppclusiveEndpoints(baseUri, credential);

            // Assert
            Assert.AreEqual(credential, sut.Diagnostics.Credentials);
            Assert.AreEqual(credential, sut.Core.Credentials);
            Assert.AreEqual(credential, sut.Infrastructure.Credentials);
            Assert.AreEqual(credential, sut.Csm.Credentials);
            Assert.AreEqual(credential, sut.Cmp.Credentials);
        }

        [TestMethod]
        public void AppclusiveEndpointsUsesJson()
        {
            // Arrange
            var baseUri = new Uri("http://www.example.com/Appclusive/api");
            var credential = new NetworkCredential("arbitrary-user", "arbitrary-password");

            // Act
            var sut = new AppclusiveEndpoints(baseUri, credential);

            // Assert
            Assert.AreEqual("JsonLight", sut.Diagnostics.Format.ODataFormat.ToString());
            Assert.AreEqual("JsonLight", sut.Core.Format.ODataFormat.ToString());
            Assert.AreEqual("JsonLight", sut.Infrastructure.Format.ODataFormat.ToString());
            Assert.AreEqual("JsonLight", sut.Csm.Format.ODataFormat.ToString());
            Assert.AreEqual("JsonLight", sut.Cmp.Format.ODataFormat.ToString());
        }

        [TestMethod]
        [ExpectContractFailure]
        public void AppclusiveEndpointsWithNullUriThrowsContractException()
        {
            // Arrange
            var baseUri = default(Uri);
            var credential = new NetworkCredential("arbitrary-user", "arbitrary-password");

            // Act
            var sut = new AppclusiveEndpoints(baseUri, credential);

            // Assert
            // N/A
        }
    }
}
