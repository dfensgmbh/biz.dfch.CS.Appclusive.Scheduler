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

using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Public.Plugins;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Text;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    [Export(typeof(IAppclusivePlugin))]
    [ExportMetadata("Type", "Default")]
    [ExportMetadata("Priority", int.MinValue)]
    [ExportMetadata("Role", "default")]
    public class DefaultPlugin : SchedulerPluginBase
    {
        private DictionaryParameters configuration;
        public override DictionaryParameters Configuration 
        { 
            get
            {
                return configuration;
            }
            set
            {
                configuration = UpdateConfiguration(value);
            }
        }

        private DictionaryParameters UpdateConfiguration(DictionaryParameters configuration)
        {
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

            this.configuration = configuration;

            return this.configuration;
        }

        public override bool Invoke(DictionaryParameters parameters, IInvocationResult jobResult)
        {
            Contract.Requires("1" == jobResult.Version);

            var fReturn = false;

            if(!IsActive)
            {
                jobResult.Succeeded = fReturn;
                jobResult.Code = 1;
                jobResult.Message = "Plugin inactive";

                return fReturn;
            }

            var message = new StringBuilder();
            message.AppendLine("DefaultPlugin.Invoke ...");
            message.AppendLine();

            foreach(KeyValuePair<string, object> item in parameters)
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
