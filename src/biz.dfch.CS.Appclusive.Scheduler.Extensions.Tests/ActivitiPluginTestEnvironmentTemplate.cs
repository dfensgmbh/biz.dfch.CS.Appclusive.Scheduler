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

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions.Tests
{
    public class ActivitiPluginTestEnvironmentTemplate
    {
        private string applicationName = "AppclusiveSchedulerPlugin.ExternalWorkflow";
        public virtual string ApplicationName
        {
            get { return applicationName; }
            set { applicationName = value; }
        }

        private NetworkCredential credential = new NetworkCredential("kermit", "kermit");
        public virtual NetworkCredential Credential
        {
            get { return credential; }
            set { credential = value; }
        }

        private Uri serverBaseUri = new Uri("http://www.example.com:9080/arbitrary-folder/");
        public virtual Uri ServerBaseUri
        {
            get { return serverBaseUri; }
            set { serverBaseUri = value; }
        }
    }
}
