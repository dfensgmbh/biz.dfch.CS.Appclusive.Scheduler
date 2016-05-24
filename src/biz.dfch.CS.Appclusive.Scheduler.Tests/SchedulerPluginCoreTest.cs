using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Utilities.Testing;

namespace biz.dfch.CS.Appclusive.Scheduler.Tests
{
    [TestClass]
    public class SchedulerPluginCoreTest
    {
        [TestMethod]
        public void SchedulerPluginCoreLogSucceeds()
        {
            // Arrange
            string message = "arbitrary-string";

            // Act
            var sut = new SchedulerPluginCore();
            sut.Log(message);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void SchedulerPluginCoreLogNullThrowsContractException()
        {
            // Arrange
            string message = null;

            // Act
            var sut = new SchedulerPluginCore();
            sut.Log(message);

            // Assert
            // N/A
        }
    }
}
