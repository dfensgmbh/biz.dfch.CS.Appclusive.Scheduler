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
using System.Net;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using System.ComponentModel.Composition;
using System.Diagnostics;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Public.Plugins;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    [Export(typeof(IAppclusivePlugin))]
    [ExportMetadata("Type", "Programme")]
    [ExportMetadata("Priority", int.MinValue)]
    [ExportMetadata("Role", "default")]
    public class ProgrammePlugin : SchedulerPluginBase
    {
        private DictionaryParameters configuration;
        public override DictionaryParameters Configuration 
        { 
            get
            {
                return configuration;
            }
            set
            {
                configuration = UpdateConfiguration(value);
            }
        }

        private DictionaryParameters UpdateConfiguration(DictionaryParameters parameters)
        {
            Contract.Requires(parameters.IsValid());

            configuration = parameters;
            return configuration;
        }

        public override bool Invoke(DictionaryParameters parameters, IInvocationResult jobResult)
        {
            var fReturn = false;

            if(!IsActive)
            {
                jobResult.Succeeded = fReturn;
                jobResult.Code = 1;
                jobResult.Message = "Plugin inactive";
            }
            
            var invocationParameters = parameters.Convert<ProgrammePluginInvokeParameters>();

            try
            {
                var result = biz.dfch.CS.Utilities.Process.StartProcess(
                    invocationParameters.CommandLine, 
                    invocationParameters.WorkingDirectory, 
                    invocationParameters.Credential);

                fReturn = true;

                jobResult.Succeeded = fReturn;
                jobResult.Code = 0;
                jobResult.Message = invocationParameters.CommandLine;
            }
            catch(Exception ex)
            {
                jobResult.Succeeded = fReturn;
                jobResult.Code = ex.HResult;
                jobResult.Message = ex.Message;
                jobResult.Description = ex.StackTrace;
            }

            return jobResult.Succeeded;
        }
    }
}
