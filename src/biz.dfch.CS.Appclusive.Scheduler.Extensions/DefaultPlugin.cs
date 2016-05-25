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

using System.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Diagnostics;
using biz.dfch.CS.Appclusive.Scheduler.Public;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    [Export(typeof(ISchedulerPlugin))]
    [ExportMetadata("Type", "Default")]
    [ExportMetadata("Priority", int.MinValue)]
    public class DefaultPlugin : ISchedulerPlugin
    {
        public Dictionary<string, object> Configuration { get; set; }

        public void Log(string message)
        {
            Trace.WriteLine(message);

            return;
        }

        public bool UpdateConfiguration(Dictionary<string, object> configuration)
        {
            var fReturn = false;
        
            var message = new StringBuilder();
            message.AppendLine("DefaultPlugin.UpdatingConfiguration ...");
            message.AppendLine();

            foreach(KeyValuePair<string, object> item in configuration)
            {
                message.AppendFormat("{0}: '{1}'", item.Key, item.Value ?? item.Value.ToString());
                message.AppendLine();
            }
            message.AppendLine();
            message.AppendLine("DefaultPlugin.UpdatingConfiguration COMPLETED.");
            
            Trace.WriteLine(message.ToString());

            fReturn = true;
            
            return fReturn;
        }

        public bool Invoke(Dictionary<string, object> data, ref JobResult jobResult)
        {
            Contract.Requires("1" == jobResult.Version);
            Contract.Ensures(jobResult.IsValid());

            var fReturn = false;

            var message = new StringBuilder();
            message.AppendLine("DefaultPlugin.Invoke ...");
            message.AppendLine();

            foreach(KeyValuePair<string, object> item in data)
            {
                message.AppendFormat("{0}: '{1}'", item.Key, item.Value ?? item.Value.ToString());
                message.AppendLine();
            }
            message.AppendLine("DefaultPlugin.Invoke() COMPLETED.");
            message.AppendLine();
            
            Trace.WriteLine(message.ToString());

            fReturn = true;
            
            jobResult.Succeeded = fReturn;
            jobResult.Code = 1;
            jobResult.Message = "DefaultPlugin.Invoke COMPLETED and logged the intended operation to a tracing facility.";
            jobResult.Description = message.ToString();
            jobResult.InnerJobResult = null;

            return fReturn;
        }
    }
}
