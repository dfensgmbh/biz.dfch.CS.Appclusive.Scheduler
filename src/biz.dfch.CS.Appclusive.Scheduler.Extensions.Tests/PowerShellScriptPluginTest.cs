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
using Telerik.JustMock;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Scheduler.Core;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions.Tests
{
    [TestClass]
    public class PowerShellScriptPluginTest
    {
        [TestMethod]
        public void InitialiseAndActivateSucceeds()
        {
            // Arrange
            var logger = new Logger();

            var appclusiveEndpoints = Mock.Create<AppclusiveEndpoints>(Constructor.Mocked);
            var parameters = new DictionaryParameters();
            parameters.Add("Endpoints", appclusiveEndpoints);
            parameters.Add("ConfigurationName", "AppclusiveScriptInvocation");
            parameters.Add("ComputerName", "localhost");
            parameters.Add("ScriptBase", "C:\\arbitrary-script-base-directory");
            parameters.Add("Credential", new NetworkCredential("arbitrary-user", "arbitrary-password", "arbitrary-domain"));
            var sut = new PowerShellScriptPlugin();

            // Act
            var result = sut.Initialise(parameters, logger, true);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(sut.IsActive);
        }
    }
}
