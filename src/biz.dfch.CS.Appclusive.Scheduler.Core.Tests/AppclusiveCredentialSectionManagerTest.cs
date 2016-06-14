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

using biz.dfch.CS.Appclusive.Scheduler.Public;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;

namespace biz.dfch.CS.Appclusive.Scheduler.Core.Tests
{
    [TestClass]
    public class AppclusiveCredentialSectionManagerTest
    {
        [TestMethod]
        public void GettingConfigSectionSucceeds()
        {
            // Arrange
            var configSection = new AppclusiveCredentialSection();
            var configSectionManager = Mock.Create<ConfigSectionManager>(Constructor.Mocked);
            Mock.Arrange(() => configSectionManager.Get(Arg.Is<string>(AppclusiveCredentialSection.SECTION_NAME)))
                .IgnoreInstance()
                .Returns(configSection)
                .MustBeCalled();

            var sut = new AppclusiveCredentialSectionManager();
            
            // Act
            var result = sut.GetSection();
            
            // Assert
            Mock.Assert(configSectionManager);
            Assert.IsNotNull(result);
        }
    }
}
