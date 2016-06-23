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

using biz.dfch.CS.Appclusive.Api.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Telerik.JustMock;

namespace biz.dfch.CS.Appclusive.Scheduler.Core.Tests
{
    [TestClass]
    public class ScheduledJobSchedulerTest
    {
        [TestMethod]
        public void GetNextScheduleForFiveStarsReturnsCurrentMinute()
        {
            // Arrange
            var scheduledJob = new ScheduledJob()
            {
                Crontab = "* * * * *"
            };

            var sut = new ScheduledJobScheduler(scheduledJob);

            var now = DateTimeOffset.Now;
            // Act
            var result = sut.GetNextSchedule(now);

            // Assert
            Assert.AreEqual(now.Minute, result.Minute);
        }

        [TestMethod]
        public void GetNextScheduleForFiveStarsReturnsMinValue()
        {
            // Arrange
            var scheduledJob = new ScheduledJob()
            {
                Crontab = "*/5 * * * *"
            };

            var sut = new ScheduledJobScheduler(scheduledJob);

            var now = new DateTimeOffset(2000, 08, 15, 05, 42, 13, TimeSpan.FromHours(0));

            // Act
            var result = sut.GetNextSchedule(now);

            // Assert
            Assert.AreEqual(DateTimeOffset.MinValue, result);
        }
        
        [TestMethod]
        public void IsScheduledToRunForFiveStarsReturnsTrue()
        {
            // Arrange
            var scheduledJob = new ScheduledJob()
            {
                Crontab = "* * * * *"
            };

            var sut = new ScheduledJobScheduler(scheduledJob);

            var now = DateTimeOffset.Now;
            
            // Act
            var result = sut.IsScheduledToRun(now);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsScheduledToRunForFiveOnesReturnsFalse()
        {
            // Arrange
            var scheduledJob = new ScheduledJob()
            {
                Crontab = "1 1 1 1 1"
            };

            var sut = new ScheduledJobScheduler(scheduledJob);

            var now = DateTimeOffset.Now;
            
            // Act
            var result = sut.IsScheduledToRun(now);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetNextScheduleForFiveOnesReturnsMinValue()
        {
            // Arrange
            var scheduledJob = new ScheduledJob()
            {
                Crontab = "1 1 1 1 1"
            };

            var sut = new ScheduledJobScheduler(scheduledJob);

            var now = DateTimeOffset.Now;
            
            // Act
            var result = sut.GetNextSchedule(now);

            // Assert
            Assert.AreEqual(DateTimeOffset.MinValue, result);
        }

        [TestMethod]
        public void IsScheduledToRunReturnsFalse()
        {
            // Arrange
            var now = DateTimeOffset.Parse("2019-04-13 08:15:42+02:00");
            var utcNow = new DateTimeOffset(now.UtcDateTime);

            Mock.SetupStatic(typeof(DateTimeOffset), Behavior.CallOriginal);
            Mock.Arrange(() => DateTimeOffset.Now).Returns(now);
            Mock.Arrange(() => DateTimeOffset.UtcNow).Returns(utcNow);
            Mock.Arrange(() => DateTime.Now).Returns(now.DateTime);
            Mock.Arrange(() => DateTime.UtcNow).Returns(utcNow.DateTime);

            var job = new ScheduledJob()
            {
                Crontab = "* 18 * * *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.IsScheduledToRun();

            // Assert
            Mock.Assert(() => DateTimeOffset.Now);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsScheduledToRunBeforeTheMinuteReturnsFalse()
        {
            // Arrange
            var now = DateTimeOffset.Parse("2019-04-13 17:59:59.999+02:00");
            var utcNow = new DateTimeOffset(now.UtcDateTime);

            Mock.SetupStatic(typeof(DateTimeOffset), Behavior.CallOriginal);
            Mock.Arrange(() => DateTimeOffset.Now).Returns(now);
            Mock.Arrange(() => DateTimeOffset.UtcNow).Returns(utcNow);
            Mock.Arrange(() => DateTime.Now).Returns(now.DateTime);
            Mock.Arrange(() => DateTime.UtcNow).Returns(utcNow.DateTime);

            var job = new ScheduledJob()
            {
                Crontab = "* 18 * * *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.IsScheduledToRun();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsScheduledToRunOnTheMinuteReturnsTrue()
        {
            // Arrange
            var now = DateTimeOffset.Parse("2019-04-13 18:00:00+02:00");
            var offsetTotalMinutes = now.Offset.TotalMinutes;
            var utcNow = new DateTimeOffset(now.UtcDateTime);

            Mock.SetupStatic(typeof(DateTimeOffset), Behavior.CallOriginal);
            Mock.Arrange(() => DateTimeOffset.Now).Returns(now);
            Mock.Arrange(() => DateTimeOffset.UtcNow).Returns(utcNow);
            //Mock.Arrange(() => DateTime.Now).Returns(now.DateTime.AddMinutes(offsetTotalMinutes));
            Mock.Arrange(() => DateTime.Now).Returns(now.DateTime);
            Mock.Arrange(() => DateTime.UtcNow).Returns(utcNow.DateTime);

            var job = new ScheduledJob()
            {
                Crontab = "* 18 * * *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.IsScheduledToRun();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsScheduledToRunAfterTheMinuteReturnsTrue()
        {
            // Arrange
            var now = DateTimeOffset.Parse("2019-04-13 18:00:00.001+02:00");
            var utcNow = new DateTimeOffset(now.UtcDateTime);

            Mock.SetupStatic(typeof(DateTimeOffset), Behavior.CallOriginal);
            Mock.Arrange(() => DateTimeOffset.Now).Returns(now);
            Mock.Arrange(() => DateTimeOffset.UtcNow).Returns(utcNow);
            Mock.Arrange(() => DateTime.Now).Returns(now.DateTime);
            Mock.Arrange(() => DateTime.UtcNow).Returns(utcNow.DateTime);

            var job = new ScheduledJob()
            {
                Crontab = "* 18 * * *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.IsScheduledToRun();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsScheduledToRunWithTwoHoursBeforeReturnsFalse()
        {
            // Arrange
            var now = DateTimeOffset.Parse("2019-04-13 16:00:00.001+02:00");
            var utcNow = new DateTimeOffset(now.UtcDateTime);

            Mock.SetupStatic(typeof(DateTimeOffset), Behavior.CallOriginal);
            Mock.Arrange(() => DateTimeOffset.Now).Returns(now);
            Mock.Arrange(() => DateTimeOffset.UtcNow).Returns(utcNow);
            Mock.Arrange(() => DateTime.Now).Returns(now.DateTime);
            Mock.Arrange(() => DateTime.UtcNow).Returns(utcNow.DateTime);

            var job = new ScheduledJob()
            {
                Crontab = "* 18 * * *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.IsScheduledToRun();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsScheduledToRunWithTwoHoursAfterReturnsFalse()
        {
            // Arrange
            var now = DateTimeOffset.Parse("2019-04-13 20:00:00.001+02:00");
            var utcNow = new DateTimeOffset(now.UtcDateTime);

            Mock.SetupStatic(typeof(DateTimeOffset), Behavior.CallOriginal);
            Mock.Arrange(() => DateTimeOffset.Now).Returns(now);
            Mock.Arrange(() => DateTimeOffset.UtcNow).Returns(utcNow);
            Mock.Arrange(() => DateTime.Now).Returns(now.DateTime);
            Mock.Arrange(() => DateTime.UtcNow).Returns(utcNow.DateTime);

            var job = new ScheduledJob()
            {
                Crontab = "* 18 * * *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.IsScheduledToRun();

            // Assert
            Assert.IsFalse(result);
        }
    }
}
