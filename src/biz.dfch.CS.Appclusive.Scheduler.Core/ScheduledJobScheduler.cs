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

        public bool IsActive { get; set; }
        
        [Required]
        public DateTime NextOccurrence { get; set; }

        public ScheduledJobScheduler(ScheduledJob scheduledJob)
        {
            Contract.Requires(null != scheduledJob);

            IsActive = true;
            NextOccurrence = DateTime.MinValue;
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

            var nextOccurrence = DateTime.MinValue;
            try
            {
                if (!this.IsActive)
                {
                    return DateTimeOffset.MinValue;
                }

                var schedule = CrontabSchedule.Parse(job.Crontab);
                
                var startMinute = new DateTime(withinThisMinute.Year, 1, 1, 0, 0, 0);
                nextOccurrence = schedule.GetNextOccurrences(startMinute, withinThisMinute.DateTime).LastOrDefault();
                if (null == nextOccurrence)
                {
                    return DateTimeOffset.MinValue;
                }
                
                if(nextOccurrence.Minute < withinThisMinute.Minute)
                {
                    return DateTimeOffset.MinValue;
                }

                NextOccurrence = nextOccurrence;
                return NextOccurrence;
            }
            catch (CrontabException ex)
            {
                Trace.WriteLine(ex.Message, ex);

                return DateTimeOffset.MinValue;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message, ex);

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
