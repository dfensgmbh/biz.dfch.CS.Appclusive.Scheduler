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
using biz.dfch.CS.Utilities.Testing;
using biz.dfch.CS.Appclusive.Scheduler.Public;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions.Tests
{
    [TestClass]
    public class ProgrammePluginTest
    {
        [TestMethod]
        public void LogSucceeds()
        {
            // Arrange
            string message = "tralala";
            string result = string.Empty;

            Mock.SetupStatic(typeof(Trace));
            
            Mock.Arrange(() => Trace.WriteLine(Arg.Is<string>(message)))
                .DoInstead((string traceMessage) => result = traceMessage)
                .OccursOnce();

            // Act
            var sut = new ProgrammePlugin();
            sut.Log(message);

            // Assert
            Mock.Assert(() => Trace.WriteLine(Arg.Is<string>(message)));
            Assert.AreEqual(result, message);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithMissingCommandLineThrowsContractException()
        {
            // Arrange
            var parameters = new SchedulerPluginParameters();
            parameters.Add("missing-parameter-CommandLine", "arbitrary-string");
            parameters.Add("WorkingDirectory", "arbitrary-string");
            parameters.Add("Credential", CredentialCache.DefaultCredentials);
            var jobResult = new JobResult();

            // Act
            var sut = new ProgrammePlugin();
            var result = sut.Invoke(parameters, ref jobResult);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithMissingWorkingDirectoryThrowsContractException()
        {
            // Arrange
            var parameters = new SchedulerPluginParameters();
            parameters.Add("CommandLine", "arbitrary-string");
            parameters.Add("missing-parameter-WorkingDirectory", "arbitrary-string");
            parameters.Add("Credential", CredentialCache.DefaultCredentials);
            var jobResult = new JobResult();

            // Act
            var sut = new ProgrammePlugin();
            var result = sut.Invoke(parameters, ref jobResult);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithMissingCredentialThrowsContractException()
        {
            // Arrange
            var parameters = new SchedulerPluginParameters();
            parameters.Add("CommandLine", "arbitrary-string");
            parameters.Add("WorkingDirectory", "arbitrary-string");
            parameters.Add("missing-parameter-Credential", CredentialCache.DefaultCredentials);
            var jobResult = new JobResult();

            // Act
            var sut = new ProgrammePlugin();
            var result = sut.Invoke(parameters, ref jobResult);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithInvalidTypeCommandLineThrowsContractException()
        {
            // Arrange
            var parameters = new SchedulerPluginParameters();
            parameters.Add("CommandLine", 42);
            parameters.Add("WorkingDirectory", "arbitrary-string");
            parameters.Add("Credential", CredentialCache.DefaultCredentials);
            var jobResult = new JobResult();

            // Act
            var sut = new ProgrammePlugin();
            var result = sut.Invoke(parameters, ref jobResult);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithInvalidTypeWorkingDirectoryThrowsContractException()
        {
            // Arrange
            var parameters = new SchedulerPluginParameters();
            parameters.Add("CommandLine", "arbitrary-string");
            parameters.Add("WorkingDirectory", 42);
            parameters.Add("Credential", CredentialCache.DefaultCredentials);
            var jobResult = new JobResult();

            // Act
            var sut = new ProgrammePlugin();
            var result = sut.Invoke(parameters, ref jobResult);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void InvokeWithInvalidTypeCredentialThrowsContractException()
        {
            // Arrange
            var parameters = new SchedulerPluginParameters();
            parameters.Add("CommandLine", "arbitrary-string");
            parameters.Add("WorkingDirectory", "arbitrary-string");
            parameters.Add("Credential", new object());
            var jobResult = new JobResult();

            // Act
            var sut = new ProgrammePlugin();
            var result = sut.Invoke(parameters, ref jobResult);

            // Assert
            // N/A
        }

        [TestMethod]
        public void InvokeSucceeds()
        {
            // Arrange
            var commandLine = "arbitrary-string";
            var workingDirectory = "arbitrary-string";
            NetworkCredential credential = (NetworkCredential) CredentialCache.DefaultCredentials;
            var parameters = new SchedulerPluginParameters();
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
            var result = sut.Invoke(parameters, ref jobResult);

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
            NetworkCredential credential = (NetworkCredential) CredentialCache.DefaultCredentials;
            var parameters = new SchedulerPluginParameters();
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
                .DoInstead(() => { throw new InvalidOperationException(); })
                .Returns(default(Dictionary<string, string>))
                .OccursOnce();

            // Act
            var sut = new ProgrammePlugin();
            var result = sut.Invoke(parameters, ref jobResult);

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
        public void UpdateConfigurationSucceeds()
        {
            // Arrange
            var configuration = new SchedulerPluginParameters();

            // Act
            var sut = new ProgrammePlugin();
            var result = sut.UpdateConfiguration(configuration);

            // Assert
            Assert.IsTrue(result);
        }
    }
}
