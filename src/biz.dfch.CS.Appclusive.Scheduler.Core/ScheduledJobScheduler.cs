/**
 * Copyright 2015 d-fens GmbH
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
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Utilities.Logging;
using NCrontab;
using System;
using System.ComponentModel.DataAnnotations;
// Install-Package ncrontab 
// https://www.nuget.org/packages/ncrontab/
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class ScheduledJobScheduler : BaseDto
    {
        [Required]
        private readonly ScheduledJob job;

        public ScheduledJobScheduler(ScheduledJob scheduledJob)
        {
            Contract.Requires(null != scheduledJob);

            job = scheduledJob;
        }

        public DateTimeOffset GetNextSchedule()
        {
            return GetNextSchedule(DateTimeOffset.Now);
        }

        public DateTimeOffset GetNextSchedule(DateTimeOffset withinThisMinute)
        {
            Contract.Requires(null != withinThisMinute);
            Contract.Ensures(null != Contract.Result<DateTimeOffset>());

            // we only add a millisecond as we get exceptions on certain Windows versions 
            // while converting DateTime to DateTimeOffset
            // see https://support.microsoft.com/en-us/kb/2346777 for details

            if(0 == withinThisMinute.Millisecond)
            {
                withinThisMinute = withinThisMinute.AddMilliseconds(1);
            }

            //Test Name:	IsScheduledToRunOnTheMinuteReturnsTrue
            //Test Outcome:	Failed
            //Result Message:	
            //Test method biz.dfch.CS.Appclusive.Scheduler.Core.Tests.ScheduledJobSchedulerTest.IsScheduledToRunOnTheMinuteReturnsTrue threw exception: 
            //System.ArgumentOutOfRangeException: The UTC time represented when the offset is applied must be between year 0 and 10,000.
            //Parameter name: offset
            //Result StandardOutput:	
            //2019-04-13 20:00:00,000|ERROR|ArgumentOutOfRangeException@mscorlib: 'The UTC time represented when the offset is applied must be between year 0 and 10,000.
            //Parameter name: offset'
            //[The UTC time represented when the offset is applied must be between year 0 and 10,000.
            //Parameter name: offset]
            //   at System.DateTimeOffset.ValidateDate(DateTime dateTime, TimeSpan offset)
            //   at System.DateTimeOffset..ctor(DateTime dateTime, TimeSpan offset)
            //   at biz.dfch.CS.Appclusive.Scheduler.Core.ScheduledJobScheduler.GetNextSchedule(DateTimeOffset withinThisMinute) 
            // in c:\Github\biz.dfch.CS.Appclusive.Scheduler\src\biz.dfch.CS.Appclusive.Scheduler.Core\ScheduledJobScheduler.cs:line 86

            var nextOccurrence = DateTime.MinValue;
            try
            {
                var schedule = CrontabSchedule.Parse(job.Crontab);

                var endMinute = withinThisMinute
                    .DateTime;
                
                // we set the start minute to millisecond 0, 
                // so we have an interval of at least 1 milliscond
                var startMinute =
                    new DateTime
                    (
                        endMinute.Year, 
                        endMinute.Month, 
                        endMinute.Day, 
                        endMinute.Hour, 
                        endMinute.Minute, 
                        0
                    )
                    .AddMinutes(-1);
                Contract.Assert(startMinute < endMinute);

                nextOccurrence = schedule.GetNextOccurrences(startMinute, endMinute).LastOrDefault();
                if 
                (
                    null == nextOccurrence
                    ||
                    DateTime.MinValue == nextOccurrence
                    ||
                    nextOccurrence.Minute < withinThisMinute.Minute
                )
                {
                    return DateTimeOffset.MinValue;
                }
                
                var result = new DateTimeOffset(nextOccurrence, withinThisMinute.Offset);
                return result;
            }
            catch (CrontabException ex)
            {
                Trace.WriteException(ex.Message, ex);

                return DateTimeOffset.MinValue;
            }
            catch (Exception ex)
            {
                Trace.WriteException(ex.Message, ex);

                throw;
            }
        }

        [Pure]
        public bool IsScheduledToRun()
        {
            return IsScheduledToRun(DateTimeOffset.Now);
        }

        [Pure]
        public bool IsScheduledToRun(DateTimeOffset withinThisMinute)
        {
            Contract.Requires(null != withinThisMinute);

            var fReturn = false;

            var nextSchedule = GetNextSchedule(withinThisMinute);
            if(!nextSchedule.Equals(DateTimeOffset.MinValue))
            {
                fReturn = true;
            }
            
            return fReturn;
        }
    }
}
