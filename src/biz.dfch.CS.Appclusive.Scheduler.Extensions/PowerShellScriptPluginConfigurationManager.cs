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

using System.Configuration;
using System.IO;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    public class PowerShellScriptPluginConfigurationManager
    {
        private readonly AppclusiveEndpoints appclusiveEndpoints;

        public PowerShellScriptPluginConfigurationManager(AppclusiveEndpoints endpoints)
        {
            Contract.Requires(null != endpoints);

            appclusiveEndpoints = endpoints;
        }

        public string GetComputerName()
        {
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            return Environment.MachineName;
        }

        private const string CONFIGURATION_TEMPLATE_NAME = "AppclusiveScheduler-PowerShellSessionConfiguration-{0}";
        public string GetConfigurationNameTemplate()
        {
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            return CONFIGURATION_TEMPLATE_NAME;
        }

        public string GetScriptBase()
        {
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            string scriptBase;

            var scriptBaseFromAppSettings = ConfigurationManager.AppSettings.Get(SchedulerAppSettings.Keys.POWERSHELL_SCRIPT_SCRIPT_BASE);
            if(!string.IsNullOrWhiteSpace(scriptBaseFromAppSettings))
            {
                var scriptBaseWithExpandedEnvironmentVariables = Environment.ExpandEnvironmentVariables(scriptBaseFromAppSettings);
                Contract.Assert(Directory.Exists(scriptBaseWithExpandedEnvironmentVariables));

                scriptBase = scriptBaseWithExpandedEnvironmentVariables;
            }
            else
            {
                var codeBase = this.GetType().Assembly.CodeBase;
                var uri = new UriBuilder(codeBase);
                var unescapedUri = Uri.UnescapeDataString(uri.Path).Replace('/', '\\');
                var scriptBaseFromPluginLocation = Path.GetDirectoryName(unescapedUri);
                Contract.Assert(Directory.Exists(scriptBaseFromPluginLocation));

                scriptBase = scriptBaseFromPluginLocation;
            }

            return scriptBase;
        }

        public ICredentials GetCredentials()
        {
            Contract.Ensures(null != Contract.Result<ICredentials>());

            return CredentialCache.DefaultNetworkCredentials;
        }
    }
}
