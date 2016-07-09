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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    public class ThreadPoolUserWorkItem
    {
        public static void ThreadProc(Object data)
        {
            Contract.Requires(data is ThreadPoolUserWorkItemParameters);
            
            var parameters = data as ThreadPoolUserWorkItemParameters;
            Contract.Assert(null != parameters);
            Contract.Assert(parameters.IsValid());

            parameters.Logger.WriteLine("{0}: Invoking '{1}' ...", parameters.ActivityId, parameters.ScriptPathAndName);

            Trace.CorrelationManager.StartLogicalOperation(string.Format("PowerShellScriptPlugin-{0}", parameters.ActivityId));

            var scriptResult = new List<object>();
            using(var scriptInvoker = new ScriptInvoker(parameters.Logger))
            {
                var hasScriptSucceeded = scriptInvoker.RunPowershell(parameters.ScriptPathAndName, parameters.ScriptParameters, ref scriptResult);
                if (!hasScriptSucceeded)
                {
                    parameters.Logger.Error("{0}: Invoking '{1}' FAILED.", parameters.ActivityId, parameters.ScriptPathAndName);
                    return;
                }
            }
        
            Trace.CorrelationManager.StopLogicalOperation();

            if(null == scriptResult)
            {
                parameters.Logger.Error("{0}: Invoking '{1}' COMPLETED but returned an invalid ScriptResult (incorrectly set to null).", parameters.ActivityId, parameters.ScriptPathAndName);
                
                return;
            }

            var c = 0;
            foreach(var item in scriptResult)
            {
                parameters.Logger.WriteLine("{0}: scriptResult[{1}]: '{2}'", parameters.ActivityId, c, item.ToString());

                c++;
            }

            parameters.Logger.WriteLine("{0}: Invoking '{1}' SUCCEEDED.", parameters.ActivityId, parameters.ScriptPathAndName);
        }
    }
}
