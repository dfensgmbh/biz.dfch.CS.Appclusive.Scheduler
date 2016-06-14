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
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using biz.dfch.CS.Appclusive.Public;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    public class ProgrammePluginInvokeParameters : SchedulerPluginBaseInvokeParameters
    {
        [Required]
        public string CommandLine { get; set; }
        [Required]
        public string WorkingDirectory { get; set; }
        [Required]
        public NetworkCredential Credential { get; set; }
    }
}
