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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.JustMock;
using biz.dfch.CS.Appclusive.Scheduler.Core;
using biz.dfch.CS.Testing.Attributes;
using System.IO;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions.Tests
{
    [TestClass]
    public class ThreadPoolUserWorkItemTest
    {
        [TestMethod]
        public void ThreadProcSucceeds()
        {
            // Arrange
            var data = new ThreadPoolUserWorkItemParameters()
            {
                ActivityId = Guid.NewGuid()
                ,
                Logger = new Logger()
                ,
                ScriptPathAndName = "C:\\arbitrary-folder\\arbitrary-script.ps1"
                ,
                ScriptParameters = new Dictionary<string,object>()
            };

            Mock.SetupStatic(typeof(File));
            
            var scriptResult = new List<object>();
            var scriptInvoker = Mock.Create<ScriptInvoker>();
            Mock.Arrange(() => scriptInvoker.RunPowershell(Arg.IsAny<string>(), Arg.IsAny<Dictionary<string, object>>(), ref scriptResult))
                .IgnoreInstance()
                .Returns(true)
                .MustBeCalled();

            // Act
            ThreadPoolUserWorkItem.ThreadProc(data);

            // Assert
            Mock.Assert(scriptInvoker);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ThreadProcWithNullParametersThrowsContractException()
        {
            // Arrange
            var data = default(ThreadPoolUserWorkItemParameters);

            // Act
            ThreadPoolUserWorkItem.ThreadProc(data);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void ThreadProcWithInvalidParametersThrowsContractException()
        {
            // Arrange
            var data = new ThreadPoolUserWorkItemParameters()
            {
                ActivityId = Guid.NewGuid()
                ,
                Logger = default(Logger)
                ,
                ScriptPathAndName = default(string)
                ,
                ScriptParameters = new Dictionary<string,object>()
            };

            // Act
            ThreadPoolUserWorkItem.ThreadProc(data);

            // Assert
            // N/A
        }
    }
}
