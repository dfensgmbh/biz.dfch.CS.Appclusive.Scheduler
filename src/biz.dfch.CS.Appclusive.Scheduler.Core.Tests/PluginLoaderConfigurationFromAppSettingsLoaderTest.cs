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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;
using System.IO;
using biz.dfch.CS.Utilities.Testing;
using biz.dfch.CS.Appclusive.Scheduler.Public;

namespace biz.dfch.CS.Appclusive.Scheduler.Core.Tests
{
    [TestClass]
    public class PluginLoaderConfigurationFromAppSettingsLoaderTest
    {
        [TestMethod]
        public void InitialiseOrdersPluginNamesAndSucceeds()
        {
            // Arrange
            var configuration = new PluginLoaderConfiguration();
            var extensionsFolder = "C:\\arbitrary-folder\\";
            var pluginNames = "BBB-FirstListedPlugin,AAA-SecondListedPlugin,CCC-ThirdListedPlugin";

            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.AppSettings[AppSettings.Keys.EXTENSIONS_FOLDER])
                .Returns(extensionsFolder)
                .MustBeCalled();
            Mock.Arrange(() => ConfigurationManager.AppSettings[AppSettings.Keys.PLUGIN_TYPES])
                .Returns(pluginNames)
                .MustBeCalled();

            Mock.SetupStatic(typeof(Directory));
            Mock.Arrange(() => Directory.Exists(Arg.Is<string>(extensionsFolder)))
                .Returns(true)
                .MustBeCalled();
            
            // Act
            var sut = new PluginLoaderConfigurationFromAppSettingsLoader();
            sut.Initialise(configuration, null);

            // Assert
            Mock.Assert(() => ConfigurationManager.AppSettings[AppSettings.Keys.EXTENSIONS_FOLDER]);
            Mock.Assert(() => ConfigurationManager.AppSettings[AppSettings.Keys.PLUGIN_TYPES]);
            Mock.Assert(() => Directory.Exists(Arg.Is<string>(extensionsFolder)));

            Assert.AreEqual(extensionsFolder, configuration.ExtensionsFolder);
            Assert.AreEqual(3, configuration.PluginTypes.Count);
            Assert.AreEqual("AAA-SecondListedPlugin".ToLower(), configuration.PluginTypes[0]);
            Assert.AreEqual("BBB-FirstListedPlugin".ToLower(), configuration.PluginTypes[1]);
            Assert.AreEqual("CCC-ThirdListedPlugin".ToLower(), configuration.PluginTypes[2]);
        }

        [TestMethod]
        public void InitialiseWithRelativePathReturnsFullPathFromAssembly()
        {
            // Arrange
            var configuration = new PluginLoaderConfiguration();
            var extensionsFolder = "arbitrary-relative-folder\\";
            var pluginNames = "BBB-FirstListedPlugin,AAA-SecondListedPlugin,CCC-ThirdListedPlugin";

            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.AppSettings[AppSettings.Keys.EXTENSIONS_FOLDER])
                .Returns(extensionsFolder)
                .MustBeCalled();
            Mock.Arrange(() => ConfigurationManager.AppSettings[AppSettings.Keys.PLUGIN_TYPES])
                .Returns(pluginNames)
                .MustBeCalled();

            Mock.SetupStatic(typeof(Directory));
            Mock.Arrange(() => Directory.Exists(Arg.IsAny<string>()))
                .Returns(true)
                .MustBeCalled();
            
            // Act
            var sut = new PluginLoaderConfigurationFromAppSettingsLoader();
            sut.Initialise(configuration, null);

            // Assert
            Mock.Assert(() => ConfigurationManager.AppSettings[AppSettings.Keys.EXTENSIONS_FOLDER]);
            Mock.Assert(() => ConfigurationManager.AppSettings[AppSettings.Keys.PLUGIN_TYPES]);
            Mock.Assert(() => Directory.Exists(Arg.IsAny<string>()));

            Assert.IsTrue(configuration.ExtensionsFolder.EndsWith(extensionsFolder));
            Assert.AreEqual(3, configuration.PluginTypes.Count);
            Assert.AreEqual("AAA-SecondListedPlugin".ToLower(), configuration.PluginTypes[0]);
            Assert.AreEqual("BBB-FirstListedPlugin".ToLower(), configuration.PluginTypes[1]);
            Assert.AreEqual("CCC-ThirdListedPlugin".ToLower(), configuration.PluginTypes[2]);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InitialiseWithInexistentFolderThrowsContractException()
        {
            // Arrange
            var configuration = new PluginLoaderConfiguration();
            var extensionsFolder = "C:\\arbitrary-inexistent-folder\\";
            var pluginNames = "BBB-FirstListedPlugin,AAA-SecondListedPlugin,CCC-ThirdListedPlugin";

            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.AppSettings[AppSettings.Keys.EXTENSIONS_FOLDER])
                .Returns(extensionsFolder)
                .MustBeCalled();
            Mock.Arrange(() => ConfigurationManager.AppSettings[AppSettings.Keys.PLUGIN_TYPES])
                .Returns(pluginNames)
                .MustBeCalled();

            Mock.SetupStatic(typeof(Directory));
            Mock.Arrange(() => Directory.Exists(Arg.Is<string>(extensionsFolder)))
                .Returns(false)
                .MustBeCalled();
            
            // Act
            var sut = new PluginLoaderConfigurationFromAppSettingsLoader();
            sut.Initialise(configuration, null);

            // Assert
            Mock.Assert(() => ConfigurationManager.AppSettings[AppSettings.Keys.EXTENSIONS_FOLDER]);
            Mock.Assert(() => ConfigurationManager.AppSettings[AppSettings.Keys.PLUGIN_TYPES]);
            Mock.Assert(() => Directory.Exists(Arg.Is<string>(extensionsFolder)));

            Assert.AreEqual(extensionsFolder, configuration.ExtensionsFolder);
            Assert.AreEqual(3, configuration.PluginTypes.Count);
            Assert.AreEqual("AAA-SecondListedPlugin".ToLower(), configuration.PluginTypes[0]);
            Assert.AreEqual("BBB-FirstListedPlugin".ToLower(), configuration.PluginTypes[1]);
            Assert.AreEqual("CCC-ThirdListedPlugin".ToLower(), configuration.PluginTypes[2]);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InitialiseWithEmptyPluginNamesThrowsContractException()
        {
            // Arrange
            var configuration = new PluginLoaderConfiguration();
            var extensionsFolder = "C:\\arbitrary-folder\\";
            var pluginNames = "";

            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.AppSettings[AppSettings.Keys.EXTENSIONS_FOLDER])
                .Returns(extensionsFolder)
                .MustBeCalled();
            Mock.Arrange(() => ConfigurationManager.AppSettings[AppSettings.Keys.PLUGIN_TYPES])
                .Returns(pluginNames)
                .MustBeCalled();

            Mock.SetupStatic(typeof(Directory));
            Mock.Arrange(() => Directory.Exists(Arg.Is<string>(extensionsFolder)))
                .Returns(true)
                .MustBeCalled();
            
            // Act
            var sut = new PluginLoaderConfigurationFromAppSettingsLoader();
            sut.Initialise(configuration, null);

            // Assert
            Mock.Assert(() => ConfigurationManager.AppSettings[AppSettings.Keys.EXTENSIONS_FOLDER]);
            Mock.Assert(() => ConfigurationManager.AppSettings[AppSettings.Keys.PLUGIN_TYPES]);
            Mock.Assert(() => Directory.Exists(Arg.Is<string>(extensionsFolder)));

            Assert.AreEqual(extensionsFolder, configuration.ExtensionsFolder);
        }

        [TestMethod]
        public void InitialiseWithDuplicatePluginNamesReturnsSinglePlugin()
        {
            // Arrange
            var configuration = new PluginLoaderConfiguration();
            var extensionsFolder = "C:\\arbitrary-folder\\";
            var pluginNames = "plugin,PLUGIN,PlUgIn";

            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.AppSettings[AppSettings.Keys.EXTENSIONS_FOLDER])
                .Returns(extensionsFolder)
                .MustBeCalled();
            Mock.Arrange(() => ConfigurationManager.AppSettings[AppSettings.Keys.PLUGIN_TYPES])
                .Returns(pluginNames)
                .MustBeCalled();

            Mock.SetupStatic(typeof(Directory));
            Mock.Arrange(() => Directory.Exists(Arg.Is<string>(extensionsFolder)))
                .Returns(true)
                .MustBeCalled();
            
            // Act
            var sut = new PluginLoaderConfigurationFromAppSettingsLoader();
            sut.Initialise(configuration, null);

            // Assert
            Mock.Assert(() => ConfigurationManager.AppSettings[AppSettings.Keys.EXTENSIONS_FOLDER]);
            Mock.Assert(() => ConfigurationManager.AppSettings[AppSettings.Keys.PLUGIN_TYPES]);
            Mock.Assert(() => Directory.Exists(Arg.Is<string>(extensionsFolder)));

            Assert.AreEqual(extensionsFolder, configuration.ExtensionsFolder);
            Assert.AreEqual(1, configuration.PluginTypes.Count);
            Assert.AreEqual("PlUgIn".ToLower(), configuration.PluginTypes[0]);
        }

        [TestMethod]
        public void InitialiseWithPluginNamesAndStarReturnsStarPlugin()
        {
            // Arrange
            var configuration = new PluginLoaderConfiguration();
            var extensionsFolder = "C:\\arbitrary-folder\\";
            var pluginNames = "*,Plugin1,Plugin2,Plugin3,*";

            Mock.SetupStatic(typeof(ConfigurationManager));
            Mock.Arrange(() => ConfigurationManager.AppSettings[AppSettings.Keys.EXTENSIONS_FOLDER])
                .Returns(extensionsFolder)
                .MustBeCalled();
            Mock.Arrange(() => ConfigurationManager.AppSettings[AppSettings.Keys.PLUGIN_TYPES])
                .Returns(pluginNames)
                .MustBeCalled();

            Mock.SetupStatic(typeof(Directory));
            Mock.Arrange(() => Directory.Exists(Arg.Is<string>(extensionsFolder)))
                .Returns(true)
                .MustBeCalled();
            
            // Act
            var sut = new PluginLoaderConfigurationFromAppSettingsLoader();
            sut.Initialise(configuration, null);

            // Assert
            Mock.Assert(() => ConfigurationManager.AppSettings[AppSettings.Keys.EXTENSIONS_FOLDER]);
            Mock.Assert(() => ConfigurationManager.AppSettings[AppSettings.Keys.PLUGIN_TYPES]);
            Mock.Assert(() => Directory.Exists(Arg.Is<string>(extensionsFolder)));

            Assert.AreEqual(extensionsFolder, configuration.ExtensionsFolder);
            Assert.AreEqual(1, configuration.PluginTypes.Count);
            Assert.AreEqual("*".ToLower(), configuration.PluginTypes[0]);
        }
    }
}
