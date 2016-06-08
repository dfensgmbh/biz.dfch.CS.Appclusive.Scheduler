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
using System.ServiceProcess;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;
using biz.dfch.CS.Utilities.Logging;

namespace biz.dfch.CS.Appclusive.Scheduler.Core.Tests
{
    [TestClass]
    public class ProgramTest
    {
        [TestMethod]
        public void ProgramMainUserInteractiveTrueSucceeds()
        {
            // Arrange
            Mock.SetupStatic(typeof(Environment));
            Mock.Arrange(() => Environment.UserInteractive)
                .Returns(true)
                .MustBeCalled();

            Mock.SetupStatic(typeof(ServiceBase));
            Mock.Arrange(() => ServiceBase.Run(Arg.IsAny<ServiceBase[]>()))
                .OccursNever();

            var appclusiveSchedulerService = Mock.Create<AppclusiveSchedulerService>();
            Mock.Arrange(() => appclusiveSchedulerService.OnStartInteractive(Arg.IsAny<string[]>()))
                .IgnoreInstance()
                .DoNothing()
                .MustBeCalled();

            var args = new string[] { "ARG0", "ARG1", "ARG2" };

            // Act
            Program.Main(args);

            // Assert
            Mock.Assert(() => Environment.UserInteractive);

            Mock.Assert(() => ServiceBase.Run(Arg.IsAny<ServiceBase[]>()));

            Mock.Assert(appclusiveSchedulerService);
        }

        [TestMethod]
        public void ProgramMainHelpMessage()
        {
            // Arrange
            Mock.SetupStatic(typeof(Environment));
            Mock.Arrange(() => Environment.UserInteractive)
                .Returns(true)
                .MustBeCalled();

            Mock.SetupStatic(typeof(ServiceBase));
            Mock.Arrange(() => ServiceBase.Run(Arg.IsAny<ServiceBase[]>()))
                .OccursNever();

            var appclusiveSchedulerService = Mock.Create<AppclusiveSchedulerService>();
            Mock.Arrange(() => appclusiveSchedulerService.OnStartInteractive(Arg.IsAny<string[]>()))
                .IgnoreInstance()
                .DoNothing()
                .OccursNever();

            var help = Mock.Create<ProgramHelp>();
            Mock.Arrange(() => help.GetHelpMessage())
                .IgnoreInstance()
                .CallOriginal()
                .MustBeCalled();

            var args = new string[] { "--help"};

            // Act
            Program.Main(args);

            // Assert
            Mock.Assert(() => Environment.UserInteractive);

            Mock.Assert(() => ServiceBase.Run(Arg.IsAny<ServiceBase[]>()));

            Mock.Assert(appclusiveSchedulerService);

            Mock.Assert(help);
        }
        
        [TestMethod]
        public void ProgramMainEncryptMessage()
        {
            // Arrange
            Mock.SetupStatic(typeof(Environment));
            Mock.Arrange(() => Environment.UserInteractive)
                .Returns(true)
                .MustBeCalled();

            Mock.SetupStatic(typeof(ServiceBase));
            Mock.Arrange(() => ServiceBase.Run(Arg.IsAny<ServiceBase[]>()))
                .OccursNever();

            var appclusiveSchedulerService = Mock.Create<AppclusiveSchedulerService>();
            Mock.Arrange(() => appclusiveSchedulerService.OnStartInteractive(Arg.IsAny<string[]>()))
                .IgnoreInstance()
                .DoNothing()
                .OccursNever();

            var help = Mock.Create<ProgramHelp>();
            Mock.Arrange(() => help.GetEncryptMessage())
                .IgnoreInstance()
                .CallOriginal()
                .MustBeCalled();

            var appclusiveCredentialSectionManager = Mock.Create<AppclusiveCredentialSectionManager>();
            Mock.Arrange(() => appclusiveCredentialSectionManager.Encrypt())
                .IgnoreInstance()
                .DoNothing()
                .MustBeCalled();

            var args = new string[] { "-encrypt"};

            // Act
            Program.Main(args);

            // Assert
            Mock.Assert(() => Environment.UserInteractive);

            Mock.Assert(() => ServiceBase.Run(Arg.IsAny<ServiceBase[]>()));

            Mock.Assert(appclusiveSchedulerService);

            Mock.Assert(help);
            Mock.Assert(appclusiveCredentialSectionManager);
        }
        
        [TestMethod]
        public void ProgramMainDecryptMessage()
        {
            // Arrange
            Mock.SetupStatic(typeof(Environment));
            Mock.Arrange(() => Environment.UserInteractive)
                .Returns(true)
                .MustBeCalled();

            Mock.SetupStatic(typeof(ServiceBase));
            Mock.Arrange(() => ServiceBase.Run(Arg.IsAny<ServiceBase[]>()))
                .OccursNever();

            var appclusiveSchedulerService = Mock.Create<AppclusiveSchedulerService>();
            Mock.Arrange(() => appclusiveSchedulerService.OnStartInteractive(Arg.IsAny<string[]>()))
                .IgnoreInstance()
                .DoNothing()
                .OccursNever();

            var help = Mock.Create<ProgramHelp>();
            Mock.Arrange(() => help.GetEncryptMessage())
                .IgnoreInstance()
                .CallOriginal()
                .MustBeCalled();

            var appclusiveCredentialSectionManager = Mock.Create<AppclusiveCredentialSectionManager>();
            Mock.Arrange(() => appclusiveCredentialSectionManager.Decrypt())
                .IgnoreInstance()
                .DoNothing()
                .MustBeCalled();

            var args = new string[] { "/decrypt"};

            // Act
            Program.Main(args);

            // Assert
            Mock.Assert(() => Environment.UserInteractive);

            Mock.Assert(() => ServiceBase.Run(Arg.IsAny<ServiceBase[]>()));

            Mock.Assert(appclusiveSchedulerService);

            Mock.Assert(help);
            Mock.Assert(appclusiveCredentialSectionManager);
        }
        
        [TestMethod]
        public void ProgramMainUserInteractiveFalseSucceeds()
        {
            // Arrange
            Mock.SetupStatic(typeof(Environment));
            Mock.Arrange(() => Environment.UserInteractive)
                .Returns(false)
                .MustBeCalled();

            Mock.SetupStatic(typeof(ServiceBase));
            Mock.Arrange(() => ServiceBase.Run(Arg.IsAny<ServiceBase[]>()))
                .DoNothing()
                .MustBeCalled();

            var appclusiveSchedulerService = Mock.Create<AppclusiveSchedulerService>();
            Mock.Arrange(() => appclusiveSchedulerService.OnStartInteractive(Arg.IsAny<string[]>()))
                .IgnoreInstance()
                .DoNothing()
                .OccursNever();

            var args = new string[] { "ARG0", "ARG1", "ARG2" };

            // Act
            Program.Main(args);

            // Assert
            Mock.Assert(() => Environment.UserInteractive);

            Mock.Assert(() => ServiceBase.Run(Arg.IsAny<ServiceBase[]>()));

            Mock.Assert(appclusiveSchedulerService);
        }
    }
}
