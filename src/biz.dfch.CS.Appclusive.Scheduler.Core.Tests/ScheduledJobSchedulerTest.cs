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
using biz.dfch.CS.Appclusive.Api.Core;

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
    }
}
