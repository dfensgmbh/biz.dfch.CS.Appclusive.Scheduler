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
using biz.dfch.CS.Appclusive.Public.Logging;
using biz.dfch.CS.Utilities.Testing;

namespace biz.dfch.CS.Appclusive.Scheduler.Public.Tests
{
    [TestClass]
    public class ILoggerTest
    {
        class LoggerImpl : ILogger
        {
            public void Write(string message)
            {
                return;
            }

            public void Write(string format, params object[] args)
            {
                return;
            }

            public void WriteLine(string message)
            {
                return;
            }

            public void WriteLine(string format, params object[] args)
            {
                return;
            }

            public void Debug(string message)
            {
                return;
            }

            public void Debug(string format, params object[] args)
            {
                return;
            }

            public void Info(string message)
            {
                return;
            }

            public void Info(string format, params object[] args)
            {
                return;
            }

            public void Notice(string message)
            {
                return;
            }

            public void Notice(string format, params object[] args)
            {
                return;
            }

            public void Warn(string message)
            {
                return;
            }

            public void Warn(string format, params object[] args)
            {
                return;
            }

            public void Error(string message)
            {
                return;
            }

            public void Error(string format, params object[] args)
            {
                return;
            }

            public void Alert(string message)
            {
                return;
            }

            public void Alert(string format, params object[] args)
            {
                return;
            }

            public void Critical(string message)
            {
                return;
            }

            public void Critical(string format, params object[] args)
            {
                return;
            }

            public void Emergency(string message)
            {
                return;
            }

            public void Emergency(string format, params object[] args)
            {
                return;
            }
        }

        class Logger : ILogger
        {
            public void Write(string message)
            {
                new LoggerImpl().Write(message);
            }

            public void Write(string format, params object[] args)
            {
                new LoggerImpl().Write(format, args);
            }

            public void WriteLine(string message)
            {
                new LoggerImpl().WriteLine(message);
            }

            public void WriteLine(string format, params object[] args)
            {
                new LoggerImpl().WriteLine(format, args);
            }

            public void Debug(string message)
            {
                new LoggerImpl().Debug(message);
            }

            public void Debug(string format, params object[] args)
            {
                new LoggerImpl().Debug(format, args);
            }

            public void Info(string message)
            {
                new LoggerImpl().Info(message);
            }

            public void Info(string format, params object[] args)
            {
                new LoggerImpl().Info(format, args);
            }

            public void Notice(string message)
            {
                new LoggerImpl().Notice(message);
            }

            public void Notice(string format, params object[] args)
            {
                new LoggerImpl().Notice(format, args);
            }

            public void Warn(string message)
            {
                new LoggerImpl().Warn(message);
            }

            public void Warn(string format, params object[] args)
            {
                new LoggerImpl().Warn(format, args);
            }

            public void Error(string message)
            {
                new LoggerImpl().Error(message);
            }

            public void Error(string format, params object[] args)
            {
                new LoggerImpl().Error(format, args);
            }

            public void Alert(string message)
            {
                new LoggerImpl().Alert(message);
            }

            public void Alert(string format, params object[] args)
            {
                new LoggerImpl().Alert(format, args);
            }

            public void Critical(string message)
            {
                new LoggerImpl().Critical(message);
            }

            public void Critical(string format, params object[] args)
            {
                new LoggerImpl().Critical(format, args);
            }

            public void Emergency(string message)
            {
                new LoggerImpl().Emergency(message);
            }

            public void Emergency(string format, params object[] args)
            {
                new LoggerImpl().Emergency(format, args);
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
