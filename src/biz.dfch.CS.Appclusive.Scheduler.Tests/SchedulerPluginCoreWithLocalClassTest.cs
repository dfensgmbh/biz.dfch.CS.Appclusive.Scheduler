using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.Utilities.Testing;

namespace biz.dfch.CS.Appclusive.Scheduler.Tests
{
    [TestClass]
    public class SchedulerPluginCoreWithLocalClassTest
    {
        class LocalSchedulerPluginCore : ISchedulerPluginCore
        {
            public void Log(string message)
            {
                return;
            }
        }

        [TestMethod]
        public void LocalSchedulerPluginCoreLogSucceeds()
        {
            // Arrange
            string message = "arbitrary-string";

            // Act
            var sut = new LocalSchedulerPluginCore();
            sut.Log(message);

            // Assert
            // N/A
        }

        [TestMethod]
        [ExpectContractFailure]
        public void LocalSchedulerPluginCoreLogNullThrowsContractException()
        {
            // Arrange
            string message = null;

            // Act
            var sut = new LocalSchedulerPluginCore();
            sut.Log(message);

            // Assert
            // N/A
        }
    }
}
