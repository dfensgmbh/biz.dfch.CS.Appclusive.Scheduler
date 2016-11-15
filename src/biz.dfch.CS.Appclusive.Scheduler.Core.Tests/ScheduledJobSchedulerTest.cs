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
using System.Linq;
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

        // self assuring tests regarding DateTime
        [TestMethod]
        public void DateTimeDefaultIsEqualToDateTimeMinValue()
        {
            var sut = default(DateTime);

            Assert.AreEqual(DateTime.MinValue, sut);
        }

        [TestMethod]
        public void DateTimeDefaultIsNotEqualNull()
        {
            var sut = default(DateTime);

            Assert.AreNotEqual(null, sut);
        }

        [TestMethod]
        public void LastOrDefaultReturnsDefaultDateTimeAndNotNull()
        {
            var dateTimes = new DateTime[]
            {
                // empty array
            };

            var result = dateTimes.FirstOrDefault();

            Assert.AreNotEqual(null, result);
        }

        [TestMethod]
        public void IsValidCrontabExpressionWithValidExpressionReturnsTrue()
        {
            // Arrange
            var job = new ScheduledJob()
            {
                Crontab = "* 18 * * *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.IsCrontabExpression();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValidCrontabExpressionWithInvalidExpressionReturnsFalse()
        {
            // Arrange
            var job = new ScheduledJob()
            {
                Crontab = "* 18 * **"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.IsCrontabExpression();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidCrontabExpressionWithValidQuartzExpressionReturnsFalse()
        {
            // Arrange
            var job = new ScheduledJob()
            {
                Crontab = "0 0/1 * 1/1 * ? *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.IsCrontabExpression();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidQuartzssionWithValidCrontabExpressionReturnsFalse()
        {
            // Arrange
            var job = new ScheduledJob()
            {
                Crontab = "* 18 * * *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.IsQuartzExpression();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidQuartzExpressionWithInvalidExpressionReturnsFalse()
        {
            // Arrange
            var job = new ScheduledJob()
            {
                Crontab = "* 18 * **"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.IsQuartzExpression();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsValidQuartzExpressionWithValidQuartzExpressionReturnsTrue()
        {
            // Arrange
            var job = new ScheduledJob()
            {
                Crontab = "0 0/1 * 1/1 * ? *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.IsQuartzExpression();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetNextScheduleFromQuartzExpressionSucceeds()
        {
            // Arrange
            var withinThisMinute = DateTimeOffset.Now;
            var expected = withinThisMinute;
            expected = expected.AddSeconds(-1 * expected.Second);
            expected = expected.AddMilliseconds(-1 * expected.Millisecond);
            
            var job = new ScheduledJob()
            {
                Crontab = "0 0/1 * 1/1 * ? *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.GetNextScheduleFromQuartzExpression(withinThisMinute);

            // Assert
            Assert.AreNotEqual(DateTimeOffset.MinValue, result);
            Assert.IsTrue(Math.Floor(Math.Abs((expected - result).TotalMilliseconds)).Equals(0));
        }

        [TestMethod]
        public void GetNextScheduleFromQuartzExpressionThatIsNotWithinThisMinuteSucceeds1()
        {
            var withinThisMinute = new DateTimeOffset(2016, 11, 14, 8, 17, 35, 123, DateTimeOffset.Now.Offset);
            
            var job = new ScheduledJob()
            {
                Crontab = "0 0 0 ? 1/1 5#3"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.GetNextScheduleFromQuartzExpression(withinThisMinute);

            // Assert
            Assert.AreEqual(DateTimeOffset.MinValue, result);
        }

        [TestMethod]
        public void GetNextScheduleFromQuartzExpressionThatIsWithinThisMinuteSucceeds1()
        {
            var withinThisMinute = new DateTimeOffset(2016, 11, 16, 23, 59, 0, 0, DateTimeOffset.Now.Offset);
            var expected = new DateTimeOffset(2016, 11, 17, 0, 0, 0, 0, withinThisMinute.Offset);
            
            var job = new ScheduledJob()
            {
                Crontab = "0 0 0 ? 1/1 5#3"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.GetNextScheduleFromQuartzExpression(withinThisMinute);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetNextScheduleFromQuartzExpressionThatIsWithinThisMinuteSucceeds2()
        {
            var withinThisMinute = new DateTimeOffset(2016, 11, 16, 23, 59, 13, 123, DateTimeOffset.Now.Offset);
            var expected = new DateTimeOffset(2016, 11, 17, 0, 0, 0, 0, withinThisMinute.Offset);
            
            var job = new ScheduledJob()
            {
                Crontab = "0 0 0 ? 1/1 5#3"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.GetNextScheduleFromQuartzExpression(withinThisMinute);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetNextScheduleFromQuartzExpressionThatIsWithinThisMinuteSucceeds3()
        {
            var withinThisMinute = new DateTimeOffset(2016, 11, 17, 0, 0, 0, 0, DateTimeOffset.Now.Offset);
            var expected = new DateTimeOffset(2016, 11, 17, 0, 0, 0, 0, withinThisMinute.Offset);
            
            var job = new ScheduledJob()
            {
                Crontab = "0 0 0 ? 1/1 5#3"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.GetNextScheduleFromQuartzExpression(withinThisMinute);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void GetNextScheduleFromQuartzExpressionThatIsWithinThisMinuteSucceeds4()
        {
            var withinThisMinute = new DateTimeOffset(2016, 11, 14, 14, 6, 0, 0, DateTimeOffset.Now.Offset);
            var expected = new DateTimeOffset(2016, 11, 14, 14, 7, 0, 0, withinThisMinute.Offset);
            
            var job = new ScheduledJob()
            {
                Crontab = "0 07 14 1/1 * ? *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            var result = sut.GetNextScheduleFromQuartzExpression(withinThisMinute);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void IsScheduledToRunSucceeds1()
        {
            var now = new DateTimeOffset(2016, 11, 14, 14, 6, 0, 0, DateTimeOffset.Now.Offset);
            now = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, 1, now.Offset);

            var job = new ScheduledJob()
            {
                Crontab = "0 07 14 1/1 * ? *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            Assert.IsFalse(sut.IsCrontabExpression());
            Assert.IsTrue(sut.IsQuartzExpression());

            var result = sut.IsScheduledToRun(now);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void IsScheduledToRunSucceeds2()
        {
            var now = new DateTimeOffset(2016, 11, 14, 14, 7, 0, 0, DateTimeOffset.Now.Offset);
            now = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, 1, now.Offset);

            var job = new ScheduledJob()
            {
                Crontab = "0 07 14 1/1 * ? *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            Assert.IsFalse(sut.IsCrontabExpression());
            Assert.IsTrue(sut.IsQuartzExpression());

            var result = sut.IsScheduledToRun(now);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsScheduledToRunSucceeds3()
        {
            var now = new DateTimeOffset(2016, 11, 15, 10, 2, 0, 0, DateTimeOffset.Now.Offset);
            now = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, 1, now.Offset);

            var job = new ScheduledJob()
            {
                Crontab = "0 30 19 1/1 * ? *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            Assert.IsFalse(sut.IsCrontabExpression());
            Assert.IsTrue(sut.IsQuartzExpression());

            var result = sut.IsScheduledToRun(now);

            // Assert
            Assert.IsFalse(result);
        }
    
        [TestMethod]
        public void IsScheduledToRunSucceeds4()
        {
            var now = new DateTimeOffset(2016, 11, 15, 19, 30, 0, 0, DateTimeOffset.Now.Offset);
            now = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, 1, now.Offset);

            var job = new ScheduledJob()
            {
                Crontab = "0 30 19 1/1 * ? *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            Assert.IsFalse(sut.IsCrontabExpression());
            Assert.IsTrue(sut.IsQuartzExpression());

            var result = sut.IsScheduledToRun(now);

            // Assert
            Assert.IsTrue(result);
        }
    
        [TestMethod]
        public void IsScheduledToRunSucceeds5()
        {
            var now = new DateTimeOffset(2016, 11, 15, 19, 29, 59, 0, DateTimeOffset.Now.Offset);
            now = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, 1, now.Offset);

            var job = new ScheduledJob()
            {
                Crontab = "0 30 19 1/1 * ? *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            Assert.IsFalse(sut.IsCrontabExpression());
            Assert.IsTrue(sut.IsQuartzExpression());

            var result = sut.IsScheduledToRun(now);

            // Assert
            Assert.IsFalse(result);
        }
    
        [TestMethod]
        public void IsScheduledToRunSucceeds6()
        {
            var now = new DateTimeOffset(2016, 11, 15, 19, 29, 59, 999, DateTimeOffset.Now.Offset);
            now = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, 1, now.Offset);

            var job = new ScheduledJob()
            {
                Crontab = "0 30 19 1/1 * ? *"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            Assert.IsFalse(sut.IsCrontabExpression());
            Assert.IsTrue(sut.IsQuartzExpression());

            var result = sut.IsScheduledToRun(now);

            // Assert
            Assert.IsFalse(result);
        }
    
        [TestMethod]
        public void IsScheduledToRunSucceeds7()
        {
            var now = new DateTimeOffset(2016, 11, 15, 19, 29, 59, 999, DateTimeOffset.Now.Offset);
            now = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0, 1, now.Offset);

            var job = new ScheduledJob()
            {
                Crontab = "0 0 0 12 1 ? 2017"
            };
            var sut = new ScheduledJobScheduler(job);

            // Act
            Assert.IsFalse(sut.IsCrontabExpression());
            Assert.IsTrue(sut.IsQuartzExpression());

            var result = sut.IsScheduledToRun(now);

            // Assert
            Assert.IsFalse(result);
        }
        
    }
}
