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
using Telerik.JustMock;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using System.Configuration;
using biz.dfch.CS.Testing.Attributes;
using System.IO;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions.Tests
{
    [TestClass]
    public class PowerShellScriptPluginConfigurationManagerTest
    {
        [TestMethod]
        public void GetScriptBaseSucceeds()
        {
            // Arrange
            var scriptBaseFromAppSettings = "%ProgramFiles%";
            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.AppSettings.Get(Arg.IsAny<string>()))
                .Returns(scriptBaseFromAppSettings)
                .MustBeCalled();

            var appclusiveEndpoints = Mock.Create<AppclusiveEndpoints>();
            var sut = new PowerShellScriptPluginConfigurationManager(appclusiveEndpoints);

            // Act
            var result = sut.GetScriptBase();

            // Assert
            Mock.Assert(() => ConfigurationManager.AppSettings.Get(Arg.IsAny<string>()));

            Assert.AreEqual(Environment.ExpandEnvironmentVariables(scriptBaseFromAppSettings), result);
        }

        [TestMethod]
        public void GetScriptBaseWithInexistentAppSettingsReturnsDefaultDirectory()
        {
            // Arrange
            var scriptBaseFromAppSettings = default(string);
            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.AppSettings.Get(Arg.IsAny<string>()))
                .Returns(scriptBaseFromAppSettings)
                .MustBeCalled();

            var appclusiveEndpoints = Mock.Create<AppclusiveEndpoints>();
            var sut = new PowerShellScriptPluginConfigurationManager(appclusiveEndpoints);

            // Act
            var result = sut.GetScriptBase();

            // Assert
            Mock.Assert(() => ConfigurationManager.AppSettings.Get(Arg.IsAny<string>()));

            Assert.IsTrue(Directory.Exists(result));
        }

        [TestMethod]
        [ExpectContractFailure]
        public void GetScriptBaseWithInexistentDirectoryThrowsContractException()
        {
            // Arrange
            var scriptBaseFromAppSettings = "C:\\inexistent-folder";
            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.AppSettings.Get(Arg.IsAny<string>()))
                .Returns(scriptBaseFromAppSettings)
                .MustBeCalled();

            var appclusiveEndpoints = Mock.Create<AppclusiveEndpoints>();
            var sut = new PowerShellScriptPluginConfigurationManager(appclusiveEndpoints);

            // Act
            var result = sut.GetScriptBase();

            // Assert
            // N/A
        }

    }
}
