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
using System.ComponentModel.Composition;
using biz.dfch.CS.Utilities.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;
using System.Reflection;

namespace biz.dfch.CS.Appclusive.Scheduler.Public.Tests
{
    [TestClass]
    public class PluginLoaderTest
    {
        [Export(typeof(IAppclusivePlugin))]
        [ExportMetadata("Type", "PluginFromPublicTests")]
        [ExportMetadata("Priority", int.MinValue)]
        [ExportMetadata("Role", "default")]
        public class DefaultPlugin : SchedulerPluginBase
        {
            public override bool Invoke(DictionaryParameters parameters, IInvocationResult jobResult)
            {
                jobResult.Code = 0;
                jobResult.Message = "arbitrary-message";
                jobResult.Description = "arbitrary-description";
                jobResult.Succeeded = true;
                return jobResult.Succeeded;
            }
        }

        private static IConfigurationLoader loader;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext)
        {
            loader = Mock.Create<IConfigurationLoader>();
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
                .Returns(new List<string>() { PluginLoader.LOAD_ALL_PATTERN });
            
            var sut = new PluginLoader(loader);

            // Act
            sut.Initialise();

            // Assert
            Mock.Assert(loader);
            Mock.Assert(configuration);
            
            Assert.IsTrue(sut.IsInitialised);
        }
        
        [TestMethod]
        public void InitialiseExplicitConfigurationSucceedsAndSetIsInitialisedToTrue()
        {
            // Arrange
            Action<BaseDto> loader = 
                delegate(BaseDto configuration) 
                {
                    Contract.Requires(configuration is PluginLoaderConfiguration);

                    var cfg = configuration as PluginLoaderConfiguration;
                    cfg.ExtensionsFolder = ".";
                    cfg.PluginTypes = new List<string>() { PluginLoader.LOAD_ALL_PATTERN };
                    cfg.Assemblies = new List<Assembly>() { this.GetType().Assembly };
                };

            var sut = new PluginLoader();

            // Act
            sut.Initialise(loader);

            // Assert
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
        public void LoadSucceedsAndReturnsPluginFromPublicTestsAssembly()
        {
            // Arrange
            var configuration = Mock.Create<PluginLoaderConfiguration>();
            Mock.Arrange(() => configuration.ExtensionsFolder)
                .IgnoreInstance()
                .Returns(".");
            Mock.Arrange(() => configuration.PluginTypes)
                .IgnoreInstance()
                .Returns(new List<string>() { "PluginFromPublicTests" });
            
            var sut = new PluginLoader(loader);
            sut.Initialise();

            // Act
            var result = sut.Load();

            // Assert
            Mock.Assert(loader);
            Mock.Assert(configuration);
            
            Assert.IsNotNull(result);
            Assert.IsTrue(0 < result.Count);
            Assert.AreEqual("PluginFromPublicTests", result[0].Metadata.Type);
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
                .Returns(new List<string>() { PluginLoader.LOAD_ALL_PATTERN });
            var logger = Mock.Create<ILogger>();
            
            var sut = new PluginLoader(loader);
            sut.Initialise();
            var plugins = sut.Load();

            var parameters = new DictionaryParameters();
            parameters.Add("arbitrary-parameter-name", "arbitrary-parameter-value");
            var jobResult = new JobResult();

            var plugin = plugins[0].Value;
            plugin.Initialise(parameters, logger, true);

            // Act
            var result = plugin.Invoke(parameters, jobResult);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(jobResult.Succeeded);
        }

        class InnerClassWithPluginLoader
        {
            public List<Lazy<IAppclusivePlugin, IAppclusivePluginData>> Load()
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
                .Returns(new List<string>() { PluginLoader.LOAD_ALL_PATTERN });
            var logger = Mock.Create<ILogger>();

            var parameters = new DictionaryParameters();
            parameters.Add("arbitrary-parameter-name", "arbitrary-parameter-value");
            var jobResult = new JobResult();

            // Act
            var plugins = new InnerClassWithPluginLoader().Load();
            
            var plugin = plugins[0].Value;
            plugin.Initialise(parameters, logger, true);

            // Act
            var result = plugin.Invoke(parameters, jobResult);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(jobResult.Succeeded);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InitialiseAndLoadWithNullLoaderThrowsContractException()
        {
            // Arrange
            var sut = new PluginLoader();

            // Act
            sut.InitialiseAndLoad(default(IConfigurationLoader));

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InitialiseAndLoadWithoutConfigurationThrowsContractException()
        {
            // Arrange
            var sut = new PluginLoader();

            // Act
            sut.InitialiseAndLoad(default(IConfigurationLoader));

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InitialiseWithNullLoaderWithoutConfigurationThrowsContractException()
        {
            // Arrange
            var sut = new PluginLoader();

            // Act
            sut.Initialise(default(IConfigurationLoader));

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InitialiseWithNullActionWithoutConfigurationThrowsContractException()
        {
            // Arrange
            var sut = new PluginLoader();

            // Act
            sut.Initialise(default(Action<BaseDto>));

            // Assert
            // N/A
        }

        [TestMethod]
        public void InitialiseAndLoadSucceedsAndReturnsPluginFromPublicTestsAssembly()
        {
            // Arrange
            var pluginName = "PluginFromPublicTests";
            var configuration = Mock.Create<PluginLoaderConfiguration>();
            Mock.Arrange(() => configuration.ExtensionsFolder)
                .IgnoreInstance()
                .Returns(".");
            Mock.Arrange(() => configuration.PluginTypes)
                .IgnoreInstance()
                .Returns(new List<string>() { pluginName });
            
            var sut = new PluginLoader(loader);

            // Act
            var result = sut.InitialiseAndLoad();

            // Assert
            Mock.Assert(loader);
            Mock.Assert(configuration);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(pluginName, result[0].Metadata.Type);
            Assert.AreEqual(int.MinValue, result[0].Metadata.Priority);
        }

        [TestMethod]
        public void InitialiseAndLoadWithLoaderSucceedsAndReturnsPluginFromPublicTestsAssembly()
        {
            // Arrange
            var pluginName = "PluginFromPublicTests";
            var configuration = Mock.Create<PluginLoaderConfiguration>();
            Mock.Arrange(() => configuration.ExtensionsFolder)
                .IgnoreInstance()
                .Returns(".");
            Mock.Arrange(() => configuration.PluginTypes)
                .IgnoreInstance()
                .Returns(new List<string>() { pluginName });
            
            var sut = new PluginLoader();

            // Act
            var result = sut.InitialiseAndLoad(loader);

            // Assert
            Mock.Assert(loader);
            Mock.Assert(configuration);
            
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(pluginName, result[0].Metadata.Type);
            Assert.AreEqual(int.MinValue, result[0].Metadata.Priority);
        }

        [TestMethod]
        public void InitialiseAndLoadWithActionSucceedsAndReturnsPluginFromPublicTestsAssembly()
        {
            // Arrange
            var pluginName = "PluginFromPublicTests";
            var sut = new PluginLoader();

            // Act
            var result = sut.InitialiseAndLoad(configuration => 
            {
                    Contract.Requires(configuration is PluginLoaderConfiguration);

                    var cfg = configuration as PluginLoaderConfiguration;
                    cfg.ExtensionsFolder = ".";
                    cfg.PluginTypes = new List<string>() { pluginName };
                    cfg.Assemblies = new List<Assembly>() { this.GetType().Assembly };
            });

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(pluginName, result[0].Metadata.Type);
            Assert.AreEqual(int.MinValue, result[0].Metadata.Priority);
        }
    }
}
