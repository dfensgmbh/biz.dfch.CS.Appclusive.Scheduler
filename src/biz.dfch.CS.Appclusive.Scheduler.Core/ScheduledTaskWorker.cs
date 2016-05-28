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
        //private const int UPDATE_INTERVAL_IN_MINUTES_DEFAULT = 5;
        //private int updateIntervalInMinutes;

        //private const int SERVER_NOT_REACHABLE_RETRIES_DEFAULT = 3;
        //private int serverNotReachableRetriesInMinutes;
        
        //private readonly string managementUri = Environment.MachineName;
        //private Uri uri;
        
        bool isInitialised = false;
        DateTimeOffset lastInitialisedDate;
        private DateTimeOffset lastUpdated = DateTimeOffset.Now;
        
        private Timer stateTimer = null;
        private TimerCallback timerCallback;

        private readonly List<ScheduledTask> scheduledTasks = new List<ScheduledTask>();

        private biz.dfch.CS.Appclusive.Api.Diagnostics.Diagnostics svcDiagnostics;
        private biz.dfch.CS.Appclusive.Api.Core.Core svcCore;

        private ScheduledTaskWorkerConfiguration configuration;

        public bool IsActive { get; set; }

        public ScheduledTaskWorker(ScheduledTaskWorkerConfiguration configuration)
        {
            Trace.WriteLine(Method.fn());

            this.Ctor(configuration);
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
                mgmtUri = svcCore.ManagementUris
                    .Where
                    (
                        e => e.Name.Equals(configuration.ManagementUriName, StringComparison.OrdinalIgnoreCase) &&
                        e.Type.Equals("biz.dfch.CS.Appclusive.Scheduler", StringComparison.OrdinalIgnoreCase)
                    )
                    .SingleOrDefault();

                if(null == mgmtUri)
                {
                    Trace.WriteLine("{0}: ManagementUri not found at '{1}'. Will retry later.", configuration.ManagementUriName, svcDiagnostics.BaseUri);
                    if (configuration.ServerNotReachableRetries <= (now - lastUpdated).TotalMinutes)
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
                            Debug.WriteLine(string.Format("{0}: Adding '{1}' ...", configuration.ManagementUriName, mgmtUri.Value));
                            scheduledTasks.Add(ExtractTask(j));
                        }
                    }
                    else if (jtoken is JObject)
                    {
                        Debug.WriteLine(string.Format("{0}: Adding '{1}' ...", configuration.ManagementUriName, mgmtUri.Value));
                        scheduledTasks.Add(ExtractTask(jtoken));
                    }
                }
            }
            catch(InvalidOperationException ex)
            {
                Trace.WriteException(ex.Message, ex);
                Debug.WriteLine("{0}: ManagementUri not found at '{1}'. Aborting ...", configuration.ManagementUriName, svcDiagnostics.BaseUri.AbsoluteUri);
                throw;
            }
            catch (TimeoutException ex)
            {
                Trace.WriteException(ex.Message, ex);
                Debug.WriteLine(string.Format("{0}: Timeout retrieving ManagementUri at '{1}'. Aborting ...", configuration.ManagementUriName, svcDiagnostics.BaseUri.AbsoluteUri));
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

        protected void Ctor(ScheduledTaskWorkerConfiguration configuration)
        {
            Contract.Requires(configuration.IsValid());

            Trace.WriteLine(Method.fn());

            var fReturn = false;
            if (isInitialised)
            {
                return;
            }

            try
            {
                Debug.WriteLine(string.Format("Uri: '{0}'", configuration.Uri.AbsoluteUri));

                svcDiagnostics = new Diagnostics(new Uri(string.Format("{0}api/Diagnostics", configuration.Uri.AbsoluteUri)));
                svcDiagnostics.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                svcDiagnostics.Format.UseJson();

                svcCore = new biz.dfch.CS.Appclusive.Api.Core.Core(new Uri(string.Format("{0}api/Core", configuration.Uri.AbsoluteUri)));
                svcCore.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                svcCore.Format.UseJson();

                UpdateScheduledTasks();

                timerCallback = new TimerCallback(this.RunTasks);
                stateTimer = new Timer(timerCallback, null, 1000, (1000 * 60) - 20);
                
                fReturn = true;
            }
            catch (Exception ex)
            {
                Trace.WriteException(ex.Message, ex);
            }

            this.isInitialised = fReturn;
            this.IsActive = fReturn;
            return;
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

                            biz.dfch.CS.Utilities.Process.StartProcess(task.Parameters.CommandLine, task.Parameters.WorkingDirectory, task.Credential);
                        }
                    }
                }

                if (configuration.UpdateIntervalInMinutes <= (now - lastInitialisedDate).TotalMinutes)
                {
                    fReturn = UpdateScheduledTasks();
                }
            }
            // why do we handle a timeout exception here ?!
            catch (TimeoutException ex)
            {
                Trace.WriteException(ex.Message, ex);
                var msg = string.Format("{0}: Timeout retrieving ManagementUri at '{1}'. Aborting ...", configuration.ManagementUriName, svcDiagnostics.BaseUri.AbsoluteUri);
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
