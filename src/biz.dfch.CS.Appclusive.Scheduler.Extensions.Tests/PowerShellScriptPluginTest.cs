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

using System.IO;
using Telerik.JustMock;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Scheduler.Core;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Utilities.Testing;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions.Tests
{
    [TestClass]
    public class PowerShellScriptPluginTest
    {
        private readonly DictionaryParameters parameters = new DictionaryParameters();
        public DictionaryParameters Parameters
        {
            get { return parameters; }
        }

        private readonly Logger logger = new Logger();
        public Logger Logger
        {
            get { return logger; }
        }

        private string path;
        public string Path
        {
            get { return path; }
            set { path = value; }
        }


        [TestInitialize]
        public void TestInitialize()
        {
            var appclusiveEndpoints = Mock.Create<AppclusiveEndpoints>(Constructor.Mocked);
            Parameters.Add(typeof(AppclusiveEndpoints).ToString(), appclusiveEndpoints);

            var uri = new UriBuilder(this.GetType().Assembly.CodeBase);
            Path = System.IO.Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
        }

        [TestMethod]
        public void InitialiseAndActivateSucceeds()
        {
            // Arrange
            var sut = new PowerShellScriptPlugin();

            // Act
            var result = sut.Initialise(Parameters, logger, true);

            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(sut.IsActive);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithEmptyParametersThrowsContractException()
        {
            // Arrange
            var sut = new PowerShellScriptPlugin();
            var initialisationResult = sut.Initialise(Parameters, logger, true);
            Assert.IsTrue(initialisationResult);
            Assert.IsTrue(sut.IsActive);

            var parameters = new DictionaryParameters();
            var invocationResult = new NonSerialisableJobResult();

            // Act
            var result = sut.Invoke(parameters, invocationResult);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithInexistentScriptFileThrowsContractException()
        {
            // Arrange
            var sut = new PowerShellScriptPlugin();
            var initialisationResult = sut.Initialise(Parameters, logger, true);
            Assert.IsTrue(initialisationResult);
            Assert.IsTrue(sut.IsActive);

            var parameters = new DictionaryParameters();
            parameters.Add(PowerShellScriptPlugin.SCRIPT_NAME_KEY, "C:\\inexistent-path\\inexistents-script-ps1");
            parameters.Add("SampleKey", "SampleValue");

            var invocationResult = new NonSerialisableJobResult();

            // Act
            var result = sut.Invoke(parameters, invocationResult);

            // Assert
            Assert.IsTrue(result);
        }

        [TestCategory("SkipOnTeamCity")]
        [TestMethod]
        public void InvokeWithArbitraryScriptFileSucceeds()
        {
            // Arrange
            var pathToScript = "C:\\arbitrary-path\\arbitrary-script-ps1";

            var sut = new PowerShellScriptPlugin();
            var initialisationResult = sut.Initialise(Parameters, logger, true);
            Assert.IsTrue(initialisationResult);
            Assert.IsTrue(sut.IsActive);

            Mock.SetupStatic(typeof(File));
            Mock.Arrange(() => File.Exists(Arg.Is<string>(pathToScript)))
                .Returns(true);

            var scriptResult = new List<object>();
            var scriptInvokeImpl = Mock.Create<ScriptInvokerImpl>();
            Mock.Arrange(() => scriptInvokeImpl.RunPowershell(Arg.Is<string>(pathToScript), Arg.IsAny<DictionaryParameters>(), ref scriptResult))
                .IgnoreInstance()
                .Returns(true)
                .MustBeCalled();

            var parameters = new DictionaryParameters();
            parameters.Add(PowerShellScriptPlugin.SCRIPT_NAME_KEY, pathToScript);
            parameters.Add("JobId", 42L);

            var invocationResult = new NonSerialisableJobResult();

            // Act
            var result = sut.Invoke(parameters, invocationResult);

            // Assert
            Mock.Assert(scriptInvokeImpl);
            Assert.IsTrue(result);
        }

        [TestCategory("SkipOnTeamCity")]
        [TestMethod]
        public void InvokeWithRealScriptFileSucceeds()
        {
            // Arrange
            var scriptName = "PowerShellScriptPluginTest.ps1";
            var pathToScript = System.IO.Path.Combine(Path, scriptName);

            var sut = new PowerShellScriptPlugin();
            var initialisationResult = sut.Initialise(Parameters, logger, true);
            Assert.IsTrue(initialisationResult);
            Assert.IsTrue(sut.IsActive);

            var parameters = new DictionaryParameters();
            parameters.Add(PowerShellScriptPlugin.SCRIPT_NAME_KEY, pathToScript);
            parameters.Add("JobId", 42L);

            var invocationResult = new NonSerialisableJobResult();

            // Act
            var result = sut.Invoke(parameters, invocationResult);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
