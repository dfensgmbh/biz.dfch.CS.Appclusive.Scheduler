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
        
        private bool isInitialised = false;
        private DateTimeOffset lastInitialisedDate;
        private DateTimeOffset lastUpdated = DateTimeOffset.Now;
        
        private Timer stateTimer = null;
        private TimerCallback timerCallback;

        private readonly List<ScheduledTask> scheduledTasks = new List<ScheduledTask>();

        private AppclusiveEndpoints endpoints;

        private ScheduledTaskWorkerConfiguration configuration;

        public bool IsActive { get; set; }

        public ScheduledTaskWorker(ScheduledTaskWorkerConfiguration configuration)
        {
            Trace.WriteLine(Method.fn());

            this.Ctor(configuration);
        }

        public bool InitialUpdateScheduledTasks()
        {
            Trace.WriteLine(Method.fn());

            var result = false;

            result = InternalUpdateScheduledTask();
            return result;
        }

        public bool UpdateScheduledTasks()
        {
            Trace.WriteLine(Method.fn());

            var result = false;

            if (!IsActive)
            {
                return result;
            }

            result = InternalUpdateScheduledTask();
            return result;
        }

        public bool InternalUpdateScheduledTask()
        {
            Trace.WriteLine(Method.fn());

            var result = false;

            ManagementUri mgmtUri = null;
            try
            {
                var now = DateTimeOffset.Now;
                mgmtUri = endpoints.Core.ManagementUris
                    .Where
                    (
                        e => e.Name.Equals(configuration.ManagementUriName, StringComparison.InvariantCultureIgnoreCase) &&
                        e.Type.Equals("biz.dfch.CS.Appclusive.Scheduler", StringComparison.InvariantCultureIgnoreCase)
                    )
                    .SingleOrDefault();

                if(null == mgmtUri)
                {
                    Trace.WriteLine("{0}: ManagementUri not found at '{1}'. Will retry later.", configuration.ManagementUriName, endpoints.Core.BaseUri);
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
                            try
                            {
                                Debug.WriteLine(string.Format("{0}: Adding '{1}' ...", configuration.ManagementUriName, mgmtUri.Value));
                                scheduledTasks.Add(ExtractTask(j));
                            }
                            catch (Exception ex)
                            {
                                Trace.WriteException(string.Format("{0}: Adding '{1}' FAILED. Skipping.", configuration.ManagementUriName, mgmtUri.Value), ex);
                            }
                        }
                    }
                    else if (jtoken is JObject)
                    {
                        try
                        {
                            Debug.WriteLine(string.Format("{0}: Adding '{1}' ...", configuration.ManagementUriName, mgmtUri.Value));
                            scheduledTasks.Add(ExtractTask(jtoken));
                        }
                        catch(Exception ex)
                        {
                            Trace.WriteException(string.Format("{0}: Adding '{1}' FAILED. Skipping.", configuration.ManagementUriName, mgmtUri.Value), ex);
                        }
                    }
                }
            }
            catch(InvalidOperationException ex)
            {
                Trace.WriteException(ex.Message, ex);
                Debug.WriteLine("{0}: ManagementUri not found at '{1}'. Aborting ...", configuration.ManagementUriName, endpoints.Core.BaseUri.AbsoluteUri);
                throw;
            }
            catch (TimeoutException ex)
            {
                Trace.WriteException(ex.Message, ex);
                Debug.WriteLine(string.Format("{0}: Timeout retrieving ManagementUri at '{1}'. Aborting ...", configuration.ManagementUriName, endpoints.Core.BaseUri.AbsoluteUri));
                throw;
            }
            finally
            {
                if(null != mgmtUri)
                {
                    endpoints.Diagnostics.Detach(mgmtUri);
                }
            }
Success :
            lastInitialisedDate = DateTimeOffset.Now;
            result = true;
            return result;
        }

        private ScheduledTask ExtractTask(JToken taskParameters)
        {
            Contract.Requires(null != taskParameters);

            var task = new ScheduledTask(taskParameters.ToString());
            Contract.Assert(null != task);
            Contract.Assert(!string.IsNullOrWhiteSpace(task.Parameters.ManagementCredential));

            var mgmtCredential = endpoints.Core.ManagementCredentials
                .Where
                (
                    e => e.Name.Equals(task.Parameters.ManagementCredential, StringComparison.InvariantCultureIgnoreCase)
                )
                .Single();

            task.Username = mgmtCredential.Username;
            task.Password = mgmtCredential.Password;

            endpoints.Core.Detach(mgmtCredential);
            return task;
        }

        protected void Ctor(ScheduledTaskWorkerConfiguration configuration)
        {
            Contract.Requires(configuration.IsValid());
            this.configuration = configuration;

            Trace.WriteLine(Method.fn());

            var result = false;
            if (isInitialised)
            {
                return;
            }

            try
            {
                Debug.WriteLine(string.Format("Uri: '{0}'", configuration.Uri.AbsoluteUri));

                var baseUri = new Uri(string.Format("{0}api", configuration.Uri.AbsoluteUri));
                endpoints = new AppclusiveEndpoints(baseUri, configuration.Credential);

                result = InitialUpdateScheduledTasks();

                timerCallback = new TimerCallback(this.RunTasks);
                stateTimer = new Timer(timerCallback, null, 1000, (1000 * 60) - 20);
            }
            catch (Exception ex)
            {
                Trace.WriteException(ex.Message, ex);
                throw;
            }

            this.isInitialised = result;
            this.IsActive = result;
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
            Contract.Assert(isInitialised);

            var fn = Method.fn();

            var result = false;
            var now = DateTimeOffset.Now;

            if (!IsActive)
            {
                Debug.WriteLine("{0}: IsActive: {1}. Nothing to do.", fn, IsActive);
                return;
            }

            Trace.WriteLine(Method.fn());
            try
            {
                lock (scheduledTasks)
                {
                    if (0 >= scheduledTasks.Count)
                    {
                        Debug.WriteLine("{0}: No scheduled tasks found. Nothing to do.", fn, "");
                    }

                    foreach (var task in scheduledTasks)
                    {
                        if (task.IsScheduledToRun(now))
                        {
                            Debug.WriteLine(string.Format("Invoking '{0}' as '{1}' [{2}] ...", task.Parameters.CommandLine, task.Username, task.NextOccurrence.ToString()));

                            var resultFromStartProcess = biz.dfch.CS.Utilities.Process.StartProcess(task.Parameters.CommandLine, task.Parameters.WorkingDirectory, task.Credential);
                            // DFTODO - we could potentionally log the result of StartProcess to a log file
                        }
                    }
                }

                if (configuration.UpdateIntervalInMinutes <= (now - lastInitialisedDate).TotalMinutes)
                {
                    result = UpdateScheduledTasks();
                    Contract.Assert(result);
                }
            }
            // DFTODO - why do we handle a timeout exception here ?!
            catch (TimeoutException ex)
            {
                Trace.WriteException(ex.Message, ex);
                var msg = string.Format("{0}: Timeout retrieving ManagementUri at '{1}'. Aborting ...", configuration.ManagementUriName, endpoints.Diagnostics.BaseUri.AbsoluteUri);
                Trace.WriteLine(msg);
                Environment.FailFast(msg);
                throw;
            }
            catch (Exception ex)
            {
                Trace.WriteException(ex.Message, ex);
                // DFTODO - should we really throw - this would bring down the whole service
                throw;
            }

            return;
        }
    }
}
