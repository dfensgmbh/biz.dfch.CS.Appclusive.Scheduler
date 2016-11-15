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

using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;
using biz.dfch.CS.Testing.Attributes;
using System.Configuration;

namespace biz.dfch.CS.Appclusive.Scheduler.Public.Tests
{
    [TestClass]
    public class ConfigSectionManagerTest
    {
        private readonly string exePath = "..\\..\\..\\biz.dfch.CS.Appclusive.Scheduler.Core\\bin\\Debug\\biz.dfch.CS.Appclusive.Scheduler.Core.exe";
        private readonly string sectionName = AppclusiveCredentialSection.SECTION_NAME;

        [TestMethod]
        public void GettingConfigSectionFromExecutableSucceeds()
        {
            // Arrange
            Mock.SetupStatic(typeof(File));
            Mock.Arrange(() => File.Exists(Arg.Is<string>(exePath)))
                .Returns(true)
                .MustBeCalled();

            var configurationSection = new AppclusiveCredentialSection();
            var configuration = Mock.Create<Configuration>();
            Mock.Arrange(() => configuration.GetSection(Arg.IsAny<string>()))
                .IgnoreInstance()
                .Returns(configurationSection)
                .MustBeCalled();

            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.OpenExeConfiguration(Arg.Is<string>(exePath)))
                .Returns(configuration)
                .MustBeCalled();

            var sut = new ConfigSectionManager(exePath);

            // Act
            var result = sut.Get(sectionName);

            // Assert
            Mock.Assert(() => ConfigurationManager.OpenExeConfiguration(Arg.Is<string>(exePath)));
            Mock.Assert(() => File.Exists(Arg.Is<string>(exePath)));
            Mock.Assert(configuration);

            Assert.IsInstanceOfType(result, typeof(AppclusiveCredentialSection));
            var appclusiveCredentialSection = result as AppclusiveCredentialSection;
            Assert.IsNotNull(appclusiveCredentialSection);
        }

        [TestCategory("SkipOnTeamCity")]
        [TestMethod]
        public void EncryptingAndDecryptingConfigSectionSucceeds()
        {
            // Arrange
            var sut = new ConfigSectionManager(exePath);

            // Act
            sut.Encrypt(sectionName);

            // Assert
            sut.Decrypt(sectionName);
        }

        [TestCategory("SkipOnTeamCity")]
        [TestMethod]
        public void EncryptingAndGettingEncryptedConfigSectionSucceeds()
        {
            // Arrange
            var sut = new ConfigSectionManager(exePath);

            // Act
            sut.Encrypt(sectionName);
            var result = sut.Get(sectionName);
            sut.Decrypt(sectionName);

            // Assert
            Assert.IsInstanceOfType(result, typeof(AppclusiveCredentialSection));
            var appclusiveCredentialSection = result as AppclusiveCredentialSection;
            Assert.IsNotNull(appclusiveCredentialSection);
            Assert.IsFalse(string.IsNullOrWhiteSpace(appclusiveCredentialSection.Domain));
        }

    }
}
