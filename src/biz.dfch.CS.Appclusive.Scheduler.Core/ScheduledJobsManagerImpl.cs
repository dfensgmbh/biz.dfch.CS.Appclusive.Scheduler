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

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class ScheduledJobsManagerImpl
    {
        public List<ScheduledJob> Load()
        {
            var scheduledJobs = new List<ScheduledJob>();

            var scheduledJob1 = new ScheduledJob()
            {
                Crontab = "00 03 * * *"
                ,
                Action = "InternalWorkflow"
                ,
                ScheduledJobParameters = "{\"Id\":\"com.swisscom.cms.agentbasedbackup.v002.RunScheduledBackup\",\"Parameters\":\"{\\\"nodeId\\\":\\\"14158\\\"}\"}"
                ,
                ParallelInvocation = "DoNotStartNewInstance"
                ,
                MaximumRuntimeMinutes = 10
                ,
                AutoDeleteIfNotScheduledDays = 10
                ,
                MaxRestartAttempts = 0
                ,
                MaxRestartWaitTimeMinutes = 1
                ,
                HistoryDepth = 0
                ,
                Parameters = "{}"
                ,
                EntityKindId = (long) Constants.EntityKindId.ScheduledJob
                ,
                ParentId = 14158L
                ,
                Id = 14159L
                ,
                Tid = new Guid("ad8f50df-2a5d-4ea5-9fcc-05882f16a9fe")
                ,
                Name = "ValidInternalWorkflow"
                ,
                Description = "ValidInternalWorkflow Description"
                ,
                CreatedById = 1011L
                ,
                ModifiedById = 1011L
                ,
                Created = new DateTimeOffset(2015, 01, 01, 08, 15, 42, TimeSpan.FromHours(2))
                ,
                Modified = new DateTimeOffset(2015, 01, 01, 08, 15, 42, TimeSpan.FromHours(2))
            };

            var scheduledJob2 = new ScheduledJob()
            {
                Crontab = "* * * * *"
                ,
                Action = "ExternalWorkflow"
                ,
                ScheduledJobParameters = "{\"Id\":\"com.swisscom.cms.agentbasedbackup.v002.RunScheduledBackup\",\"Parameters\":\"{\\\"nodeId\\\":\\\"14158\\\"}\"}"
                ,
                ParallelInvocation = "DoNotStartNewInstance"
                ,
                MaximumRuntimeMinutes = 10
                ,
                AutoDeleteIfNotScheduledDays = 10
                ,
                MaxRestartAttempts = 0
                ,
                MaxRestartWaitTimeMinutes = 1
                ,
                HistoryDepth = 0
                ,
                Parameters = "{}"
                ,
                EntityKindId = (long) Constants.EntityKindId.ScheduledJob
                ,
                ParentId = 24158L
                ,
                Id = 24159L
                ,
                Tid = new Guid("ad8f50df-2a5d-4ea5-9fcc-05882f16a9fe")
                ,
                Name = "ValidExternalWorkflow"
                ,
                Description = "ValidExternalWorkflow Description"
                ,
                CreatedById = 1011L
                ,
                ModifiedById = 1011L
                ,
                Created = new DateTimeOffset(2016, 01, 01, 08, 15, 42, TimeSpan.FromHours(2))
                ,
                Modified = new DateTimeOffset(2016, 01, 01, 08, 15, 42, TimeSpan.FromHours(2))
            };

            var scheduledJob3 = new ScheduledJob()
            {
                Crontab = "00 03 * * *"
                ,
                Action = "InternalWorkflow"
                ,
                ScheduledJobParameters = "{\"WrongId\":\"com.swisscom.cms.agentbasedbackup.v002.RunScheduledBackup\",\"Parameters\":\"{\\\"nodeId\\\":\\\"14158\\\"}\"}"
                ,
                ParallelInvocation = "DoNotStartNewInstance"
                ,
                MaximumRuntimeMinutes = 10
                ,
                AutoDeleteIfNotScheduledDays = 10
                ,
                MaxRestartAttempts = 0
                ,
                MaxRestartWaitTimeMinutes = 1
                ,
                HistoryDepth = 0
                ,
                Parameters = "{}"
                ,
                EntityKindId = (long) Constants.EntityKindId.ScheduledJob
                ,
                ParentId = 14158L
                ,
                Id = 14159L
                ,
                Tid = new Guid("ad8f50df-2a5d-4ea5-9fcc-05882f16a9fe")
                ,
                Name = "InvalidInternalWorkflow"
                ,
                Description = "InvalidInternalWorkflow Description"
                ,
                CreatedById = 1011L
                ,
                ModifiedById = 1011L
                ,
                Created = new DateTimeOffset(2015, 01, 01, 08, 15, 42, TimeSpan.FromHours(2))
                ,
                Modified = new DateTimeOffset(2015, 01, 01, 08, 15, 42, TimeSpan.FromHours(2))
            };

            var scheduledJob4 = new ScheduledJob()
            {
                Crontab = "00 03 * * *"
                ,
                Action = "ExternalWorkflow"
                ,
                ScheduledJobParameters = "{\"WrongId\":\"com.swisscom.cms.agentbasedbackup.v002.RunScheduledBackup\",\"Parameters\":\"{\\\"nodeId\\\":\\\"14158\\\"}\"}"
                ,
                ParallelInvocation = "DoNotStartNewInstance"
                ,
                MaximumRuntimeMinutes = 10
                ,
                AutoDeleteIfNotScheduledDays = 10
                ,
                MaxRestartAttempts = 0
                ,
                MaxRestartWaitTimeMinutes = 1
                ,
                HistoryDepth = 0
                ,
                Parameters = "{}"
                ,
                EntityKindId = (long) Constants.EntityKindId.ScheduledJob
                ,
                ParentId = 14158L
                ,
                Id = 14159L
                ,
                Tid = new Guid("ad8f50df-2a5d-4ea5-9fcc-05882f16a9fe")
                ,
                Name = "InvalidExternalWorkflow"
                ,
                Description = "InvalidExternalWorkflow Description"
                ,
                CreatedById = 1011L
                ,
                ModifiedById = 1011L
                ,
                Created = new DateTimeOffset(2015, 01, 01, 08, 15, 42, TimeSpan.FromHours(2))
                ,
                Modified = new DateTimeOffset(2015, 01, 01, 08, 15, 42, TimeSpan.FromHours(2))
            };

            scheduledJobs.Add(scheduledJob1);
            scheduledJobs.Add(scheduledJob2);
            scheduledJobs.Add(scheduledJob3);
            scheduledJobs.Add(scheduledJob4);

            return scheduledJobs;
        }
    }
}
