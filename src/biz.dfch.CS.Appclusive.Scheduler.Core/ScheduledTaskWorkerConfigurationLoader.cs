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
using biz.dfch.CS.Appclusive.Public.Configuration;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using biz.dfch.CS.Utilities.Logging;
using System;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Net;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class ScheduledTaskWorkerConfigurationLoader : IConfigurationLoader
    {
        public void Initialise(BaseDto configuration, DictionaryParameters parameters)
        {
            Contract.Assert(configuration is ScheduledTaskWorkerConfiguration);
            
            parameters = parameters ?? new DictionaryParameters();

            var cfg = configuration as ScheduledTaskWorkerConfiguration;

            cfg.UpdateIntervalInMinutes = 
                (0 != cfg.UpdateIntervalInMinutes) ? 
                cfg.UpdateIntervalInMinutes : 
                ScheduledTaskWorkerConfiguration.UPDATE_INTERVAL_IN_MINUTES_DEFAULT;

            cfg.ServerNotReachableRetries = 
                cfg.UpdateIntervalInMinutes * (0 != cfg.ServerNotReachableRetries ?
                cfg.ServerNotReachableRetries : 
                ScheduledTaskWorkerConfiguration.SERVER_NOT_REACHABLE_RETRIES_DEFAULT);

            var uri = ConfigurationManager.AppSettings["Uri"];
            if(parameters.ContainsKey("args0"))
            {
                uri = parameters["args0"] as string;
            }
            Contract.Assert(!string.IsNullOrWhiteSpace(uri));
            cfg.Uri = new Uri(uri);

            cfg.ManagementUriName = ConfigurationManager.AppSettings["ManagementUri"];
            if(parameters.ContainsKey("args1"))
            {
                cfg.ManagementUriName = parameters["args1"] as string;
            }
            Contract.Assert(!string.IsNullOrWhiteSpace(cfg.ManagementUriName));

            cfg.UpdateIntervalInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["UpdateIntervalMinutes"]);

            cfg.ServerNotReachableRetries = Convert.ToInt32(ConfigurationManager.AppSettings["ServerNotReachableRetries"]);

            var credentialSection = ConfigurationManager.GetSection(AppclusiveCredentialSection.SECTION_NAME) as AppclusiveCredentialSection;
            if(null == credentialSection)
            {
                Trace.WriteLine("No credential in app.config section '{0}' defined. Using 'DefaultNetworkCredentials'.", AppclusiveCredentialSection.SECTION_NAME, "");
                
                cfg.Credential = CredentialCache.DefaultNetworkCredentials;
            }
            else
            {
                Trace.WriteLine("Credential in app.config section '{0}' found. Using '{1}\\{2}'.", AppclusiveCredentialSection.SECTION_NAME, credentialSection.Domain, credentialSection.Username);

                var networkCredential = new NetworkCredential(credentialSection.Username, credentialSection.Password, credentialSection.Domain);
                Contract.Assert(null != networkCredential);
                cfg.Credential = networkCredential;
            }
        }
    }

}
