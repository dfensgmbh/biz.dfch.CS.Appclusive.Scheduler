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
    [ContractClass(typeof(ContractClassForISchedulerPlugin))]
    public interface ISchedulerPlugin
    {
        // this property holds the configuration needed by 
        // the plugin
        // an update (set) operation on this property must 
        // be treated as a refresh operation of the 
        // configuration settings
        DictionaryParameters Configuration { get; set; }

        // this flag must be set to true by the plugin after 
        // the Initialise method has been called an if initialisation 
        // has been successful
        bool IsInitialised { get; }

        // the host of the plugin must call this method to initialise 
        // the plugin before the first use of Invoke
        // parameters and logger
        bool Initialise(DictionaryParameters parameters, ILogger logger, bool activate);

        // toggling this flag will enable or disable
        // the plugin
        // the plugin must only operate when this flag
        // is set to true
        bool IsActive { get; set; }
        
        // the plugin can use this interface to log messages
        // the plugin can rely on this property to be set
        // after the plugin has been initialised
        ILogger Logger { get; }

        // the main method of the plugin 
        // will be used by the host of the plugin to perform 
        // operations via the plugin
        bool Invoke(DictionaryParameters parameters, ref JobResult jobResult);
    }
}
