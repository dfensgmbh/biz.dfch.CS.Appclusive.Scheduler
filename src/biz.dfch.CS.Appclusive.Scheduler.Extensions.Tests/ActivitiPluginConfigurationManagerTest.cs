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
using System.Configuration;
using System.Data.Services.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;
using biz.dfch.CS.Appclusive.Api.Core;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions.Tests
{
    [TestClass]
    public class ActivitiPluginConfigurationManagerTest
    {
        [TestMethod]
        public void GetManagementUriSucceeds()
        {
            // Arrange
            var managementUriName = SchedulerAppSettings.Keys.EXTERNAL_WORKFLOW_MANAGEMENT_URI_NAME;
            var managementUri = new ManagementUri()
            {
                Id = 42
                ,
                Name = managementUriName
                ,
                ManagementCredentialId = 5
            };
            var managementUris = new List<ManagementUri>()
            {
                managementUri
            };
            
            var dataServiceQuery = Mock.Create<DataServiceQuery<ManagementUri>>();
            Mock.Arrange(() => dataServiceQuery.Where(e => e.Name == managementUriName))
                .IgnoreInstance()
                .Returns(managementUris.AsQueryable())
                .MustBeCalled();

            var sut = new ActivitiPluginConfigurationManager();

            // Act
            var result = sut.GetManagementUri(dataServiceQuery, managementUriName);

            // Assert
            Assert.AreEqual(managementUriName, result.Name);
        }

        [TestMethod]
        public void GetManagementCredentialSucceeds()
        {
            // Arrange
            var managementUriName = SchedulerAppSettings.Keys.EXTERNAL_WORKFLOW_MANAGEMENT_URI_NAME;
            var managementCredentialId = 42;
            var username = "arbitrary-user";
            var password = "arbitrary-password";
            var managementCredential = new ManagementCredential()
            {
                Id = managementCredentialId
                ,
                Name = managementUriName
                ,
                Username = username
                ,
                Password = password
            };
            var managementCredentials = new List<ManagementCredential>()
            {
                managementCredential
            };
            
            var dataServiceQuery = Mock.Create<DataServiceQuery<ManagementCredential>>();
            Mock.Arrange(() => dataServiceQuery.Where(e => e.Id == managementCredentialId))
                .IgnoreInstance()
                .Returns(managementCredentials.AsQueryable())
                .MustBeCalled();

            var sut = new ActivitiPluginConfigurationManager();

            // Act
            var result = sut.GetManagementCredential(dataServiceQuery, managementCredentialId);

            // Assert
            Assert.AreEqual(managementCredentialId, result.Id);
            Assert.AreEqual(username, result.Username);
            Assert.AreEqual(password, result.Password);
        }
        
        [TestMethod]
        public void GetCredentialSucceeds()
        {
            // Arrange
            var managementUriName = SchedulerAppSettings.Keys.EXTERNAL_WORKFLOW_MANAGEMENT_URI_NAME;
            var managementCredentialId = 42;
            var username = "arbitrary-user";
            var password = "arbitrary-password";
            var managementCredential = new ManagementCredential()
            {
                Id = managementCredentialId
                ,
                Name = managementUriName
                ,
                Username = username
                ,
                Password = password
            };

            var sut = new ActivitiPluginConfigurationManager();

            // Act
            var result = sut.GetCredential(managementCredential);

            // Assert
            Assert.AreEqual(username, result.UserName);
            Assert.AreEqual(password, result.Password);
        }

        [TestMethod]
        public void GetManagementUriName()
        {
            // Arrange
            var managementUriName = SchedulerAppSettings.Keys.EXTERNAL_WORKFLOW_MANAGEMENT_URI_NAME;

            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.AppSettings[Arg.IsAny<string>()])
                .Returns(managementUriName)
                .MustBeCalled();

            var sut = new ActivitiPluginConfigurationManager();

            // Act
            var result = sut.GetManagementUriName();

            // Assert
            Mock.Assert(() => ConfigurationManager.AppSettings[Arg.IsAny<string>()]);
            Assert.AreEqual(managementUriName, result);
        }
    }
}
