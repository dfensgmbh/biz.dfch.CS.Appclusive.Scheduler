using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using biz.dfch.CS.Utilities.Testing;
using System.Collections.Specialized;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using System.Collections.Generic;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions.Tests
{
    [TestClass]
    public class DefaultPluginTest
    {
        [TestMethod]
        public void LogSucceeds()
        {
            // Arrange
            var message = "arbitrary-message";

            Mock.Arrange(() => Trace.WriteLine(Arg.Is<string>(message)))
                .OccursOnce();

            // Act
            var sut = new DefaultPlugin();
            sut.Log(message);

            // Assert
            Mock.Assert(() => Trace.WriteLine(Arg.Is<string>(message)));
        }

        [TestMethod]
        public void InvokeConfigurationSucceeds()
        {
            // Arrange
            var parameters = new DictionaryParameters();
            var key1 = "arbitrary-key1";
            var value1 = "arbitrary-value";
            parameters.Add(key1, value1);
            var key2 = "arbitrary-key2";
            var value2 = 42;
            parameters.Add(key2, value2);
            var key3 = "arbitrary-key3";
            var value3 = new object();
            parameters.Add(key3, value3);

            var jobResult = new JobResult();

            Mock.Arrange(() => Trace.WriteLine(Arg.IsAny<string>()))
                .OccursOnce();

            // Act
            var sut = new DefaultPlugin();
            var result = sut.Invoke(parameters, ref jobResult);

            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(jobResult);
            Assert.IsTrue(jobResult.Description.Contains(key1));
            Assert.IsTrue(jobResult.Description.Contains(value1.ToString()));
            Assert.IsTrue(jobResult.Description.Contains(key2));
            Assert.IsTrue(jobResult.Description.Contains(value2.ToString()));
            Assert.IsTrue(jobResult.Description.Contains(key3));
            Assert.IsTrue(jobResult.Description.Contains(value3.ToString()));

            Mock.Assert(() => Trace.WriteLine(Arg.IsAny<string>()));
        }
    }
}
