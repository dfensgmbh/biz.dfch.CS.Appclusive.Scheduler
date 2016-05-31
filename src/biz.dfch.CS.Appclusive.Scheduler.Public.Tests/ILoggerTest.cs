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
using biz.dfch.CS.Utilities.Testing;

namespace biz.dfch.CS.Appclusive.Scheduler.Public.Tests
{
    [TestClass]
    public class ILoggerTest
    {
        class LoggerImpl : ILogger
        {
            public void Write(string format, params object[] args)
            {
                return;
            }

            public void WriteLine(string format, params object[] args)
            {
                return;
            }
        }

        class Logger : ILogger
        {
            public void Write(string format, params object[] args)
            {
                new LoggerImpl().Write(format, args);
            }

            public void WriteLine(string format, params object[] args)
            {
                new LoggerImpl().WriteLine(format, args);
            }
        }

        [TestMethod]
        [ExpectContractFailure]
        public void WriteWithEmptyStringThrowsContractException()
        {
            // Arrange
            var message = string.Empty;
            var arg0 = default(object);
            var arg1 = default(object);

            // Act
            var sut = new Logger();
            sut.Write(message, arg0, arg1);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void WriteLineWithEmptyStringThrowsContractException()
        {
            // Arrange
            var message = string.Empty;
            var arg0 = default(object);
            var arg1 = default(object);

            // Act
            var sut = new Logger();
            sut.WriteLine(message, arg0, arg1);

            // Assert
            // N/A
        }

        [TestMethod]
        public void WriteWithSucceeds()
        {
            // Arrange
            var message = "{0}{1}";
            var arg0 = "42";
            var arg1 = "arbitrary-string";

            // Act
            var sut = new Logger();
            sut.Write(message, arg0, arg1);

            // Assert
            // N/A
        }

        [TestMethod]
        public void WriteLineSucceeds()
        {
            // Arrange
            var message = "{0}{1}";
            var arg0 = "42";
            var arg1 = "arbitrary-string";

            var loggerImpl = Mock.Create<LoggerImpl>();
            Mock.Arrange(() => loggerImpl.WriteLine(Arg.Is<string>(message), Arg.Is<string>(arg0), Arg.Is<string>(arg1)))
                .IgnoreInstance()
                .OccursOnce();

            // Act
            var sut = new Logger();
            sut.WriteLine(message, arg0, arg1);

            // Assert
            Mock.Assert(loggerImpl);
        }
    }
}
