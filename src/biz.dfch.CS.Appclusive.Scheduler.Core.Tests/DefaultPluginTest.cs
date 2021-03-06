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
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Testing.Attributes;
using System.Collections.Specialized;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using System.Collections.Generic;

namespace biz.dfch.CS.Appclusive.Scheduler.Core.Tests
{
    [TestClass]
    public class DefaultPluginTest
    {
        [TestMethod]
        public void LogSucceeds()
        {
            // Arrange
            var message = "arbitrary-message";
            var logger = new Logger();
            var sut = new DefaultPlugin();
            sut.Initialise(new DictionaryParameters(), logger, true);

            // Act
            sut.Logger.WriteLine(message);

            // Assert
            // N/A
        }

        [TestMethod]
        public void InvokeWithIsActiveFalseReturnsFalse()
        {
            // Arrange
            var jobResult = new JobResult();
            var logger = new Logger();
            var sut = new DefaultPlugin();
            sut.Initialise(new DictionaryParameters(), logger, false);

            // Act
            var result = sut.Invoke(new DictionaryParameters(), jobResult);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeIsInitialisedFalseThrowsContractException()
        {
            // Arrange
            var jobResult = new JobResult();
            var sut = new DefaultPlugin();
            //sut.Initialise(new DictionaryParameters(), new Logger(), true);

            // Act
            sut.Invoke(new DictionaryParameters(), jobResult);

            // Assert
            // N/A
        }

        [TestMethod]
        public void GetAndSetConfigurationSucceeds()
        {
            // Arrange
            var configuration = new DictionaryParameters();
            var key1 = "arbitrary-key1";
            var value1 = "arbitrary-value";
            configuration.Add(key1, value1);
            var key2 = "arbitrary-key2";
            var value2 = 42;
            configuration.Add(key2, value2);
            var key3 = "arbitrary-key3";
            var value3 = new object();
            configuration.Add(key3, value3);

            // Act
            var sut = new DefaultPlugin();
            sut.Configuration = configuration;

            // Assert
            Assert.AreEqual(configuration, sut.Configuration);
        }

        [TestMethod]
        public void InvokeConfigurationSucceeds()
        {
            // Arrange
            var parameters = new DictionaryParameters();
            var key1 = "arbitrary-key1";
            var value1 = "arbitrary-value";
            parameters.Add(key1, value1);
            var key2 = "arbitrary-key2";
            var value2 = 42;
            parameters.Add(key2, value2);
            var key3 = "arbitrary-key3";
            var value3 = new object();
            parameters.Add(key3, value3);

            var jobResult = new JobResult();

            var schedulerPluginBase = Mock.Create<SchedulerPluginBase>();
            Mock.Arrange(() => schedulerPluginBase.Invoke(Arg.IsAny<DictionaryParameters>(), Arg.IsAny<IInvocationResult>()))
                .IgnoreInstance()
                .CallOriginal()
                .MustBeCalled();

            // Act
            var sut = new DefaultPlugin();
            sut.Initialise(new DictionaryParameters(), new Logger(), true);
            var result = sut.Invoke(parameters, jobResult);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(jobResult);
            Assert.IsTrue(jobResult.Description.Contains(key1));
            Assert.IsTrue(jobResult.Description.Contains(value1.ToString()));
            Assert.IsTrue(jobResult.Description.Contains(key2));
            Assert.IsTrue(jobResult.Description.Contains(value2.ToString()));
            Assert.IsTrue(jobResult.Description.Contains(key3));
            Assert.IsTrue(jobResult.Description.Contains(value3.ToString()));
        
            Mock.Assert(schedulerPluginBase);
        }
    }
}
