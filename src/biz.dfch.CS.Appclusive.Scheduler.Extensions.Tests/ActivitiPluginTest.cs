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
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Scheduler.Core;
using biz.dfch.CS.Utilities.Testing;
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
    public class ActivitiPluginTest
    {
        public static string Username = "arbitrary-user";
        public static string Password = "arbitrary-password";
        public static string ServerBaseUriString = "http://www.example.com/arbitrary-folder/";

        public static ActivitiPluginConfiguration ValidActivitiPluginConfiguration { get; set; }

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            ValidActivitiPluginConfiguration = new ActivitiPluginConfiguration()
            {
                ServerBaseUri = new Uri(ServerBaseUriString)
                ,
                Credential = new System.Net.NetworkCredential(Username, Password)
            };
        }

        [TestMethod]
        public void InitialiseSucceeds()
        {
            // Arrange
            var logger = new Logger();
            var parameters = new DictionaryParameters().Convert(ValidActivitiPluginConfiguration);
            var sut = new ActivitiPlugin();

            // Act
            var result = sut.Initialise(parameters, logger, true);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(sut.IsInitialised);
            Assert.IsTrue(sut.IsActive);
        }
        
        [TestMethod]
        [ExpectContractFailure]
        public void InitialiseWithInvalidConfigurationThrowsContractException()
        {
            // Arrange
            var logger = new Logger();
            var sut = new ActivitiPlugin();

            // Act
            sut.Initialise(new DictionaryParameters(), logger, true);

            // Assert
            // N/A
        }
        
        [TestMethod]
        [ExpectContractFailure]
        public void InitialiseWithNullConfigurationThrowsContractException()
        {
            // Arrange
            var logger = new Logger();
            var sut = new ActivitiPlugin();

            // Act
            sut.Initialise(default(DictionaryParameters), logger, true);

            // Assert
            // N/A
        }
        
        [TestMethod]
        public void LogSucceeds()
        {
            // Arrange
            var message = "arbitrary-message";
            var logger = new Logger();
            var sut = new ActivitiPlugin();
            var parameters = new DictionaryParameters().Convert(ValidActivitiPluginConfiguration);
            sut.Initialise(parameters, logger, true);

            // Act
            sut.Logger.WriteLine(message);

            // Assert
            // N/A
        }
    }
}
