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
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Scheduler.Core.Tests
{
    [TestClass]
    public class AppclusiveEndpointsTest
    {
        [TestMethod]
        public void AppclusiveEndpointsSucceeds()
        {
            // Arrange
            var baseUri = new Uri("http://www.example.com/");
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

            Assert.AreEqual(baseUri, sut.Diagnostics.BaseUri);
        }
    }
}
