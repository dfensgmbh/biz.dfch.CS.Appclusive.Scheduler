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
using System.Diagnostics.Contracts;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Appclusive.Scheduler.Core;
using biz.dfch.CS.Utilities.Testing;
using System.Collections.Generic;
using biz.dfch.CS.Appclusive.Scheduler.Public;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions.Tests
{
    [TestClass]
    public class ISchedulerPluginTest
    {
        class SchedulerPluginImpl : ISchedulerPlugin
        {
            public DictionaryParameters Configuration { get; set; }

            public ILogger Logger { get; set; }

            public bool Invoke(DictionaryParameters parameters, ref JobResult jobResult)
            {
                jobResult = null;
                
                return true;
            }
        }

        [TestMethod]
        public void LogSucceeds()
        {
            // Arrange
            var message = "arbitrary-message";
            var logger = new Logger();

            // Act
            var sut = new DefaultPlugin();
            sut.Logger = logger;
            sut.Logger.WriteLine(message);

            // Assert
            // N/A
        }
        
        [TestMethod]
        [ExpectContractFailure]
        public void ConfigurationSetNullThrowsContractException()
        {
            // Arrange
            var sut = new SchedulerPluginImpl();
            var configuration = default(DictionaryParameters);

            // Act
            sut.Configuration = configuration;

            // Assert
            Assert.Fail("CodeContracts are not enabled.");
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ConfigurationGetNullThrowsContractException()
        {
            // Arrange
            var sut = new SchedulerPluginImpl();

            // Act
            var result = sut.Configuration;

            // Assert
            Assert.Fail("CodeContracts are not enabled.");
        }

        [TestMethod]
        [ExpectContractFailure]
        public void LogEmptyThrowsContractException()
        {
            // Arrange
            var sut = new SchedulerPluginImpl();
            var message = string.Empty;
            var logger = new Logger();
            sut.Logger = logger;

            // Act
            sut.Logger.WriteLine(message);

            // Assert
            Assert.Fail("CodeContracts are not enabled.");
        }

        [TestMethod]
        [ExpectContractFailure]
        public void SetConfigurationNullThrowsContractException()
        {
            // Arrange
            var sut = new SchedulerPluginImpl();
            var configuration = default(DictionaryParameters);

            // Act
            sut.Configuration = configuration;

            // Assert
            Assert.Fail("CodeContracts are not enabled.");
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeDataNullThrowsContractException()
        {
            // Arrange
            var sut = new SchedulerPluginImpl();
            var parameters = default(DictionaryParameters);
            var jobResult = new JobResult();

            // Act
            sut.Invoke(parameters, ref jobResult);

            // Assert
            Assert.Fail("CodeContracts are not enabled.");
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeJobResultNullThrowsContractException()
        {
            // Arrange
            var sut = new SchedulerPluginImpl();
            var parameters = new DictionaryParameters();
            var jobResult = default(JobResult);

            // Act
            sut.Invoke(parameters, ref jobResult);

            // Assert
            Assert.Fail("CodeContracts are not enabled.");
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeJobResultNullOnReturnThrowsContractException()
        {
            // Arrange
            var sut = new SchedulerPluginImpl();
            var parameters = new DictionaryParameters();
            var jobResult = new JobResult();

            // Act
            sut.Invoke(parameters, ref jobResult);

            // Assert
            Assert.Fail("CodeContracts are not enabled.");
        }
    }
}
