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
using biz.dfch.CS.Appclusive.Scheduler.Public;
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

        // DFTODO - change T : BaseDto to T : ScheduledJobBase 
        // as soon as we update biz.dfch.CS.Appclusive.Public
        public T ConvertJobParameters<T>(string parameters)
            where T : BaseDto
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(parameters));
            Contract.Ensures(null != Contract.Result<T>());

            var result = BaseDto.DeserializeObject<T>(parameters);
            return result;
        }

        public DictionaryParameters ConvertJobParameters(string action, string parameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(action));
            Contract.Requires(!string.IsNullOrWhiteSpace(parameters));
            Contract.Ensures(null != Contract.Result<DictionaryParameters>());

            var result = default(DictionaryParameters);

            int actionInt = default(int);
            var isValidAction = ConvertActionToEnum(action, ref actionInt);
            Contract.Assert(isValidAction);

            switch (actionInt)
            {
                default:
                    break;
                case (int) JobActionEnum.Programme:
                    var jobActionProgramme = BaseDto.DeserializeObject<JobActionProgramme>(parameters);
                    Contract.Assert(jobActionProgramme.IsValid());
                    result = new DictionaryParameters(jobActionProgramme.SerializeObject());
                    break;
                case (int) JobActionEnum.Mail:
                    var jobActionMail = BaseDto.DeserializeObject<JobActionMail>(parameters);
                    Contract.Assert(jobActionMail.IsValid());
                    result = new DictionaryParameters(jobActionMail.SerializeObject());
                    break;
                case (int) JobActionEnum.InternalWorkflow:
                    var jobActionInternalWorkflow = BaseDto.DeserializeObject<JobActionInternalWorkflow>(parameters);
                    Contract.Assert(jobActionInternalWorkflow.IsValid());
                    result = new DictionaryParameters(jobActionInternalWorkflow.SerializeObject());
                    break;
                case (int) JobActionEnum.ExternalWorkflow:
                    var jobActionExternalWorkflow = BaseDto.DeserializeObject<JobActionExternalWorkflow>(parameters);
                    Contract.Assert(jobActionExternalWorkflow.IsValid());
                    result = new DictionaryParameters(jobActionExternalWorkflow.SerializeObject());
                    break;
                case (int) JobActionEnum.PowerShellScript:
                    var jobActionPowerShellScript = BaseDto.DeserializeObject<JobActionPowerShellScript>(parameters);
                    Contract.Assert(jobActionPowerShellScript.IsValid());
                    result = new DictionaryParameters(jobActionPowerShellScript.SerializeObject());
                    break;
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
                try
                {
                    var parameters = ConvertJobParameters(job.Action, job.ScheduledJobParameters);
                    validJobs.Add(job);
                }
                catch(Exception ex)
                {
                    Trace.WriteLine("Parsing scheduled job '{0}' [{1}] FAILED. Invalid Action or ScheduledJobParameters.", job.Id.ToString(), job.Name);

                    continue;
                }
            }

            return validJobs;
        }
    }
}
