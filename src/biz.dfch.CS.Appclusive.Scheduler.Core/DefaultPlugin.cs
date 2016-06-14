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
using System.ComponentModel.Composition;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Public.Plugins;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
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
            
            if (null != Logger)
            { 
                Logger.WriteLine("[{0}] {1}", System.Diagnostics.Trace.CorrelationManager.ActivityId, message.ToString());
            }

            this.configuration = configuration;

            return this.configuration;
        }

        public override bool Initialise(DictionaryParameters parameters, IAppclusivePluginLogger logger, bool activate)
        {
            var result = false;

            result = base.Initialise(parameters, logger, activate);
            if(!configuration.IsValid())
            {
                return result;
            }

            return result;
        }

        public override bool Invoke(DictionaryParameters parameters, IInvocationResult jobResult)
        {
            var result = base.Invoke(parameters, jobResult);
            if(!result)
            {
                return result;
            }

            var message = new StringBuilder();
            message.AppendLine("[{0}] DefaultPlugin.Invoke ...");
            message.AppendLine();

            foreach(KeyValuePair<string, object> item in parameters)
            {
                message.AppendFormat("{0}: '{1}'", item.Key, item.Value ?? item.Value.ToString());
                message.AppendLine();
            }
            message.AppendLine("DefaultPlugin.Invoke() COMPLETED.");
            message.AppendLine();
            
            Logger.WriteLine("[{0}] {1}", System.Diagnostics.Trace.CorrelationManager.ActivityId, message.ToString());

            result = true;
            
            jobResult.Succeeded = result;
            jobResult.Code = 1;
            jobResult.Message = "DefaultPlugin.Invoke COMPLETED and logged the intended operation to a tracing facility.";
            jobResult.Description = message.ToString();
            jobResult.InnerJobResult = null;

            return result;
        }
    }
}
