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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Public.Plugins;

namespace biz.dfch.CS.Appclusive.Scheduler.Public
{
    public abstract class SchedulerPluginBase : AppclusivePluginBase
        , 
        ISchedulerPlugin
    {
        public override bool Invoke(DictionaryParameters parameters, IInvocationResult jobResult)
        {
            var result = base.Invoke(parameters, jobResult);
            
            Contract.Assert(parameters.ContainsKey("JobId"));

            return result;
        }
    }
}
