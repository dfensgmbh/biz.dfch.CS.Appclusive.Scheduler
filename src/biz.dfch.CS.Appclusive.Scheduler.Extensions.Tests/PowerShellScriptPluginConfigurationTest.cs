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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Utilities.Testing;
using System.Net;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions.Tests
{
    [TestClass]
    public class PowerShellScriptPluginConfigurationTest
    {
        [TestMethod]
        public void PowerShellScriptPluginConfigurationPlainIsNotValid()
        {
            // Arrange
            var sut = new PowerShellScriptPluginConfiguration();

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void PowerShellScriptPluginConfigurationIsValid()
        {
            // Arrange
            var sut = new PowerShellScriptPluginConfiguration();
            sut.ScriptBase = "C:\\arbitrary-valid-script-base-directory\\";
            sut.ComputerName = "arbitrary-valid-computername";
            sut.ConfigurationName = "arbitrary-valid-configuration-name";
            var username = "arbitrary-user";
            var password = "arbitrary-password";
            var domain = "arbitrary-domain";
            sut.Credential = new NetworkCredential(username, password, domain);

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsTrue(result);
        }
    }
}
