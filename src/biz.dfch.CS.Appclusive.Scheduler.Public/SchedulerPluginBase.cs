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
using System.Diagnostics.Contracts;
using System.Linq;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Public.Plugins;

namespace biz.dfch.CS.Appclusive.Scheduler.Public
{
    public abstract class SchedulerPluginBase 
        : AppclusivePluginBase, ISchedulerPlugin
    {
        public override bool Invoke(DictionaryParameters parameters, IInvocationResult jobResult)
        {
            var result = IsActive;

            var description = string.Format("ActivityId '{0}'.", System.Diagnostics.Trace.CorrelationManager.ActivityId.ToString());
            jobResult.Description = description;
            jobResult.Succeeded = result;
            
            if(!result)
            {
                var message = "Plugin not active";

                Logger.Warn("{0} {1}. Nothing to do.", description, message);

                jobResult.Code = Constants.InvocationResultCodes.ERROR_SERVICE_NOT_ACTIVE;
                jobResult.Message = message;
            }
            else
            {
                var message = "Plugin active";

                jobResult.Code = Constants.InvocationResultCodes.ERROR_INVALID_FUNCTION;
                jobResult.Message = message;
            }
            
            return result;
        }
    }
}
