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

using System.Net;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Public.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Appclusive.Public.Plugins;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class ScheduledJobsWorkerConfiguration : BaseDto
    {
        public const int UPDATE_INTERVAL_IN_MINUTES_DEFAULT = 5;
        public const int SERVER_NOT_REACHABLE_RETRIES_DEFAULT = 60;

        [Required]
        public Uri Uri { get; set; }

        [Required]
        public string ManagementUriName { get; set; }

        private IAppclusivePluginLogger logger = new Logger();
        public IAppclusivePluginLogger Logger
        {
            get
            {
                return logger;
            }
            set
            {
                logger = value;
            }
        }
        private List<Lazy<IAppclusivePlugin, IAppclusivePluginData>> plugins = new List<Lazy<IAppclusivePlugin, IAppclusivePluginData>>();
        public List<Lazy<IAppclusivePlugin, IAppclusivePluginData>> Plugins
        {
            get
            {
                return plugins;
            }
            set
            {
                plugins = value;
            }
        }

        private string managementUriType = "biz.dfch.CS.Appclusive.Scheduler";
        public string ManagementUriType 
        { 
            get
            {
                return managementUriType;
            }
            private set
            {
                managementUriType = value;
            }
        }
        
        [Range(0, int.MaxValue)]
        public int UpdateIntervalInMinutes { get; set; }
        
        [Range(0, int.MaxValue)]
        public int ServerNotReachableRetries { get; set; }

        public NetworkCredential Credential { get; set; }

        [ContractInvariantMethod]
        private void ContractInvariantMethod()
        {
            Contract.Invariant(null != Uri);
            Contract.Invariant(null != Logger);
            Contract.Invariant(null != Plugins);
            Contract.Invariant(!string.IsNullOrWhiteSpace(ManagementUriName));
            Contract.Invariant(0 <= UpdateIntervalInMinutes);
            Contract.Invariant(0 <= ServerNotReachableRetries);
        }

        public ScheduledJobsWorkerConfiguration()
        {
            // N/A
        }

        public ScheduledJobsWorkerConfiguration(IConfigurationLoader loader, string[] args)
        {
            var parameters = new DictionaryParameters();
            if(null != args)
            {
                for(var c = 0; c < args.Count(); c++)
                {
                    var arg = args[c];
                    parameters.Add(string.Format("args{0}", c), arg);
                }
            }

            loader.Initialise(this, parameters);

            return;
        }
    }
}
