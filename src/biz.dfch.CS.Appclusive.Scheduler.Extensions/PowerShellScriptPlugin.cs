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
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using biz.dfch.CS.Appclusive.Public.Plugins;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    [Export(typeof(IAppclusivePlugin))]
    [ExportMetadata("Type", "PowerShellScript")]
    [ExportMetadata("Priority", int.MinValue + 1)]
    [ExportMetadata("Role", "default")]
    public class PowerShellScriptPlugin : SchedulerPluginBase
    {
        private PowerShellScriptPluginConfiguration configuration;
        //private DictionaryParameters configuration;
        public override DictionaryParameters Configuration 
        { 
            get
            {
                return new DictionaryParameters().Convert(configuration);
            }
            set
            {
                var newConfiguration = UpdateConfiguration(value);
                configuration = newConfiguration.Convert<PowerShellScriptPluginConfiguration>();
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

            this.configuration = configuration.Convert<PowerShellScriptPluginConfiguration>();

            return Configuration;
        }

        public override bool Invoke(DictionaryParameters parameters, IInvocationResult invocationResult)
        {
            Contract.Requires<NotSupportedException>("2" == invocationResult.Version, "This plugin only supports non-serialisable invocation results.");

            var fReturn = false;

            if(!IsActive)
            {
                invocationResult.Succeeded = fReturn;
                invocationResult.Code = 1;
                invocationResult.Message = "Plugin inactive";

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
            
            invocationResult.Succeeded = fReturn;
            invocationResult.Code = 1;
            invocationResult.Message = "DefaultPlugin.Invoke COMPLETED and logged the intended operation to a tracing facility.";
            invocationResult.Description = message.ToString();
            invocationResult.InnerJobResult = null;

            return fReturn;
        }
    }
}
