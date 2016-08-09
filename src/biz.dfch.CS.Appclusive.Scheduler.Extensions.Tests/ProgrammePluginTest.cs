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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using System.Diagnostics;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Scheduler.Core;
using biz.dfch.CS.Utilities.Testing;
using biz.dfch.CS.Appclusive.Scheduler.Public.Tests;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions.Tests
{
    [TestClass]
    public class ProgrammePluginTest
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
        [ExpectContractFailure]
        public void InvokeWithMissingCommandLineThrowsContractException()
        {
            // Arrange
            var parameters = new DictionaryParameters();
            parameters.Add("missing-parameter-CommandLine", "arbitrary-string");
            parameters.Add("WorkingDirectory", "arbitrary-string");
            parameters.Add("Credential", CredentialCache.DefaultCredentials);
            var jobResult = new JobResult();

            // Act
            var sut = new ProgrammePlugin();
            var result = sut.Invoke(parameters, jobResult);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithMissingWorkingDirectoryThrowsContractException()
        {
            // Arrange
            var parameters = new DictionaryParameters();
            parameters.Add("CommandLine", "arbitrary-string");
            parameters.Add("missing-parameter-WorkingDirectory", "arbitrary-string");
            parameters.Add("Credential", CredentialCache.DefaultCredentials);
            var jobResult = new JobResult();

            // Act
            var sut = new ProgrammePlugin();
            var result = sut.Invoke(parameters, jobResult);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithMissingCredentialThrowsContractException()
        {
            // Arrange
            var parameters = new DictionaryParameters();
            parameters.Add("CommandLine", "arbitrary-string");
            parameters.Add("WorkingDirectory", "arbitrary-string");
            parameters.Add("missing-parameter-Credential", CredentialCache.DefaultCredentials);
            var jobResult = new JobResult();

            // Act
            var sut = new ProgrammePlugin();
            var result = sut.Invoke(parameters, jobResult);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithInvalidTypeCommandLineThrowsContractException()
        {
            // Arrange
            var parameters = new DictionaryParameters();
            parameters.Add("CommandLine", new InvalidObject());
            parameters.Add("WorkingDirectory", "arbitrary-string");
            parameters.Add("Credential", CredentialCache.DefaultCredentials);
            var jobResult = new JobResult();

            // Act
            var sut = new ProgrammePlugin();
            var result = sut.Invoke(parameters, jobResult);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithInvalidTypeWorkingDirectoryThrowsContractException()
        {
            // Arrange
            var parameters = new DictionaryParameters();
            parameters.Add("CommandLine", "arbitrary-string");
            parameters.Add("WorkingDirectory", new InvalidObject());
            parameters.Add("Credential", CredentialCache.DefaultCredentials);
            var jobResult = new JobResult();

            // Act
            var sut = new ProgrammePlugin();
            var result = sut.Invoke(parameters, jobResult);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithInvalidTypeCredentialThrowsContractException()
        {
            // Arrange
            var parameters = new DictionaryParameters();
            parameters.Add("CommandLine", "arbitrary-string");
            parameters.Add("WorkingDirectory", "arbitrary-string");
            parameters.Add("Credential", new object());
            var jobResult = new JobResult();

            // Act
            var sut = new ProgrammePlugin();
            var result = sut.Invoke(parameters, jobResult);

            // Assert
            // N/A
        }

        [TestMethod]
        public void InvokeSucceeds()
        {
            // Arrange
            var commandLine = "arbitrary-string";
            var workingDirectory = "arbitrary-string";
            var credential = new NetworkCredential("arbitrary-user", "arbitrary-password", "arbitrary-domain");
            var parameters = new DictionaryParameters();
            parameters.Add("JobId", 42);
            parameters.Add("CommandLine", commandLine);
            parameters.Add("WorkingDirectory", workingDirectory);
            parameters.Add("Credential", credential);
            var jobResult = new JobResult();

            Mock.SetupStatic(typeof(biz.dfch.CS.Utilities.Process));

            Mock.Arrange(
                () => biz.dfch.CS.Utilities.Process.StartProcess(
                    Arg.Is<string>(commandLine), 
                    Arg.Is<string>(workingDirectory), 
                    Arg.Is<NetworkCredential>(credential))
                )
                .Returns(default(Dictionary<string, string>))
                .OccursOnce();

            // Act
            var sut = new ProgrammePlugin();
            sut.Initialise(new DictionaryParameters(), new Logger(), true);
            var result = sut.Invoke(parameters, jobResult);

            // Assert
            Mock.Assert(
                () => biz.dfch.CS.Utilities.Process.StartProcess(
                    Arg.Is<string>(commandLine), 
                    Arg.Is<string>(workingDirectory), 
                    Arg.Is<NetworkCredential>(credential))
                );
            Assert.IsTrue(result);
            Assert.IsTrue(jobResult.Succeeded);
            Assert.AreEqual(0, jobResult.Code);
        }

        [TestMethod]
        public void InvokeFails()
        {
            // Arrange
            var commandLine = "arbitrary-string";
            var workingDirectory = "arbitrary-string";
            var credential = new NetworkCredential("arbitrary-user", "arbitrary-password", "arbitrary-domain");
            var parameters = new DictionaryParameters();
            parameters.Add("JobId", 42);
            parameters.Add("CommandLine", commandLine);
            parameters.Add("WorkingDirectory", workingDirectory);
            parameters.Add("Credential", credential);
            var jobResult = new JobResult();

            Mock.SetupStatic(typeof(biz.dfch.CS.Utilities.Process));

            Mock.Arrange(
                () => biz.dfch.CS.Utilities.Process.StartProcess(
                    Arg.Is<string>(commandLine),
                    Arg.Is<string>(workingDirectory),
                    Arg.Is<NetworkCredential>(credential))
                )
                .Throws<InvalidOperationException>()
                .OccursOnce();

            // Act
            var sut = new ProgrammePlugin();
            sut.Initialise(new DictionaryParameters(), new Logger(), true);
            var result = sut.Invoke(parameters, jobResult);

            // Assert
            Mock.Assert(
                () => biz.dfch.CS.Utilities.Process.StartProcess(
                    Arg.Is<string>(commandLine), 
                    Arg.Is<string>(workingDirectory), 
                    Arg.Is<NetworkCredential>(credential))
                );
            Assert.IsFalse(result);
            Assert.IsFalse(jobResult.Succeeded);
            Assert.AreNotEqual(0, jobResult.Code);
            Assert.IsFalse(string.IsNullOrWhiteSpace(jobResult.Message));
            Assert.IsFalse(string.IsNullOrWhiteSpace(jobResult.Description));
        }

        [TestMethod]
        public void SetConfigurationSucceeds()
        {
            // Arrange
            var configuration = new DictionaryParameters();

            // Act
            var sut = new ProgrammePlugin();
            sut.Configuration = configuration;

            // Assert
            Assert.AreEqual(configuration, sut.Configuration);
        }

        [TestMethod]
        public void GetConfigurationSucceeds()
        {
            // Arrange
            var configuration = new DictionaryParameters();
            var sut = new ProgrammePlugin();
            sut.Initialise(configuration, new Logger(), true);

            // Act
            configuration = sut.Configuration;

            // Assert
            Assert.AreEqual(configuration, sut.Configuration);
        }
    }
}
