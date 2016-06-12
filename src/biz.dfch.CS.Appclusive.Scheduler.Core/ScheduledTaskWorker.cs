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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using biz.dfch.CS.Utilities.General;
using biz.dfch.CS.Utilities.Logging;
using biz.dfch.CS.Appclusive.Api.Core;
using biz.dfch.CS.Appclusive.Api.Diagnostics;
using biz.dfch.CS.Appclusive.Public;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class ScheduledTaskWorker
    {
        public const long SCHEDULED_TASK_WORKER_JOBS_PER_INSTANCE_MAX = 10000;

        private readonly bool isInitialised = false;
        private DateTimeOffset lastUpdated = DateTimeOffset.Now;
        private readonly Timer stateTimer = null;

        private readonly ScheduledTaskWorkerConfiguration configuration;
        private readonly AppclusiveEndpoints endpoints;

        private List<ScheduledJob> scheduledJobs = new List<ScheduledJob>();

        public bool IsActive { get; set; }

        public ScheduledTaskWorker(ScheduledTaskWorkerConfiguration configuration)
        {
            Contract.Requires(configuration.IsValid());

            this.configuration = configuration;

            Trace.WriteLine(Method.fn());

            var result = false;

            try
            {
                Trace.WriteLine("Uri: '{0}'", configuration.Uri.AbsoluteUri, "");

                var baseUri = new Uri(string.Format("{0}api", configuration.Uri.AbsoluteUri));
                endpoints = new AppclusiveEndpoints(baseUri, configuration.Credential);

                result = GetScheduledJobs();

                stateTimer = new ScheduledTaskWorkerTimerFactory().CreateTimer(new TimerCallback(this.RunTasks), null, 1000, (1000 * 60) - 20);
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

        public bool GetScheduledJobs()
        {
            var result = false;

            if (isInitialised && !IsActive)
            {
                return result;
            }
            
            // load ScheduledJob entities
            try
            {
                var scheduledJobsManager = new ScheduledJobsManager();

                var scheduledJobs = scheduledJobsManager.LoadJobs();
                var validJobs = scheduledJobsManager.GetValidJobs(scheduledJobs);
                this.scheduledJobs = validJobs;
                Contract.Assert(SCHEDULED_TASK_WORKER_JOBS_PER_INSTANCE_MAX >= validJobs.Count);
            }
            catch(InvalidOperationException ex)
            {
                var message = string.Format("Loading ScheduledJobs from '{0}' FAILED with InvalidOperationException. Check the specified credentials.", endpoints.Core.BaseUri.AbsoluteUri);
                Trace.WriteException(message, ex);
            }
            catch (Exception ex)
            {
                Trace.WriteException(ex.Message, ex);
                
                throw;
            }

            lastUpdated = DateTimeOffset.Now;
            result = true;
            return result;
        }

        //private ScheduledTask ExtractTask(JToken taskParameters)
        //{
        //    Contract.Requires(null != taskParameters);

        //    var task = new ScheduledTask(taskParameters.ToString());
        //    Contract.Assert(null != task);
        //    Contract.Assert(!string.IsNullOrWhiteSpace(task.Parameters.ManagementCredential));

        //    var mgmtCredential = endpoints.Core.ManagementCredentials
        //        .Where
        //        (
        //            e => e.Name.Equals(task.Parameters.ManagementCredential, StringComparison.InvariantCultureIgnoreCase)
        //        )
        //        .Single();

        //    task.Username = mgmtCredential.Username;
        //    task.Password = mgmtCredential.Password;

        //    endpoints.Core.Detach(mgmtCredential);
        //    return task;
        //}

        // The state object is necessary for a TimerCallback.
        public void RunTasks(object stateObject)
        {
            Contract.Assert(isInitialised);

            var fn = Method.fn();
            Trace.WriteLine(Method.fn());

            var result = false;
            var now = DateTimeOffset.Now;

            if (!IsActive)
            {
                Trace.WriteLine("{0}: IsActive: {1}. Nothing to do.", fn, IsActive);
                return;
            }

            lock(scheduledJobs)
            {
                if (0 >= scheduledJobs.Count)
                {
                    Trace.WriteLine("{0}: No scheduled jobs found. Nothing to do.", fn, "");
                }

                var defaultPlugin = configuration.Plugins.FirstOrDefault(p => p.Metadata.Type.Equals("Default"));
                
                foreach (var job in scheduledJobs)
                {
                    try
                    {
                        var task = new ScheduledTask(job);
                        if (!task.IsScheduledToRun(now))
                        {
                            continue;
                        }

                        var plugin = configuration.Plugins.FirstOrDefault(p => p.Metadata.Type.Equals(job.Action));
                        if(null == plugin)
                        {
                            Trace.WriteLine("No plugin for Job.Id '{0}' with Job.Action '{1}' found. Using 'Default' plugin ...", job.Id, job.Action);
                            plugin = defaultPlugin;
                        }
                        if(null == plugin)
                        {
                            Trace.WriteLine("No plugin for Job.Id '{0}' with Job.Action '{1}' found and no 'Default' plugin found either. Skipping.", job.Id, job.Action);
                        }
                        Contract.Assert(null != plugin, "No plugin found to execute scheduled job");

                        var învocationResult = new NonSerialisableJobResult();
                        plugin.Value.Invoke(default(DictionaryParameters), învocationResult);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteException(ex.Message, ex);

                        throw;
                    }
                }
            }

            if (configuration.UpdateIntervalInMinutes <= (now - lastUpdated).TotalMinutes)
            {
                result = GetScheduledJobs();
                Contract.Assert(result);
            }

            return;
        }

        ~ScheduledTaskWorker()
        {
            if(null != stateTimer)
            {
                stateTimer.Dispose();
            }
        }
    }
}
