/**
 * Copyright 2011-2016 d-fens GmbH
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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
// Install-Package Microsoft.Net.Http
// https://www.nuget.org/packages/Microsoft.Net.Http
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using biz.dfch.CS.Utilities.General;
using biz.dfch.CS.Utilities.Logging;
using biz.dfch.CS.Appclusive.Api.Core;
using biz.dfch.CS.Appclusive.Api.Diagnostics;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    class ScheduledTaskWorker
    {
        private const int UPDATE_INTERVAL_IN_MINUTES_DEFAULT = 5;
        private int updateIntervalInMinutes;

        private const int SERVER_NOT_REACHABLE_RETRIES_DEFAULT = 3;
        private int serverNotReachableRetriesInMinutes;
        
        bool isInitialised = false;
        DateTimeOffset lastInitialisedDate;
        private DateTimeOffset lastUpdated = DateTimeOffset.Now;
        
        private Timer stateTimer = null;
        private TimerCallback timerCallback;

        private readonly List<ScheduledTask> scheduledTasks = new List<ScheduledTask>();
        private Uri uri;

        private biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics svcDiagnostics;
        private biz.dfch.CS.Appclusive.Api.Core.Core svcCore;
        
        private readonly string managementUri = Environment.MachineName;

        public bool IsActive { get; set; }

        public ScheduledTaskWorker(string uri, string managementUri, int updateIntervalMinutes, int serverNotReachableRetries)
        {
            Trace.WriteLine(Method.fn());

            this.Initialise(uri, managementUri, updateIntervalMinutes, serverNotReachableRetries, true);
        }

        public bool UpdateScheduledTasks()
        {
            Trace.WriteLine(Method.fn());

            var fReturn = false;

            if (!IsActive)
            {
                return fReturn;
            }

            ManagementUri mgmtUri = null;
            try
            {
                var now = DateTimeOffset.Now;
                // DFTODO - read from App.config
                mgmtUri = svcCore.ManagementUris
                    .Where
                    (
                        e => e.Name.Equals(managementUri, StringComparison.OrdinalIgnoreCase) &&
                        e.Type.Equals("biz.dfch.CS.Appclusive.Scheduler", StringComparison.OrdinalIgnoreCase)
                    )
                    .SingleOrDefault();

                if(null == mgmtUri)
                {
                    Debug.WriteLine("{0}: ManagementUri not found at '{1}'. Will retry later.", managementUri, svcDiagnostics.BaseUri);
                    if (serverNotReachableRetriesInMinutes <= (now - lastUpdated).TotalMinutes)
                    {
                        throw new TimeoutException();
                    }
                    goto Success;
                }

                var jtoken = JToken.Parse(mgmtUri.Value);
                lock (scheduledTasks)
                {
                    scheduledTasks.Clear();
                    if (jtoken is JArray)
                    {
                        var ja = JArray.Parse(mgmtUri.Value);
                        foreach (var j in ja)
                        {
                            Debug.WriteLine(string.Format("{0}: Adding '{1}' ...", managementUri, mgmtUri.Value));
                            scheduledTasks.Add(ExtractTask(j));
                        }
                    }
                    else if (jtoken is JObject)
                    {
                        Debug.WriteLine(string.Format("{0}: Adding '{1}' ...", managementUri, mgmtUri.Value));
                        scheduledTasks.Add(ExtractTask(jtoken));
                    }
                }
            }
            catch(InvalidOperationException ex)
            {
                Trace.WriteException(ex.Message, ex);
                Debug.WriteLine("{0}: ManagementUri not found at '{1}'. Aborting ...", managementUri, svcDiagnostics.BaseUri.AbsoluteUri);
                throw;
            }
            catch (TimeoutException ex)
            {
                Trace.WriteException(ex.Message, ex);
                Debug.WriteLine(string.Format("{0}: Timeout retrieving ManagementUri at '{1}'. Aborting ...", managementUri, svcDiagnostics.BaseUri.AbsoluteUri));
                throw;
            }
            finally
            {
                if(null != mgmtUri)
                {
                    svcDiagnostics.Detach(mgmtUri);
                }
            }
Success :
            lastInitialisedDate = DateTimeOffset.Now;
            fReturn = true;
            return fReturn;
        }

        private ScheduledTask ExtractTask(JToken taskParameters)
        {
            Contract.Requires(null != taskParameters);

            var task = new ScheduledTask(taskParameters.ToString());
            var mgmtCredential = svcCore.ManagementCredentials
                .Where
                (
                    e => e.Name.Equals(task.Parameters.ManagementCredential, StringComparison.OrdinalIgnoreCase)
                )
                .Single();

            task.Username = mgmtCredential.Username;
            task.Password = mgmtCredential.Password;

            svcCore.Detach(mgmtCredential);
            return task;
        }

        protected bool Initialise(string uri, string managementUri, int updateIntervalMinutes, int serverNotReachableRetries, bool fThrowException)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(uri));
            Contract.Requires(!string.IsNullOrWhiteSpace(managementUri));
            Contract.Requires(0 <= updateIntervalMinutes);
            Contract.Requires(0 <= serverNotReachableRetries);

            Trace.WriteLine(Method.fn());

            var fReturn = false;
            if (isInitialised)
            {
                return fReturn;
            }

            try
            {
                updateIntervalInMinutes = (0 != updateIntervalMinutes) ? updateIntervalMinutes : UPDATE_INTERVAL_IN_MINUTES_DEFAULT;
                serverNotReachableRetriesInMinutes = updateIntervalInMinutes * (0 != serverNotReachableRetries ? serverNotReachableRetries : SERVER_NOT_REACHABLE_RETRIES_DEFAULT);

                this.uri = new Uri(uri);
                Debug.WriteLine(string.Format("Uri: '{0}'", this.uri.AbsoluteUri));
                managementUri = managementUri;

                svcDiagnostics = new Diagnostics(new Uri(string.Format("{0}api/Diagnostics", this.uri.AbsoluteUri)));
                svcDiagnostics.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                svcDiagnostics.Format.UseJson();

                svcCore = new biz.dfch.CS.Appclusive.Api.Core.Core(new Uri(string.Format("{0}api/Core", this.uri.AbsoluteUri)));
                svcCore.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                svcCore.Format.UseJson();

                UpdateScheduledTasks();

                timerCallback = new TimerCallback(this.RunTasks);
                //var MilliSecondsToWait = (60 - DateTimeOffset.now.Second) * 1000;
                //Debug.WriteLine(string.Format("Waiting {0}ms for begin of next minute ...", MilliSecondsToWait));
                //System.Threading.Thread.Sleep(MilliSecondsToWait);
                stateTimer = new Timer(timerCallback, null, 1000, (1000 * 60) - 20);
                
                fReturn = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}@{1}: {2}\r\n{3}", ex.GetType().Name, ex.Source, ex.Message, ex.StackTrace));
                if (fThrowException)
                {
                    throw;
                }
                else
                {
                    fReturn = false;
                }
            }

            this.isInitialised = fReturn;
            this.IsActive = fReturn;
            return fReturn;
        }

        ~ScheduledTaskWorker()
        {
            Trace.WriteLine(Method.fn());

            if (null == this.stateTimer)
            {
                return;
            }
            
            stateTimer.Dispose();
        }

        // The state object is necessary for a TimerCallback.
        protected void RunTasks(object stateObject)
        {
            var fReturn = false;
            var now = DateTimeOffset.Now;

            if (!IsActive || !isInitialised)
            {
                return;
            }

            Trace.WriteLine(Method.fn());
            try
            {
                lock (scheduledTasks)
                {
                    foreach (var task in scheduledTasks)
                    {
                        if (task.IsScheduledToRun(now))
                        {
                            Debug.WriteLine(string.Format("Invoking '{0}' as '{1}' [{2}] ...", task.Parameters.CommandLine, task.Username, task.NextOccurrence.ToString()));

                            // DFTODO - check if this call is blocking
                            biz.dfch.CS.Utilities.Process.StartProcess(task.Parameters.CommandLine, task.Parameters.WorkingDirectory, task.Credential);
                        }
                    }
                }

                if (updateIntervalInMinutes <= (now - lastInitialisedDate).TotalMinutes)
                {
                    fReturn = UpdateScheduledTasks();
                }
            }
            catch (TimeoutException ex)
            {
                Trace.WriteException(ex.Message, ex);
                var msg = string.Format("{0}: Timeout retrieving ManagementUri at '{1}'. Aborting ...", managementUri, svcDiagnostics.BaseUri.AbsoluteUri);
                Trace.WriteLine(msg);
                Environment.FailFast(msg);
                throw;
            }
            catch (Exception ex)
            {
                Trace.WriteException(ex.Message, ex);
                throw;
            }
        }
    }
}
