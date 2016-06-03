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
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Scheduler.Public
{
    public class AppclusivePluginBase : IAppclusivePlugin
    {
        public virtual DictionaryParameters Configuration { get; set; }

        public virtual bool IsInitialised { get; private set; }

        public virtual bool Initialise(DictionaryParameters parameters, ILogger logger, bool activate)
        {
            Configuration = parameters;
            Logger = logger;
                
            IsInitialised = true;
            IsActive = activate;

            return IsInitialised;
        }

        public virtual bool IsActive { get; set; }

        public virtual ILogger Logger { get; private set; }

        public virtual bool Invoke(DictionaryParameters parameters, IInvocationResult jobResult)
        {
            return default(bool);
        }

        public virtual string GetName()
        {
            return this.GetType().Name;
        }

        public virtual Version GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var version = assembly.GetName().Version;
            return version;
        }
    }
}
