/**
 * Copyright 2016 d-fens GmbH
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
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Appclusive.Api.Core;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Public.OdataServices.Core;
using biz.dfch.CS.Utilities.Logging;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class ScheduledJobsManager
    {
        private static readonly ScheduledJobsManagerImpl scheduledJobsManagerImpl;
        private readonly AppclusiveEndpoints appclusiveEndpoints;
        
        static ScheduledJobsManager()
        {
            scheduledJobsManagerImpl = new ScheduledJobsManagerImpl();
        }

        public ScheduledJobsManager(AppclusiveEndpoints endpoints)
        {
            Contract.Requires(null != endpoints);
            Contract.Requires(null != endpoints.Core);
            appclusiveEndpoints = endpoints;
        }

        public List<ScheduledJob> GetJobs()
        {
            Contract.Ensures(null != Contract.Result<List<ScheduledJob>>());

            var result = scheduledJobsManagerImpl.GetJobs(appclusiveEndpoints);
            return result;
        }

        public bool ConvertActionToEnum(string action, ref int value)
        {
            Contract.Requires(null != action);

            JobActionEnum jobActionEnum = default(JobActionEnum);
            var result = Enum.TryParse<JobActionEnum>(action, out jobActionEnum);
            if(result)
            {
                value = (int) jobActionEnum;
            }

            return result;
        }

        public List<ScheduledJob> GetValidJobs(List<ScheduledJob> jobs)
        {
            Contract.Requires(null != jobs);
            Contract.Ensures(null != Contract.Result<List<ScheduledJob>>());

            var validJobs = new List<ScheduledJob>();

            foreach(var job in jobs)
            {
                int action = default(int);
                var result = ConvertActionToEnum(job.Action, ref action);
                if(!result)
                {
                    Trace.WriteLine("Parsing scheduled job '{0}' [{1}] FAILED. Invalid action '{2}'.", job.Id.ToString(), job.Name, job.Action);
                    continue;
                }

                var isValidAction = true;
                switch(action)
                {
                    default:
                        isValidAction = false;
                        break;
                    case (int) JobActionEnum.Programme:
                        var actionProgramme = BaseDto.DeserializeObject<JobActionProgramme>(job.ScheduledJobParameters);
                        isValidAction = actionProgramme.IsValid();
                        break;
                    case (int) JobActionEnum.Mail:
                        var actionMail = BaseDto.DeserializeObject<JobActionMail>(job.ScheduledJobParameters);
                        isValidAction = actionMail.IsValid();
                        break;
                    case (int) JobActionEnum.InternalWorkflow:
                        var actionInternalWorkflow = BaseDto.DeserializeObject<JobActionInternalWorkflow>(job.ScheduledJobParameters);
                        isValidAction = actionInternalWorkflow.IsValid();
                        break;
                    case (int) JobActionEnum.ExternalWorkflow:
                        var actionExternalWorkflow = BaseDto.DeserializeObject<JobActionExternalWorkflow>(job.ScheduledJobParameters);
                        isValidAction = actionExternalWorkflow.IsValid();
                        break;
                    case (int) JobActionEnum.PowerShellScript:
                        var actionPowerShellScript = BaseDto.DeserializeObject<JobActionPowerShellScript>(job.ScheduledJobParameters);
                        isValidAction = actionPowerShellScript.IsValid();
                        break;
                }

                if(!isValidAction)
                {
                    Trace.WriteLine("Parsing scheduled job '{0}' [{1}] FAILED. Invalid ScheduledJobParameters.", job.Id.ToString(), job.Name);
                    continue;
                }

                validJobs.Add(job);
            }

            return validJobs;
        }
    }
}
