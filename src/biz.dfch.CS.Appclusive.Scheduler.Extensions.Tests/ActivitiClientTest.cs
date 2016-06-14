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
using System.Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;
using biz.dfch.CS.Activiti.Client;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions.Tests
{
    [TestClass]
    public class ActivitiClientTest
    {
        private readonly static ActivitiPluginTestEnvironmentTemplate environment = new ActivitiPluginTestEnvironment();

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            // N/A
        }

        [TestMethod]
        public void LoginFails()
        {
            // Arrange
            environment.Credential = new NetworkCredential("invalid-user", "invalid-password");

            var processEngine = Mock.Create<ProcessEngine>();
            Mock.Arrange(() => processEngine.Login(Arg.Is<NetworkCredential>(environment.Credential)))
                .IgnoreInstance()
                .DoNothing()
                .MustBeCalled();
            Mock.Arrange(() => processEngine.IsLoggedIn())
                .IgnoreInstance()
                .Returns(false)
                .MustBeCalled();

            var sut = new ActivitiClient(environment.ServerBaseUri, environment.ApplicationName);

            // Act
            var result = sut.Login(environment.Credential);

            // Assert
            Mock.Assert(processEngine);
            Assert.IsFalse(result);
        }

        [TestCategory("SkipOnTeamCity")]
        [TestMethod]
        public void LoginSucceeds()
        {
            // Arrange
            var sut = new ActivitiClient(environment.ServerBaseUri, environment.ApplicationName);

            // Act
            var result = sut.Login(environment.Credential);

            // Assert
            Assert.IsTrue(result);
        }
    }
}