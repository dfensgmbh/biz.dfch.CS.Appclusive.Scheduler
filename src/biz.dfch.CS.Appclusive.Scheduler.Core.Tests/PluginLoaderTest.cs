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

using biz.dfch.CS.Appclusive.Scheduler.Public;
using biz.dfch.CS.Utilities.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;

namespace biz.dfch.CS.Appclusive.Scheduler.Core.Tests
{
    [TestClass]
    public class PluginLoaderTest
    {
        private static PluginLoaderConfigurationFromAppSettingsLoader loader;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            loader = Mock.Create<PluginLoaderConfigurationFromAppSettingsLoader>();
            Mock.Arrange(() => loader.Initialise(Arg.IsAny<BaseDto>(), Arg.IsAny<DictionaryParameters>()))
                .IgnoreInstance()
                .MustBeCalled();
        }
        
        [TestMethod]
        public void InitialiseSucceedsAndSetIsInitialisedToTrue()
        {
            // Arrange
            var configuration = Mock.Create<PluginLoaderConfiguration>();
            Mock.Arrange(() => configuration.ExtensionsFolder)
                .IgnoreInstance()
                .Returns(".");
            Mock.Arrange(() => configuration.PluginTypes)
                .IgnoreInstance()
                .Returns(new List<string>() { "*" });
            
            var sut = new PluginLoader(loader);

            // Act
            sut.Initialise();

            // Assert
            Mock.Assert(loader);
            Mock.Assert(configuration);
            
            Assert.IsTrue(sut.IsInitialised);
        }
        
        [TestMethod]
        [ExpectContractFailure]
        public void LoadWithoutInitialiseThrowsContractException()
        {
            // Arrange
            var sut = new PluginLoader(loader);

            // Act
            var result = sut.Load();

            // Assert
            // N/A
        }
        
        [TestMethod]
        public void LoadSucceedsAndReturnsCoreDefaultPlugin()
        {
            // Arrange
            var configuration = Mock.Create<PluginLoaderConfiguration>();
            Mock.Arrange(() => configuration.ExtensionsFolder)
                .IgnoreInstance()
                .Returns(".");
            Mock.Arrange(() => configuration.PluginTypes)
                .IgnoreInstance()
                .Returns(new List<string>() { "*" });
            
            var sut = new PluginLoader(loader);
            sut.Initialise();

            // Act
            var result = sut.Load();

            // Assert
            Mock.Assert(loader);
            Mock.Assert(configuration);
            
            Assert.IsNotNull(result);
            Assert.IsTrue(0 < result.Count);
            Assert.AreEqual("Default", result[0].Metadata.Type);
            Assert.AreEqual(int.MinValue, result[0].Metadata.Priority);
        }

        [TestMethod]
        public void LoadAndInvokePluginSucceeds()
        {
            // Arrange
            var configuration = Mock.Create<PluginLoaderConfiguration>();
            Mock.Arrange(() => configuration.ExtensionsFolder)
                .IgnoreInstance()
                .Returns(".");
            Mock.Arrange(() => configuration.PluginTypes)
                .IgnoreInstance()
                .Returns(new List<string>() { "*" });
            
            var sut = new PluginLoader(loader);
            sut.Initialise();
            var plugins = sut.Load();

            var parameters = new DictionaryParameters();
            parameters.Add("arbitrary-parameter-name", "arbitrary-parameter-value");
            var jobResult = new JobResult();

            var plugin = plugins[0].Value;
            plugin.Initialise(parameters, new Logger(), true);

            // Act
            var result = plugin.Invoke(parameters, jobResult);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(jobResult.Succeeded);
        }

        class InnerClassWithPluginLoader
        {
            public List<Lazy<ISchedulerPlugin, ISchedulerPluginData>> Load()
            {
                var sut = new PluginLoader(loader);
                sut.Initialise();
                var plugins = sut.Load();
                return plugins;
            }
        }

        [TestMethod]
        public void LoadFromInnerClassAndInvokePluginSucceeds()
        {
            // Arrange
            var configuration = Mock.Create<PluginLoaderConfiguration>();
            Mock.Arrange(() => configuration.ExtensionsFolder)
                .IgnoreInstance()
                .Returns(".");
            Mock.Arrange(() => configuration.PluginTypes)
                .IgnoreInstance()
                .Returns(new List<string>() { "*" });

            var parameters = new DictionaryParameters();
            parameters.Add("arbitrary-parameter-name", "arbitrary-parameter-value");
            var jobResult = new JobResult();

            // Act
            var plugins = new InnerClassWithPluginLoader().Load();
            
            var plugin = plugins[0].Value;
            plugin.Initialise(parameters, new Logger(), true);

            // Act
            var result = plugin.Invoke(parameters, jobResult);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(jobResult.Succeeded);
        }
    }
}
