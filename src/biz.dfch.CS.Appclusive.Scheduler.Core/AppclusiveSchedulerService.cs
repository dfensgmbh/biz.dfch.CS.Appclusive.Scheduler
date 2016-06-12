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
 *
 */
using System.Diagnostics.Contracts;
using biz.dfch.CS.Utilities.General;
using biz.dfch.CS.Utilities.Logging;
using System;
using System.ServiceProcess;
using System.Threading;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public partial class AppclusiveSchedulerService : ServiceBase
    {
        private ManualResetEvent serviceAbortSignal = new ManualResetEvent(false);

        ScheduledJobsWorker scheduledJobsWorker;

        public AppclusiveSchedulerService()
        {
            this.CanPauseAndContinue = true;
            InitializeComponent();
        }

        public void TerminateInteractiveService()
        {
            serviceAbortSignal.Set();
        }

        public void OnStartInteractive(string[] args)
        {
            var fn = Method.fn();
            Trace.WriteLine("{0}.{1}", this.GetType().FullName, fn);

            try
            {
                Console.WriteLine(new ProgramHelp().GetInteractiveMessage());

                OnStart(args);
                serviceAbortSignal.WaitOne();
                Console.WriteLine(string.Format("CancelKeyPress detected. Stopping interactive mode."));
            }
            catch (Exception ex)
            {
                var message = string.Format("{0}@{1}: '{2}'\r\n[{3}]\r\n{4}", ex.GetType().Name, ex.Source, "Stopping interactive mode.", ex.Message, ex.StackTrace);
                Console.WriteLine(message);
            }
            finally
            {
                OnStop();
            }
        }

        protected override void OnStart(string[] args)
        {
            var fn = Method.fn();
            Trace.WriteLine("{0}.{1}", this.GetType().FullName, fn);

            var configuration = new ScheduledJobsWorkerConfiguration(new ScheduledJobsWorkerConfigurationLoader(), args);
            scheduledJobsWorker = new ScheduledJobsWorker(configuration);

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            var fn = Method.fn();
            Trace.WriteLine("{0}.{1}", this.GetType().FullName, fn);

            if(null != scheduledJobsWorker)
            {
                scheduledJobsWorker.IsActive = false;
            }

            base.OnStop();
        }
        
        protected override void OnPause()
        {
            var fn = Method.fn();
            Trace.WriteLine("{0}.{1}", this.GetType().FullName, fn);

            if(null != scheduledJobsWorker)
            {
                scheduledJobsWorker.IsActive = false;
            }

            base.OnPause();
        }
        
        protected override void OnContinue()
        {
            var fn = Method.fn();
            Trace.WriteLine("{0}.{1}", this.GetType().FullName, fn);

            if(null != scheduledJobsWorker)
            {
                scheduledJobsWorker.IsActive = true;
                var isUpdateScheduledJobsSucceeded = scheduledJobsWorker.GetScheduledJobs();
                Contract.Assert(isUpdateScheduledJobsSucceeded);
            }

            base.OnContinue();
        }
        
        protected override void OnCustomCommand(int command)
        {
            var fn = Method.fn();
            Trace.WriteLine("{0}.{1}", this.GetType().FullName, fn);

            base.OnCustomCommand(command);
        }
        
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            var fn = Method.fn();
            Trace.WriteLine("{0}.{1}", this.GetType().FullName, fn);

            base.OnSessionChange(changeDescription);
        }
        
        protected override void OnShutdown()
        {
            var fn = Method.fn();
            Trace.WriteLine("{0}.{1}", this.GetType().FullName, fn);

            if(null != scheduledJobsWorker)
            {
                scheduledJobsWorker.IsActive = false;
            }

            base.OnShutdown();
        }
    }
}
