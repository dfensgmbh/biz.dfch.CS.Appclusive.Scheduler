﻿/**
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
using biz.dfch.CS.Testing.Attributes;
using System.Collections.Generic;
using Telerik.JustMock;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Public.Plugins;

namespace biz.dfch.CS.Appclusive.Scheduler.Public.Tests
{
    [TestClass]
    public class ISchedulerPluginTest
    {
        class SchedulerPluginImpl : SchedulerPluginBase
        {
            public override bool Invoke(DictionaryParameters parameters, IInvocationResult jobResult)
            {
                if(!IsActive)
                {
                    return false;
                }

                jobResult = null;
                
                return true;
            }
        }
        
        [TestMethod]
        public void LogSucceeds()
        {
            // Arrange
            var message = "arbitrary-message";
            var sut = new SchedulerPluginImpl();
            var logger = Mock.Create<IAppclusivePluginLogger>();
            sut.Initialise(new DictionaryParameters(), logger, true);

            // Act
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
        public void InvokeDataNullThrowsContractException()
        {
            // Arrange
            var sut = new SchedulerPluginImpl();
            var parameters = default(DictionaryParameters);
            var jobResult = new JobResult();

            // Act
            sut.Invoke(parameters, jobResult);

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
            var jobResult = default(IInvocationResult);

            // Act
            sut.Invoke(parameters, jobResult);

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
            sut.Invoke(parameters, jobResult);

            // Assert
            Assert.Fail("CodeContracts are not enabled.");
        }
    }
}
