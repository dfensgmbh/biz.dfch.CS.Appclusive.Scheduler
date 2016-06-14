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

using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Public.Logging;
using biz.dfch.CS.Appclusive.Public.Plugins;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;
using biz.dfch.CS.Utilities.Testing;
using biz.dfch.CS.Appclusive.Public.Configuration;

namespace biz.dfch.CS.Appclusive.Scheduler.Core.Tests
{
    [TestClass]
    public class ScheduledJobsWorkerConfigurationLoaderTest
    {
        [TestCategory("SkipOnTeamCity")]
        [TestMethod]
        public void ScheduledTaskWorkerConfigurationLoaderInitialiseWithConfigSectionSucceeds()
        {
            // Arrange
            var username = "arbitrary-user";
            var password = "arbitrary-password";
            var domain = "arbitrary-domain";
            var appclusiveCredentialSection = new AppclusiveCredentialSection();
            appclusiveCredentialSection.Username = username;
            appclusiveCredentialSection.Password = password;
            appclusiveCredentialSection.Domain = domain;
            
            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.GetSection(Arg.Is<string>(AppclusiveCredentialSection.SECTION_NAME)))
                .Returns(appclusiveCredentialSection)
                .MustBeCalled();

            Mock.Arrange(() => ConfigurationManager.AppSettings["ExtensionsFolder"])
                .IgnoreInstance()
                .Returns("..\\..\\..\\biz.dfch.CS.Appclusive.Scheduler.Extensions\\bin\\Debug");
            Mock.Arrange(() => ConfigurationManager.AppSettings["PluginTypes"])
                .IgnoreInstance()
                .Returns("*");
            Mock.Arrange(() => ConfigurationManager.AppSettings["UpdateIntervalMinutes"])
                .IgnoreInstance()
                .Returns("0");
            Mock.Arrange(() => ConfigurationManager.AppSettings["ServerNotReachableRetries"])
                .IgnoreInstance()
                .Returns("0");

            var uri = "http://www.example.com/arbitrary-path/";
            var mgmtUriName = "arbitrary-management-uri-name";
            string[] args = { uri, mgmtUriName };
            var sut = new ScheduledJobsWorkerConfigurationLoader();
            
            // Act
            // sut.Initialise() is implicitly called by ScheduledJobsWorkerConfiguration
            var config = new ScheduledJobsWorkerConfiguration(sut, args);

            // Assert
            Mock.Assert(() => ConfigurationManager.GetSection(Arg.Is<string>(AppclusiveCredentialSection.SECTION_NAME)));
            
            Assert.AreEqual(uri, config.Uri.AbsoluteUri);
            Assert.AreEqual(mgmtUriName, config.ManagementUriName);
            Assert.AreEqual(username, config.Credential.UserName);
            Assert.AreEqual(password, config.Credential.Password);
            Assert.AreEqual(domain, config.Credential.Domain);
        }
        
        [TestCategory("SkipOnTeamCity")]
        [TestMethod]
        public void ScheduledTaskWorkerConfigurationLoaderWithoutConfigSectionInitialiseSucceeds()
        {
            // Arrange
            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.GetSection(Arg.Is<string>(AppclusiveCredentialSection.SECTION_NAME)))
                .Returns(default(object))
                .MustBeCalled();

            Mock.Arrange(() => ConfigurationManager.AppSettings["ExtensionsFolder"])
                .IgnoreInstance()
                .Returns("..\\..\\..\\biz.dfch.CS.Appclusive.Scheduler.Extensions\\bin\\Debug");
            Mock.Arrange(() => ConfigurationManager.AppSettings["PluginTypes"])
                .IgnoreInstance()
                .Returns("*");
            Mock.Arrange(() => ConfigurationManager.AppSettings["UpdateIntervalMinutes"])
                .IgnoreInstance()
                .Returns("0");
            Mock.Arrange(() => ConfigurationManager.AppSettings["ServerNotReachableRetries"])
                .IgnoreInstance()
                .Returns("0");

            var uri = "http://www.example.com/arbitrary-path/";
            var mgmtUriName = "arbitrary-management-uri-name";
            string[] args = { uri, mgmtUriName };
            var sut = new ScheduledJobsWorkerConfigurationLoader();
            
            // Act
            // sut.Initialise() is implicitly called by ScheduledJobsWorkerConfiguration
            var config = new ScheduledJobsWorkerConfiguration(sut, args);

            // Assert
            Mock.Assert(() => ConfigurationManager.GetSection(Arg.Is<string>(AppclusiveCredentialSection.SECTION_NAME)));

            Assert.AreEqual(uri, config.Uri.AbsoluteUri);
            Assert.AreEqual(mgmtUriName, config.ManagementUriName);
            Assert.AreEqual(System.Net.CredentialCache.DefaultNetworkCredentials, config.Credential);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InitialiseWithInvalidConfigurationThrowsContractException()
        {
            var configuration = Mock.Create<ScheduledJobsWorkerConfiguration>();
            var parameters = new DictionaryParameters();

            var sut = new ScheduledJobsWorkerConfigurationLoader();
            sut.Initialise(configuration, parameters);
        }

        [TestMethod]
        public void InitialiseWithValidConfigurationSucceeds()
        {
            // Arrange
            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.AppSettings["UpdateIntervalMinutes"])
                .Returns("5");
            Mock.Arrange(() => ConfigurationManager.AppSettings["ServerNotReachableRetries"])
                .Returns("5");
            Mock.Arrange(() => ConfigurationManager.AppSettings["ManagementUri"])
                .Returns("ManagementUri");
            Mock.Arrange(() => ConfigurationManager.AppSettings["Uri"])
                .Returns("http://www.example.com/arbitrary-path/");

            var plugin = Mock.Create<Lazy<IAppclusivePlugin, IAppclusivePluginData>>();
            var plugins = new List<Lazy<IAppclusivePlugin, IAppclusivePluginData>>();
            plugins.Add(plugin);
            var pluginLoader = Mock.Create<PluginLoader>(Constructor.Mocked);
            Mock.Arrange(() => pluginLoader.InitialiseAndLoad())
                .IgnoreInstance()
                .Returns(plugins);

            var configuration = Mock.Create<ScheduledJobsWorkerConfiguration>(Constructor.NotMocked);
            Mock.Arrange(() => configuration.ManagementUriType)
                .IgnoreInstance()
                .Returns("ManagementUriType");
            Mock.Arrange(() => configuration.Uri)
                .IgnoreInstance()
                .Returns(new Uri("http://www.example.com/arbitrary-path/"));
            Mock.Arrange(() => configuration.IsValid())
                .IgnoreInstance()
                .Returns(true);
            var parameters = new DictionaryParameters();

            var sut = new ScheduledJobsWorkerConfigurationLoader();

            // Act
            sut.Initialise(configuration, parameters);

            // Assert
            // N/A
        }
    }
}
