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
using biz.dfch.CS.Appclusive.Scheduler.Public;
using biz.dfch.CS.Utilities.General;
using biz.dfch.CS.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public partial class AppclusiveSchedulerService : ServiceBase
    {
        private ManualResetEvent serviceAbortSignal = new ManualResetEvent(false);

        ScheduledTaskWorker scheduledTaskWorker;

        public AppclusiveSchedulerService()
        {
            this.CanPauseAndContinue = true;
            InitializeComponent();
        }

        public void TerminateInteractiveService()
        {
            serviceAbortSignal.Set();
        }

        internal void OnStartInteractive(string[] args)
        {
            var fn = Method.fn();
            Trace.WriteLine("{0}.{1}", this.GetType().FullName, fn);

            try
            {
                var message = @"d-fens AppclusiveScheduler
Copyright (C) d-fens GmbH. Source code licensed under Apache 2.0 license.

Starting service interactively. Press Ctrl-C to abort ...";
                Console.WriteLine(message);

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

            var configuration = new ScheduledTaskWorkerConfiguration(new ScheduledTaskWorkerConfigurationLoader(), args);
            scheduledTaskWorker = new ScheduledTaskWorker(configuration);

            base.OnStart(args);
        }

        protected override void OnStop()
        {
            var fn = Method.fn();
            Trace.WriteLine("{0}.{1}", this.GetType().FullName, fn);

            if(null != scheduledTaskWorker)
            {
                scheduledTaskWorker.IsActive = false;
            }

            base.OnStop();
        }
        
        protected override void OnPause()
        {
            var fn = Method.fn();
            Trace.WriteLine("{0}.{1}", this.GetType().FullName, fn);

            if(null != scheduledTaskWorker)
            {
                scheduledTaskWorker.IsActive = false;
            }

            base.OnPause();
        }
        
        protected override void OnContinue()
        {
            var fn = Method.fn();
            Trace.WriteLine("{0}.{1}", this.GetType().FullName, fn);

            if(null != scheduledTaskWorker)
            {
                scheduledTaskWorker.IsActive = true;
                scheduledTaskWorker.UpdateScheduledTasks();
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

            if(null != scheduledTaskWorker)
            {
                scheduledTaskWorker.IsActive = false;
            }

            base.OnShutdown();
        }
    }
}
