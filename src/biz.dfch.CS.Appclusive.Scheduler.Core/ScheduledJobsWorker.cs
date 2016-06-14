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

using biz.dfch.CS.Appclusive.Api.Core;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using biz.dfch.CS.Utilities.General;
using biz.dfch.CS.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
// Install-Package Microsoft.Net.Http
// https://www.nuget.org/packages/Microsoft.Net.Http
using System.Threading;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class ScheduledJobsWorker
    {
        public const long SCHEDULED_JOBS_WORKER_JOBS_PER_INSTANCE_MAX = 10000;

        private readonly bool isInitialised = false;
        private DateTimeOffset lastUpdated = DateTimeOffset.Now;
        private readonly Timer stateTimer = null;

        private readonly ScheduledJobsWorkerConfiguration configuration;
        private readonly AppclusiveEndpoints endpoints;

        private List<ScheduledJob> scheduledJobs = new List<ScheduledJob>();

        public bool IsActive { get; set; }

        public ScheduledJobsWorker(ScheduledJobsWorkerConfiguration configuration)
        {
            Contract.Requires(configuration.IsValid());

            this.configuration = configuration;

            Trace.WriteLine(Method.fn());

            var result = false;

            try
            {
                Trace.WriteLine("Uri: '{0}'", configuration.Uri.AbsoluteUri, "");

                // connect to Appclusive server
                var baseUri = new Uri(string.Format("{0}api", configuration.Uri.AbsoluteUri));
                endpoints = new AppclusiveEndpoints(baseUri, configuration.Credential);

                // initialise each plugin
                foreach (var plugin in configuration.Plugins)
                {
                    try
                    {
                        Trace.WriteLine("Initialising plugin '{0}' [{1}, {2}] ...", plugin.Metadata.Type, plugin.Metadata.Role, plugin.Metadata.Priority);

                        var pluginParameters = new DictionaryParameters();
                        pluginParameters.Add(typeof(AppclusiveEndpoints).ToString(), endpoints);
                        plugin.Value.Initialise(pluginParameters, configuration.Logger, true);

                        Trace.WriteLine(
                            "Initialising plugin '{0}' [{1}, {2}] COMPLETED. IsInitialised {3}. IsActive {4}."
                            , 
                            plugin.Metadata.Type, plugin.Metadata.Role, plugin.Metadata.Priority
                            ,
                            plugin.Value.IsInitialised, plugin.Value.IsActive
                            );
                    }
                    catch (Exception ex)
                    {
                        var message = string.Format("Initialising plugin '{0}' [{1}, {2}] FAILED.", plugin.Metadata.Type, plugin.Metadata.Role, plugin.Metadata.Priority);
                        Trace.WriteException(message, ex);
                    }
                }

                // get all defined scheduled jobs for all tenants (based on credentials)
                result = GetScheduledJobs();

                // create the timer to process all scheduled jobs periodically
                stateTimer = new ScheduledJobsWorkerTimerFactory().CreateTimer(new TimerCallback(this.RunJobs), null, 1000, (1000 * 60) - 20);
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
                configuration.Logger.Warn("Scheduler is not initialised [{0}] or not active [{1}]. Skip loading of ScheduledJobs.", isInitialised, IsActive);
                goto Success;
            }
            
            // load ScheduledJob entities
            try
            {
                configuration.Logger.Info("Loading ScheduledJobs from '{0}' ...", endpoints.Core.BaseUri.AbsoluteUri);
                var scheduledJobsManager = new ScheduledJobsManager(endpoints);

                var scheduledJobs = scheduledJobsManager.GetJobs();
                var validJobs = scheduledJobsManager.GetValidJobs(scheduledJobs);
                this.scheduledJobs = validJobs;
                Contract.Assert(SCHEDULED_JOBS_WORKER_JOBS_PER_INSTANCE_MAX >= validJobs.Count);
            
                configuration.Logger.Info("Loading ScheduledJobs from '{0}' SUCCEEDED. [{1}]", endpoints.Core.BaseUri.AbsoluteUri, scheduledJobs.Count);

                result = true;
            }
            catch(InvalidOperationException ex)
            {
                var message = string.Format("Loading ScheduledJobs from '{0}' FAILED with InvalidOperationException. Check the specified credentials.", endpoints.Core.BaseUri.AbsoluteUri);
                Trace.WriteException(message, ex);

                result = false;
            }
            catch (Exception ex)
            {
                Trace.WriteException(ex.Message, ex);
                
                throw;
            }

Success:
            lastUpdated = DateTimeOffset.Now;
            return result;
        }

        // The state object is necessary for a TimerCallback.
        public void RunJobs(object stateObject)
        {
            Contract.Assert(isInitialised);

            var fn = Method.fn();

            var result = false;
            var now = DateTimeOffset.Now;

            if (!IsActive)
            {
                Trace.WriteLine("{0}: IsActive: {1}. Nothing to do.", fn, IsActive);
                
                goto Success;
            }

            lock(scheduledJobs)
            {
                if (0 >= scheduledJobs.Count)
                {
                    Trace.WriteLine("{0}: No scheduled jobs found. Nothing to do.", fn, "");
                    
                    goto Success;
                }

                var defaultPlugin = configuration.Plugins.FirstOrDefault
                (
                    p => p.Metadata.Type.Equals(biz.dfch.CS.Appclusive.Scheduler.Public.Constants.PLUGIN_TYPE_DEFAULT)
                );
                
                Trace.WriteLine("{0}: Processing {1} ScheduledJobs ...", fn, scheduledJobs.Count);
                foreach (var job in scheduledJobs)
                {
                    try
                    {
                        var jobScheduler = new ScheduledJobScheduler(job);
                        if (!jobScheduler.IsScheduledToRun(now))
                        {
                            continue;
                        }

                        // get plugin for ScheduledJob
                        var plugin = configuration.Plugins.FirstOrDefault(p => p.Metadata.Type.Equals(job.Action));
                        if(null == plugin)
                        {
                            Trace.WriteLine("No plugin for Job.Id '{0}' with Job.Action '{1}' found. Using 'Default' plugin ...", 
                                job.Id, job.Action);
                            plugin = defaultPlugin;
                        }
                        if(null == plugin)
                        {
                            Trace.WriteLine(
                                "No plugin for Job.Id '{0}' with Job.Action '{1}' found and no '{2}' plugin found either. Skipping.", 
                                job.Id, job.Action, biz.dfch.CS.Appclusive.Scheduler.Public.Constants.PLUGIN_TYPE_DEFAULT);

                            continue;
                        }

                        // invoke plugin
                        var invocationResult = new NonSerialisableJobResult();
                        
                        var scheduledJobsManager = new ScheduledJobsManager(endpoints);
                        var parameters = scheduledJobsManager.ConvertJobParameters(job.Action, job.ScheduledJobParameters);
                        parameters.Add("JobId", job.Id);

                        if(default(Guid) == System.Diagnostics.Trace.CorrelationManager.ActivityId)
                        {
                            System.Diagnostics.Trace.CorrelationManager.ActivityId = Guid.NewGuid();
                        }

                        configuration.Logger.WriteLine("Invoking {0} with Job.Action '{1}' [ActivityId {2}] ...", job.Id, job.Action, System.Diagnostics.Trace.CorrelationManager.ActivityId);
                        plugin.Value.Invoke(parameters, invocationResult);

                        if(invocationResult.Succeeded)
                        {
                            configuration.Logger.Info("Invoking {0} with Job.Action '{1}' [ActivityId {2}] SUCCEEDED.", job.Id, job.Action, System.Diagnostics.Trace.CorrelationManager.ActivityId);
                        }
                        else
                        {
                            configuration.Logger.Error("Invoking {0} with Job.Action '{1}' [ActivityId {2}] FAILED.", job.Id, job.Action, System.Diagnostics.Trace.CorrelationManager.ActivityId);
                        }
                    }
                    catch (Exception ex)
                    {
                        var message = string.Format("Invoking {0} with Job.Action '{1}' [ActivityId {2}] FAILED.", job.Id, job.Action, System.Diagnostics.Trace.CorrelationManager.ActivityId);
                        Trace.WriteException(message, ex);
                    }
                }
                Trace.WriteLine("{0}: Processing {1} ScheduledJobs COMPLETED.", fn, scheduledJobs.Count);
            }
Success:
            if (configuration.UpdateIntervalInMinutes <= (now - lastUpdated).TotalMinutes)
            {
                result = GetScheduledJobs();
                Contract.Assert(result);
            }

            return;
        }

        ~ScheduledJobsWorker()
        {
            if(null != stateTimer)
            {
                stateTimer.Dispose();
            }
        }
    }
}
