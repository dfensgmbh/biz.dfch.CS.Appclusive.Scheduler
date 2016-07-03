﻿/**
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
using System.Net;
using System.Text;
using System.ComponentModel.Composition;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using biz.dfch.CS.Appclusive.Public.Plugins;
using System.Diagnostics;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    [Export(typeof(IAppclusivePlugin))]
    [ExportMetadata("Type", "PowerShellScript")]
    [ExportMetadata("Priority", int.MinValue)]
    [ExportMetadata("Role", "default")]
    public class PowerShellScriptPlugin : SchedulerPluginBase
    {
        public const string SCRIPT_PATH_AND_NAME_KEY = "ScriptPathAndName";

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
            var endpoints = configurationBase.Endpoints;
            
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
            
            if (null != Logger)
            { 
                Logger.WriteLine(message.ToString());
            }

            configuration = PowerShellScriptPluginConfiguration.Convert<PowerShellScriptPluginConfiguration>(parameters, true);
            Contract.Assert(configuration.IsValid());

            return configuration;
        }

        public override bool Initialise(DictionaryParameters parameters, IAppclusivePluginLogger logger, bool activate)
        {
            var result = false;

            var configurationManager = new PowerShellScriptPluginConfigurationManager();
            parameters.Add("ComputerName", configurationManager.GetComputerName());
            parameters.Add("ConfigurationName", configurationManager.GetConfigurationNameTemplate());
            parameters.Add("ScriptBase", configurationManager.GetScriptBase());
            parameters.Add("Credential", configurationManager.GetCredentials());

            result = base.Initialise(parameters, logger, activate);
            if(!configuration.IsValid())
            {
                return result;
            }

            return result;
        }
        
        public override bool Invoke(DictionaryParameters parameters, IInvocationResult invocationResult)
        {
            Contract.Requires<NotSupportedException>("2" == invocationResult.Version, "This plugin only supports non-serialisable invocation results.");

            var fReturn = false;

            var result = base.Invoke(parameters, invocationResult);
            if(!result)
            {
                return result;
            }

            var message = new StringBuilder();
            message.AppendLine("PowerShellScriptPlugin.Invoke ...");
            message.AppendLine();

            Contract.Assert(parameters.ContainsKey(SCRIPT_PATH_AND_NAME_KEY));
            var scriptPathAndName = parameters[SCRIPT_PATH_AND_NAME_KEY] as string;
            Contract.Assert(!string.IsNullOrWhiteSpace(scriptPathAndName));

            parameters.Remove(SCRIPT_PATH_AND_NAME_KEY);
            Contract.Assert(!parameters.ContainsKey(SCRIPT_PATH_AND_NAME_KEY));

            var scriptParameters = (Dictionary<string, object>) parameters;
            Contract.Assert(null != scriptParameters);

            var activityId = Trace.CorrelationManager.ActivityId;

            Trace.CorrelationManager.StartLogicalOperation(string.Format("PowerShellScriptPlugin-{0}", activityId));

            var scriptResult = new List<object>();
            using(var scriptInvoker = new ScriptInvoker(Logger))
            {
                var hasScriptSucceeded = scriptInvoker.RunPowershell(scriptPathAndName, scriptParameters, ref scriptResult);
                Contract.Assert(true == hasScriptSucceeded);
            }

            Trace.CorrelationManager.StopLogicalOperation();

            var c = 0;
            foreach(var item in scriptResult)
            {
                message.AppendFormat("scriptResult[{0}]: '{1}'", c, item.ToString());
                message.AppendLine();

                c++;
            }

            message.AppendLine("PowerShellScriptPlugin.Invoke COMPLETED.");
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
