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
        public DictionaryParameters Configuration
        {
            get
            {
                Contract.Ensures(null != Contract.Result<DictionaryParameters>());

                return default(DictionaryParameters);
            }
            set
            {
                Contract.Requires(null != value);
                Contract.Requires(value.IsValid());
            }
        }

        public abstract bool IsInitialised { get; }
        
        public bool IsActive 
        { 
            get
            {
                return default(bool);
            } 
            set
            {
                Contract.Requires(IsInitialised);
            } 
        }

        public abstract ILogger Logger { get; }

        [ContractInvariantMethod]
        private void ContractInvariantMethod()
        {
            Contract.Invariant(null != Logger);
        }

        public bool Initialise(DictionaryParameters parameters, ILogger logger, bool activate)
        {
            Contract.Requires(null != parameters);
            Contract.Requires(null != logger);

            return default(bool);
        }

        public bool Invoke(DictionaryParameters parameters, ref JobResult jobResult)
        {
            Contract.Requires(IsInitialised);
            Contract.Requires(null != parameters);
            Contract.Requires(null != jobResult);
            Contract.Ensures(null != jobResult);
            Contract.Ensures(jobResult.IsValid());

            return default(bool);
        }
    }
}
