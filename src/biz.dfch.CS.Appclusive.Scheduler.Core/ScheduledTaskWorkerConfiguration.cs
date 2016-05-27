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

using biz.dfch.CS.Appclusive.Scheduler.Public;
using biz.dfch.CS.Utilities.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class ScheduledTaskWorkerConfiguration : BaseDto
    {
        public const int UPDATE_INTERVAL_IN_MINUTES_DEFAULT = 5;
        public const int SERVER_NOT_REACHABLE_RETRIES_DEFAULT = 3;

        [Required]
        public Uri Uri { get; set; }
        [Required]
        public string ManagementUriName { get; set; }
        [Range(0, int.MaxValue)]
        public int UpdateIntervalInMinutes { get; set; }
        [Range(0, int.MaxValue)]
        public int ServerNotReachableRetries { get; set; }

        [ContractInvariantMethod]
        private void ContractInvariantMethod()
        {
            Contract.Invariant(null != Uri);
            Contract.Invariant(!string.IsNullOrWhiteSpace(ManagementUriName));
            Contract.Invariant(0 <= UpdateIntervalInMinutes);
            Contract.Invariant(0 <= ServerNotReachableRetries);
        }

        public ScheduledTaskWorkerConfiguration()
        {
            // N/A
        }

        public ScheduledTaskWorkerConfiguration(IConfigurationLoader loader, string[] args)
        {
            var parameters = new Dictionary<string, object>();
            if(null != args)
            {
                for(var c = 0; c < args.Count(); c++)
                {
                    var arg = args[c];
                    parameters.Add(string.Format("arg{0}", c), arg);
                }
            }
            loader.Initialise(this, parameters);
        }

        public override bool IsValid()
        {
            return base.IsValid();
        }

        public bool Initialise(string[] args)
        {
            try
            {
                UpdateIntervalInMinutes = 
                    (0 != UpdateIntervalInMinutes) ? 
                    UpdateIntervalInMinutes : 
                    ScheduledTaskWorkerConfiguration.UPDATE_INTERVAL_IN_MINUTES_DEFAULT;

                ServerNotReachableRetries = 
                    UpdateIntervalInMinutes * (0 != ServerNotReachableRetries ?
                    ServerNotReachableRetries : 
                    ScheduledTaskWorkerConfiguration.SERVER_NOT_REACHABLE_RETRIES_DEFAULT);

                var uri = string.Empty;
                uri = ConfigurationManager.AppSettings["Uri"];
                if (2 <= args.Length)
                {
                    uri = args[0];
                }
                Contract.Assert(!string.IsNullOrWhiteSpace(uri), "Uri: Parameter validation FAILED.");
                Uri = new Uri(uri);

                ManagementUriName = ConfigurationManager.AppSettings["ManagementUri"];
                if (2 <= args.Length) ManagementUriName = args[1];
                Contract.Assert(!string.IsNullOrWhiteSpace(ManagementUriName), "ManagementUri: Parameter validation FAILED.");

                UpdateIntervalInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["UpdateIntervalMinutes"]);

                ServerNotReachableRetries = Convert.ToInt32(ConfigurationManager.AppSettings["ServerNotReachableRetries"]);

            }
            catch (Exception ex)
            {
                Trace.WriteException(ex.Message, ex);
            }
            
            return default(bool);
        }
    }
}
