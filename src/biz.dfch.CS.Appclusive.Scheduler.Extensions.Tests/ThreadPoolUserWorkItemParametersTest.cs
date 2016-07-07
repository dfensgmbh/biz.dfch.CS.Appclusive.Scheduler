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

using biz.dfch.CS.Appclusive.Scheduler.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions.Tests
{
    [TestClass]
    public class ThreadPoolUserWorkItemParametersTest
    {
        [TestMethod]
        public void ThreadPoolUserWorkItemParametersWithValidConfigurationSucceeds()
        {
            // Arrange
            var sut = new ThreadPoolUserWorkItemParameters()
            {
                ActivityId = Guid.NewGuid()
                ,
                Logger = new Logger()
                ,
                ScriptPathAndName = "C:\\arbitrary-folder\\arbitrary-script.ps1"
                ,
                ScriptParameters = new Dictionary<string,object>()
            };

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ThreadPoolUserWorkItemParametersWithInvalidConfigurationFails()
        {
            // Arrange
            var sut = new ThreadPoolUserWorkItemParameters();

            // Act
            var result = sut.IsValid();

            // Assert
            Assert.IsFalse(result);
        }
    }
}
