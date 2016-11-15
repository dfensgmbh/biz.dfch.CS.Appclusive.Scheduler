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
using System.Data.Services.Client;
using biz.dfch.CS.Appclusive.Api.Core;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Scheduler.Core;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using biz.dfch.CS.Testing.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;
using System.Net;
using System.Configuration;

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
        public void ConvertFromDictionaryParametersToSchedulerPluginConfigurationBaseSucceeds()
        {
            var uri = new Uri("http://www.example.com/");
            var credentials = CredentialCache.DefaultNetworkCredentials;
            var appclusiveEndpoints = new AppclusiveEndpoints(uri, credentials);

            var sut = new DictionaryParameters();
            sut.Add(typeof(AppclusiveEndpoints).ToString(), appclusiveEndpoints);

            // Act
            var result = SchedulerPluginConfigurationBase.Convert<SchedulerPluginConfigurationBase>(sut);

            // Assert
            Assert.IsTrue(result.IsValid());
            Assert.IsNotNull(result.Endpoints);
        }

        [TestMethod]
        public void ConvertToDictionaryParametersFromSchedulerPluginConfigurationBaseSucceeds()
        {
            var uri = new Uri("http://www.example.com/");
            var credentials = CredentialCache.DefaultNetworkCredentials;
            var appclusiveEndpoints = new AppclusiveEndpoints(uri, credentials);
            var keyName = appclusiveEndpoints.GetType().FullName;

            var sut = new SchedulerPluginConfigurationBase()
            {
                Endpoints = appclusiveEndpoints
            };
            Assert.IsTrue(sut.IsValid());

            // Act
            var result = sut.Convert();

            // Assert
            Assert.IsTrue(result.ContainsKey(keyName));
            Assert.AreEqual(appclusiveEndpoints, result.Get(keyName));
        }

        [TestMethod]
        public void InitialiseSucceeds()
        {
            // Arrange
            var client = Mock.Create<ActivitiClient>();
            Mock.Arrange(() => client.Login(Arg.IsAny<NetworkCredential>()))
                .IgnoreInstance()
                .Returns(true)
                .MustBeCalled();

            var serverBaseUri = new Uri("http://www.example.com:9080/activiti-rest/service/");
            var managementCredentialId = 5;
            var managementUriName = SchedulerAppSettings.Keys.EXTERNAL_WORKFLOW_MANAGEMENT_URI_NAME;
            var managementUri = new ManagementUri()
            {
                Id = 42
                ,
                Name = managementUriName
                ,
                ManagementCredentialId = managementCredentialId
                ,
                Value = string.Format("{{\"ServerUri\":\"{0}\"}}", serverBaseUri)
            };
            var managementUris = new List<ManagementUri>()
            {
                managementUri
            };

            var dataServiceQueryManagementUris = Mock.Create<DataServiceQuery<ManagementUri>>();
            Mock.Arrange(() => dataServiceQueryManagementUris.Where(e => e.Name == managementUriName))
                .IgnoreInstance()
                .Returns(managementUris.AsQueryable())
                .MustBeCalled();

            var encryptedPassword = "encrypted-arbitrary-password";
            var managementCredential = new ManagementCredential()
            {
                Id = managementCredentialId
                ,
                Name = managementUriName
                ,
                Username = Username
                ,
                Password = Password
                ,
                EncryptedPassword = encryptedPassword
            };
            var managementCredentials = new List<ManagementCredential>()
            {
                managementCredential
            };

            var dataServiceQueryManagementCredentials = Mock.Create<DataServiceQuery<ManagementCredential>>();
            Mock.Arrange(() => dataServiceQueryManagementCredentials.Where(e => e.Id == managementCredentialId))
                .IgnoreInstance()
                .Returns(managementCredentials.AsQueryable())
                .MustBeCalled();

            var endpoints = Mock.Create<AppclusiveEndpoints>(Constructor.Mocked);
            var apiCore = Mock.Create<Api.Core.Core>(Constructor.Mocked);
            endpoints.Core = apiCore;
            var parameters = new DictionaryParameters();
            parameters.Add(typeof(AppclusiveEndpoints).ToString(), endpoints);
            
            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.AppSettings[Arg.IsAny<string>()])
                .Returns(managementUriName)
                .MustBeCalled();

            var logger = new Logger();

            var sut = new ActivitiPlugin();

            // Act
            var result = sut.Initialise(parameters, logger, true);

            // Assert
            Mock.Assert(() => ConfigurationManager.AppSettings[Arg.IsAny<string>()]);
            Mock.Assert(client);

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
            var client = Mock.Create<ActivitiClient>();
            Mock.Arrange(() => client.Login(Arg.IsAny<NetworkCredential>()))
                .IgnoreInstance()
                .Returns(true)
                .MustBeCalled();

            var logger = new Logger();
            var sut = new ActivitiPlugin();

            // Act
            sut.Initialise(default(DictionaryParameters), logger, true);

            // Assert
            Mock.Assert(client);
        }
        
        [TestMethod]
        public void LogSucceeds()
        {
            // Arrange
            var client = Mock.Create<ActivitiClient>();
            Mock.Arrange(() => client.Login(Arg.IsAny<NetworkCredential>()))
                .IgnoreInstance()
                .Returns(true)
                .MustBeCalled();

            var managementUriName = SchedulerAppSettings.Keys.EXTERNAL_WORKFLOW_MANAGEMENT_URI_NAME;
            var serverBaseUri = new Uri("http://www.example.com:9080/activiti-rest/service/");
            var managementCredentialId = 5;
            var managementUri = new ManagementUri()
            {
                Id = 42
                ,
                Name = managementUriName
                ,
                ManagementCredentialId = managementCredentialId
                ,
                Value = string.Format("{{\"ServerUri\":\"{0}\"}}", serverBaseUri)
            };

            var encryptedPassword = "encrypted-arbitrary-password";
            var managementCredential = new ManagementCredential()
            {
                Id = managementCredentialId
                ,
                Name = managementUriName
                ,
                Username = Username
                ,
                Password = Password
                ,
                EncryptedPassword = encryptedPassword
            };

            var activitiPluginConfigurationManager = Mock.Create<ActivitiPluginConfigurationManager>();
            Mock.Arrange(() => activitiPluginConfigurationManager.GetManagementUriName())
                .IgnoreInstance()
                .Returns(managementUriName)
                .MustBeCalled();
            Mock.Arrange(() => activitiPluginConfigurationManager.GetManagementUri(Arg.IsAny<DataServiceQuery<ManagementUri>>(), Arg.IsAny<string>()))
                .IgnoreInstance()
                .Returns(managementUri)
                .MustBeCalled();
            Mock.Arrange(() => activitiPluginConfigurationManager.GetManagementCredential(Arg.IsAny<DataServiceQuery<ManagementCredential>>(), Arg.IsAny<long>()))
                .IgnoreInstance()
                .Returns(managementCredential)
                .MustBeCalled();
            
            var endpoints = Mock.Create<AppclusiveEndpoints>(Constructor.Mocked);
            var apiCore = Mock.Create<Api.Core.Core>(Constructor.Mocked);
            endpoints.Core = apiCore;
            var parameters = new DictionaryParameters();
            parameters.Add(typeof(AppclusiveEndpoints).ToString(), endpoints);

            var message = "arbitrary-message";
            var logger = new Logger();
            var sut = new ActivitiPlugin();

            sut.Initialise(parameters, logger, true);

            // Act
            sut.Logger.WriteLine(message);

            // Assert
            Mock.Assert(client);
            Mock.Assert(activitiPluginConfigurationManager);
        }

        [TestMethod]
        public void UpdateConfigurationSucceeds()
        {
            // Arrange
            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.AppSettings[Arg.IsAny<string>()])
                .Returns(SchedulerAppSettings.Keys.EXTERNAL_WORKFLOW_MANAGEMENT_URI_NAME)
                .MustBeCalled();

            var client = Mock.Create<ActivitiClient>();
            Mock.Arrange(() => client.Login(Arg.IsAny<NetworkCredential>()))
                .IgnoreInstance()
                .Returns(true)
                .MustBeCalled();

            var serverBaseUri = new Uri("http://www.example.com:9080/activiti-rest/service/");
            var managementCredentialId = 5;
            var managementUriName = SchedulerAppSettings.Keys.EXTERNAL_WORKFLOW_MANAGEMENT_URI_NAME;
            var managementUri = new ManagementUri()
            {
                Id = 42
                ,
                Name = managementUriName
                ,
                ManagementCredentialId = managementCredentialId
                ,
                Value = string.Format("{{\"ServerUri\":\"{0}\"}}", serverBaseUri)
            };
            var managementUris = new List<ManagementUri>()
            {
                managementUri
            };

            var dataServiceQueryManagementUris = Mock.Create<DataServiceQuery<ManagementUri>>();
            Mock.Arrange(() => dataServiceQueryManagementUris.Where(e => e.Name == managementUriName))
                .IgnoreInstance()
                .Returns(managementUris.AsQueryable())
                .MustBeCalled();

            var encryptedPassword = "encrypted-arbitrary-password";
            var managementCredential = new ManagementCredential()
            {
                Id = managementCredentialId
                ,
                Name = managementUriName
                ,
                Username = Username
                ,
                Password = Password
                ,
                EncryptedPassword = encryptedPassword
            };
            var managementCredentials = new List<ManagementCredential>()
            {
                managementCredential
            };

            var dataServiceQueryManagementCredentials = Mock.Create<DataServiceQuery<ManagementCredential>>();
            Mock.Arrange(() => dataServiceQueryManagementCredentials.Where(e => e.Id == managementCredentialId))
                .IgnoreInstance()
                .Returns(managementCredentials.AsQueryable())
                .MustBeCalled();

            var endpoints = Mock.Create<AppclusiveEndpoints>(Constructor.Mocked);
            var apiCore = Mock.Create<Api.Core.Core>(Constructor.Mocked);
            endpoints.Core = apiCore;
            var parameters = new DictionaryParameters();
            parameters.Add(typeof(AppclusiveEndpoints).ToString(), endpoints);

            var sut = new ActivitiPlugin();
            Mock.NonPublic.Arrange<string>(sut, "managementUriName")
                .IgnoreInstance()
                .Returns(managementUriName);

            // Act
            sut.Configuration = parameters;

            // Assert
            Mock.Arrange(() => ConfigurationManager.AppSettings[Arg.IsAny<string>()]);
            Mock.Assert(client);
            Mock.Assert(endpoints);
            Mock.Assert(dataServiceQueryManagementUris);
            Mock.Assert(dataServiceQueryManagementCredentials);

            Assert.IsTrue(sut.Configuration.ContainsKey("ServerBaseUri"));
            var actualServerBaseUri = sut.Configuration["ServerBaseUri"] as Uri;
            Assert.IsNotNull(actualServerBaseUri);
            Assert.AreEqual(serverBaseUri, actualServerBaseUri);

            Assert.IsTrue(sut.Configuration.ContainsKey("Credential"));
            var actualCredential = sut.Configuration["Credential"] as NetworkCredential;
            Assert.IsNotNull(actualCredential);
            Assert.AreEqual(Username, actualCredential.UserName);
            Assert.AreEqual(Password, actualCredential.Password);
        }
    }
}
