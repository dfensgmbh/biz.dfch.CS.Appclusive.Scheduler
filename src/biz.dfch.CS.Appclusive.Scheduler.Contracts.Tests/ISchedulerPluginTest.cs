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
using System.Collections.Specialized;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Utilities.Testing;

namespace biz.dfch.CS.Appclusive.Scheduler.Contracts.Tests
{
    [TestClass]
    public class ISchedulerPluginTest
    {
        class SchedulerPlugin : ISchedulerPlugin
        {
            public StringDictionary Configuration { get; set; }

            public void Log(string message)
            {
                throw new NotImplementedException();
            }

            public bool UpdateConfiguration(StringDictionary configuration)
            {
                throw new NotImplementedException();
            }

            public bool Invoke(StringDictionary data, ref JobResult jobResult)
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void ISchedulerPluginInterfaceIsIntact()
        {
            var sut = new SchedulerPlugin();

            Assert.IsNotNull(sut);
        }
        
        [TestMethod]
        [ExpectContractFailure]
        public void LogEmptyThrowsContractException()
        {
            // Arrange
            var sut = new ISchedulerPluginTest.SchedulerPlugin();
            var message = string.Empty;

            // Act
            sut.Log(message);

            // Assert
            // N/A
        }
    }
}
