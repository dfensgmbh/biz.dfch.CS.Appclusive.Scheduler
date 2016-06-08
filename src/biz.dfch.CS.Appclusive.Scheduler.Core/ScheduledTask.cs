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
// Install-Package ncrontab 
// https://www.nuget.org/packages/ncrontab/
using System.Diagnostics.Contracts;
using NCrontab;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using biz.dfch.CS.Utilities.Logging;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    class ScheduledTask
    {
        private bool isInitialised = false;
        private const int VERSION_DEFAULT = 0;

        public ScheduledTaskParameters Parameters { get; set; }
        
        private DateTime nextOccurrence = DateTime.MinValue;
        public DateTime NextOccurrence
        {
            get { return nextOccurrence; }
            set { nextOccurrence = value; }
        }

        public string Username
        {
            get 
            { 
                return credential.UserName; 
            }
            set 
            {
                if (value.Contains("\\"))
                {
                    var domainUsername = value.Split('\\');
                    Contract.Assert(null != domainUsername);
                    Contract.Assert(2 == domainUsername.Count());

                    credential.Domain = domainUsername.First();
                    credential.UserName = domainUsername.Last();
                }
                else
                {
                    credential.UserName = value;
                }
            }
        }

        public string Password
        {
            get { return credential.Password; }
            set { credential.Password = value; }
        }

        public string Domain
        {
            get { return credential.Domain; }
            set { credential.Domain = value; }
        }

        private NetworkCredential credential = new NetworkCredential();
        public NetworkCredential Credential
        {
            get { return credential; }
            set { credential = value; }
        }

        public ScheduledTask()
        {
            return;
        }

        public ScheduledTask(string parameters)
        {
            var isValidParameter = this.Initialise(parameters, VERSION_DEFAULT, true);
            Contract.Assert(isValidParameter);
        }

        public ScheduledTask(string parameters, int version)
        {
            var isValidParameter = this.Initialise(parameters, version, true);
            Contract.Assert(isValidParameter);
        }

        public bool Initialise(string parameters, bool fThrowException = false)
        {
            return this.Initialise(parameters, VERSION_DEFAULT, fThrowException);
        }

        public bool Initialise(string parameters, int version, bool fThrowException = false)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(parameters));
            Contract.Requires(VERSION_DEFAULT <= version);

            var fReturn = false;
            if (isInitialised)
            {
                return fReturn;
            }

            try
            {
                this.Parameters = JsonConvert.DeserializeObject<ScheduledTaskParameters>(parameters);
                fReturn = true;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(string.Format("{0}@{1}: {2}\r\n{3}", ex.GetType().Name, ex.Source, ex.Message, ex.StackTrace));
                if(fThrowException)
                {
                    throw;
                } 
                else
                {
                    this.Parameters = null;
                    fReturn = false;
                }
            }

            this.isInitialised = fReturn;
            return fReturn;
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
                if (!this.Parameters.IsActive)
                {
                    return DateTimeOffset.MinValue;
                }

                var schedule = CrontabSchedule.Parse(this.Parameters.CrontabExpression);
                
                var startMinute = new DateTime(withinThisMinute.Year, 1, 1, 0, 0, 0);
                nextOccurrence = schedule.GetNextOccurrences(startMinute, withinThisMinute.DateTime).LastOrDefault();
                if (null == nextOccurrence)
                {
                    Debug.WriteLine(string.Format(
                        "{0}: Getting next occurrence for time range '{1}-{2}' [{3}] FAILED. Check CrontabExpression or time range.", 
                        this.Parameters.CommandLine, 
                        startMinute.ToString("yyyy-MM-dd HH:mm:ss.fffzzz"), 
                        withinThisMinute.ToString("yyyy-MM-dd HH:mm:ss.fffzzz"), 
                        this.Parameters.CrontabExpression
                        ));
                    return DateTimeOffset.MinValue;
                }
                
                if(nextOccurrence.Minute < withinThisMinute.Minute)
                {
                    return DateTimeOffset.MinValue;
                }

                this.nextOccurrence = nextOccurrence;
                return this.nextOccurrence;
            }
            catch (CrontabException ex)
            {
                Debug.WriteLine(string.Format("{0}@{1}: {2}\r\n{3}", ex.GetType().Name, ex.Source, ex.Message, ex.StackTrace));
                return DateTimeOffset.MinValue;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}@{1}: {2}\r\n{3}", ex.GetType().Name, ex.Source, ex.Message, ex.StackTrace));
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
