using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Telerik.JustMock;
using biz.dfch.CS.Utilities.Testing;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions.Tests
{
    [TestClass]
    public class DefaultPluginTest
    {
        [TestMethod]
        public void RequiresTest()
        {
            Contract.Requires(false == true);
        }

        [TestMethod]
        public void AssertTest()
        {
            Contract.Assert(false == true);
        }

        [TestMethod]
        [ExpectContractFailure]
        public void LogThrowsContractException()
        {
            // Arrange
            var sut = new DefaultPlugin();
            //var message = string.Empty;
            string message = null;

            //Mock.SetupStatic(typeof(DateTimeOffset), Behavior.Loose);
            //Mock.Arrange(() => Trace.WriteLine(Arg.Is<string>(message)))
            //    .OccursNever();

            // Act
            sut.Log(message);

            // Assert
            //Mock.Assert(() => Trace.WriteLine(Arg.Is<string>(message)));
        }

        [TestMethod]
        public void LogSucceeds()
        {
            // Arrange
            var sut = new DefaultPlugin();
            var message = "arbitrary-message";

            Mock.SetupStatic(typeof(DateTimeOffset), Behavior.Loose);
            Mock.Arrange(() => Trace.WriteLine(Arg.Is<string>(message)))
                .OccursOnce();

            // Act
            sut.Log(message);

            // Assert
            Mock.Assert(() => Trace.WriteLine(Arg.Is<string>(message)));
        }
    }
}
