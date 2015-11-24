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

namespace biz.dfch.CS.Appclusive.Scheduler
{
    class ScheduledTask
    {
        private bool _fInitialised = false;
        private int _VersionDefault = 0;

        public ScheduledTaskParameters Parameters;
        public DateTime NextSchedule = DateTime.MinValue;
        public string Username
        {
            get { return _Credential.UserName; }
            set 
            {
                if (value.Contains("\\"))
                {
                    var DomainUsername = value.Split('\\');
                    _Credential.Domain = DomainUsername.First();
                    _Credential.UserName = DomainUsername.Last();
                }
                else
                {
                    _Credential.UserName = value;
                }
            }
        }
        public string Password
        {
            get { return _Credential.Password; }
            set { _Credential.Password = value; }
        }
        public string Domain
        {
            get { return _Credential.Domain; }
            set { _Credential.Domain = value; }
        }
        private NetworkCredential _Credential = new NetworkCredential();
        public NetworkCredential Credential
        {
            get { return _Credential; }
            set { _Credential = value; }
        }

        public ScheduledTask()
        {
            return;
        }

        public ScheduledTask(string parameters)
        {
            this.Initialise(parameters, this._VersionDefault, true);
        }

        public ScheduledTask(string parameters, int version)
        {
            this.Initialise(parameters, version, true);
        }

        public bool Initialise(string parameters, bool fThrowException = false)
        {
            return this.Initialise(parameters, this._VersionDefault, fThrowException);
        }

        public bool Initialise(string parameters, int version, bool fThrowException = false)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(parameters));
            Contract.Ensures(Contract.OldValue(parameters) == Contract.ValueAtReturn(out parameters));
            Contract.Ensures(Contract.OldValue(version) == Contract.ValueAtReturn(out version));
            Contract.Ensures(Contract.OldValue(fThrowException) == Contract.ValueAtReturn(out fThrowException));

            var fReturn = false;
            if (_fInitialised) return fReturn;

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
            finally
            {

            }
            this._fInitialised = fReturn;
            return fReturn;
        }

        public DateTime GetNextSchedule()
        {
            return GetNextSchedule(DateTime.Now);
        }

        public DateTime GetNextSchedule(DateTime withinThisMinute)
        {
            Contract.Requires(null != withinThisMinute);
            Contract.Ensures(Contract.OldValue(withinThisMinute) == Contract.ValueAtReturn(out withinThisMinute));

            var nextSchedule = DateTime.MinValue;
            try
            {
                if (!this.Parameters.Active)
                {
                    return DateTime.MinValue;
                }
                var schedule = CrontabSchedule.Parse(this.Parameters.CrontabExpression);
                var now = withinThisMinute;
                var startMinute = new DateTime(now.Year, 1, 1, 0, 0, 0);
                //var endMinute = now.AddMinutes(1).AddSeconds(-1);
                var endMinute = now;
                nextSchedule = schedule.GetNextOccurrences(startMinute, endMinute).LastOrDefault();
                if (null == nextSchedule)
                {
                    Debug.WriteLine(string.Format(
                        "{0}: Getting next occurrence for time range '{1}-{2}' [{3}] FAILED. Check CrontabExpression or time range.", 
                        this.Parameters.CommandLine, 
                        startMinute.ToString("yyyy-MM-dd HH:mm:ss.fffzzz"), 
                        endMinute.ToString("yyyy-MM-dd HH:mm:ss.fffzzz"), 
                        this.Parameters.CrontabExpression
                        ));
                    return DateTime.MinValue;
                }
                if(nextSchedule.Minute < now.Minute)
                {
                    return DateTime.MinValue;
                }
                this.NextSchedule = nextSchedule;
                return this.NextSchedule;
            }
            catch (CrontabException ex)
            {
                Debug.WriteLine(string.Format("{0}@{1}: {2}\r\n{3}", ex.GetType().Name, ex.Source, ex.Message, ex.StackTrace));
                return DateTime.MinValue;
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
            return IsScheduledToRun(DateTime.Now);
        }

        [Pure]
        public bool IsScheduledToRun(DateTime withinThisMinute)
        {
            Contract.Requires(null != withinThisMinute);
            Contract.Ensures(Contract.OldValue(withinThisMinute) == Contract.ValueAtReturn(out withinThisMinute));

            var fReturn = false;
            var nextSchedule = GetNextSchedule(withinThisMinute);
            if(!nextSchedule.Equals(DateTime.MinValue))
            {
                fReturn = true;
            }
            return fReturn;
        }
    }
}
