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
using System.Net;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using System.ComponentModel.Composition;
using System.Diagnostics;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    [Export(typeof(ISchedulerPlugin))]
    [ExportMetadata("Type", "Programme")]
    [ExportMetadata("Priority", int.MaxValue)]
    public class ProgrammePlugin : ISchedulerPlugin
    {
        public Dictionary<string, object> Configuration
        {
            get
            {
                // TODO: Implement this property getter
                throw new NotImplementedException();
            }
            set
            {
                // TODO: Implement this property setter
                throw new NotImplementedException();
            }
        }

        public void Log(string message)
        {
            Trace.WriteLine(message);
        }

        public bool UpdateConfiguration(Dictionary<string, object> configuration)
        {
            // TODO: Implement this method
            throw new NotImplementedException();
        }

        public bool Invoke(Dictionary<string, object> data, ref JobResult jobResult)
        {
            Contract.Requires(data.ContainsKey("CommandLine"));
            Contract.Requires(data.ContainsKey("WorkingDirectory"));
            Contract.Requires(data.ContainsKey("Credential"));

            Contract.Assert(data["CommandLine"] is string);
            var commandLine = data["CommandLine"] as string;

            Contract.Assert(data["WorkingDirectory"] is string);
            var workingDirectory = data["WorkingDirectory"] as string;
            
            Contract.Assert(data["Credential"] is NetworkCredential);
            var credential = data["Credential"] as NetworkCredential;

            try
            {
                var result = biz.dfch.CS.Utilities.Process.StartProcess(commandLine, workingDirectory, credential);
                jobResult.Succeeded = true;
                jobResult.Code = 0;
            }
            catch(Exception ex)
            {
                jobResult.Succeeded = false;
                jobResult.Code = ex.HResult;
                jobResult.Message = ex.Message;
                jobResult.Description = ex.StackTrace;
            }

            return jobResult.Succeeded;
        }
    }
}
