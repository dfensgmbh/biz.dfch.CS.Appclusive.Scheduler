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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.ComponentModel.Composition;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using biz.dfch.CS.Appclusive.Public.Plugins;
using System.Diagnostics;
using System.Threading;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    [Export(typeof(IAppclusivePlugin))]
    [ExportMetadata("Type", "PowerShellScript")]
    [ExportMetadata("Priority", int.MinValue)]
    [ExportMetadata("Role", "default")]
    public class PowerShellScriptPlugin : SchedulerPluginBase
    {
        public const string SCRIPT_NAME_KEY = "ScriptName";

        private PowerShellScriptPluginConfiguration configuration;
        public override DictionaryParameters Configuration 
        { 
            get
            {
                return configuration.Convert(true);
            }
            set
            {
                var result = UpdateConfiguration(value);
                configuration = result;
            }
        }

        private PowerShellScriptPluginConfiguration UpdateConfiguration(DictionaryParameters parameters)
        {
            Contract.Requires(parameters.IsValid());

            // check for endpoint
            var configurationBase = SchedulerPluginConfigurationBase.Convert<SchedulerPluginConfigurationBase>(parameters);
            Contract.Assert(configurationBase.IsValid());
            
            var configurationManager = new PowerShellScriptPluginConfigurationManager(configurationBase.Endpoints);
            parameters.Add("ComputerName", configurationManager.GetComputerName());
            parameters.Add("ConfigurationName", configurationManager.GetConfigurationNameTemplate());
            parameters.Add("ScriptBase", configurationManager.GetScriptBase());
            parameters.Add("Credential", configurationManager.GetCredentials());

            var message = new StringBuilder();
            message.AppendLine("PowerShellScriptPlugin.UpdatingConfiguration ...");
            message.AppendLine();

            foreach(KeyValuePair<string, object> item in parameters)
            {
                message.AppendFormat("{0}: '{1}'", item.Key, item.Value ?? item.Value.ToString());
                message.AppendLine();
            }
            message.AppendLine();
            message.AppendLine("PowerShellScriptPlugin.UpdatingConfiguration COMPLETED.");
            
            Logger.WriteLine(message.ToString());

            configuration = PowerShellScriptPluginConfiguration.Convert<PowerShellScriptPluginConfiguration>(parameters, true);
            Contract.Assert(configuration.IsValid());

            return configuration;
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

        public override bool Invoke(DictionaryParameters parameters, IInvocationResult invocationResult)
        {
            Contract.Requires("2" == invocationResult.Version, "This plugin only supports non-serialisable invocation results.");

            var fReturn = false;

            var result = base.Invoke(parameters, invocationResult);
            if(!result)
            {
                return result;
            }

            var message = new StringBuilder();
            message.AppendLine("PowerShellScriptPlugin.Invoke ...");
            message.AppendLine();

            Logger.WriteLine(message.ToString());
            message.Clear();

            var scriptPathAndName = parameters.GetOrDefault(SCRIPT_NAME_KEY, "") as string;
            parameters.Remove(SCRIPT_NAME_KEY);

            var scriptParameters = (Dictionary<string, object>) parameters;
            Contract.Assert(null != scriptParameters);

            var activityId = Trace.CorrelationManager.ActivityId;

            foreach(var item in scriptParameters)
            {
                message.AppendFormat("{0} - {1}", item.Key, item.Value);
                message.AppendLine();
            }
            Logger.WriteLine(message.ToString());
            message.Clear();

            var data = new ThreadPoolUserWorkItemParameters()
            {
                ActivityId = activityId
                ,
                Logger = Logger
                ,
                ScriptParameters = scriptParameters
                ,
                ScriptPathAndName = scriptPathAndName
            };
            ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadPoolUserWorkItem.ThreadProc), data);

            message.AppendLine("PowerShellScriptPlugin.Invoke DISPATCHED.");
            message.AppendLine();
            
            Logger.WriteLine(message.ToString());

            fReturn = true;
            
            invocationResult.Succeeded = fReturn;
            invocationResult.Code = 1;
            invocationResult.Message = "PowerShellScriptPlugin.Invoke COMPLETED and logged the intended operation to a tracing facility.";
            invocationResult.Description = message.ToString();
            invocationResult.InnerJobResult = null;

            return fReturn;
        }
    }
}
