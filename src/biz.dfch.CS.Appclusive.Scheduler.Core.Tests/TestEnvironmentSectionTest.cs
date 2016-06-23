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
using System.Configuration;

namespace biz.dfch.CS.Appclusive.Scheduler.Core.Tests
{
    [TestClass]
    public class TestEnvironmentSectionTest
    {
        [TestCategory("SkipOnTeamCity")]
        [TestMethod]
        public void GettingConfigSectionSucceeds()
        {
            // Arrange
            
            // Act
            var result = ConfigurationManager.GetSection(TestEnvironmentSection.SECTION_NAME) as TestEnvironmentSection;
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("arbitrary-user", result.Username);
            Assert.AreEqual("arbitrary-password", result.Password);
            Assert.AreEqual("www.example.com", result.ServerBaseUri.Host);
        }
    }
}
