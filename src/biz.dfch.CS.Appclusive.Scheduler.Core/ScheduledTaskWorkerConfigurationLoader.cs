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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using System.Configuration;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class ScheduledTaskWorkerConfigurationLoader : IConfigurationLoader
    {
        public void Initialise(BaseDto configuration, DictionaryParameters parameters)
        {
            Contract.Requires(configuration is ScheduledTaskWorkerConfiguration);
            
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
            if(parameters.ContainsKey("args1"))
            {
                uri = parameters["args0"] as string;
            }
            Contract.Assert(!string.IsNullOrWhiteSpace(uri));
            cfg.Uri = new Uri(uri);

            cfg.ManagementUriName = ConfigurationManager.AppSettings["ManagementUri"];
            if(parameters.ContainsKey("args0"))
            {
                cfg.ManagementUriName = parameters["args1"] as string;
            }
            Contract.Assert(!string.IsNullOrWhiteSpace(cfg.ManagementUriName));

            cfg.UpdateIntervalInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["UpdateIntervalMinutes"]);

            cfg.ServerNotReachableRetries = Convert.ToInt32(ConfigurationManager.AppSettings["ServerNotReachableRetries"]);
        }
    }

}
