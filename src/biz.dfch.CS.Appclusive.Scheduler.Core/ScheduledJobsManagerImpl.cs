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
using System.Data.Services.Client;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Appclusive.Api.Core;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using biz.dfch.CS.Utilities.Logging;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class ScheduledJobsManagerImpl
    {
        public List<ScheduledJob> GetJobs(AppclusiveEndpoints endpoints)
        {
            Contract.Requires(null != endpoints);
            Contract.Ensures(null != Contract.Result<List<ScheduledJob>>());

            try
            {
                var scheduledJobs = new List<ScheduledJob>();

                var entities = endpoints.Core.ScheduledJobs.Execute() as QueryOperationResponse<ScheduledJob>;
                while(null != entities)
                {
                    foreach (var entity in entities)
                    {
                        scheduledJobs.Add(entity);
                        Contract.Assert(ScheduledJobsWorker.SCHEDULED_JOBS_WORKER_JOBS_PER_INSTANCE_MAX >= scheduledJobs.Count);
                    }

                    var dataServiceQueryContinuation = entities.GetContinuation();
                    if (null != dataServiceQueryContinuation)
                    {
                        entities = endpoints.Core.Execute(dataServiceQueryContinuation);
                    }
                    else
                    {
                        entities = null;
                    }
                }
                
                return scheduledJobs;
            }
            catch (DataServiceQueryException ex)
            {
                if(null == ex.InnerException || !(ex.InnerException is DataServiceClientException))
                {
                    throw;
                }

                var exInner = (DataServiceClientException) ex.InnerException;
                var message = string.Format("Loading ScheduledJobs from '{0}' FAILED with DataServiceClientException. The StatusCode was {1}. '{2}'", endpoints.Core.BaseUri.AbsoluteUri, exInner.StatusCode, exInner.Message);
                Trace.WriteException(message, ex);
                throw;
            }
        }
    }
}
