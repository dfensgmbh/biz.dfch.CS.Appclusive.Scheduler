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

namespace biz.dfch.CS.Appclusive.Scheduler.Public
{
    [ContractClassFor(typeof(ISchedulerPlugin))]
    abstract class ContractClassForISchedulerPlugin : ISchedulerPlugin
    {
        public SchedulerPluginParameters Configuration
        {
            get
            {
                Contract.Ensures(null != Contract.Result<SchedulerPluginParameters>());

                return default(SchedulerPluginParameters);
            }
            set
            {
                Contract.Requires(null != value);
            }
        }

        void ISchedulerPlugin.Log(string message)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(message));

            return;
        }

        public bool UpdateConfiguration(SchedulerPluginParameters configuration)
        {
            Contract.Requires(null != configuration);

            return default(bool);
        }

        public bool Invoke(SchedulerPluginParameters parameters, ref JobResult jobResult)
        {
            Contract.Requires(null != parameters);
            Contract.Requires(null != jobResult);
            Contract.Ensures(null != jobResult);

            return default(bool);
        }
    }
}
