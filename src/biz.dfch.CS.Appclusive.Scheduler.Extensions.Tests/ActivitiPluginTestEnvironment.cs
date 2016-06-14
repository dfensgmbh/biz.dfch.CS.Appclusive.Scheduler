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
    public class ActivitiPluginTestEnvironment : ActivitiPluginTestEnvironmentTemplate
    {
        // use this file to insert your connection information and credentials
        // this file is ignored by git, 
        // so any changes you make here will NOT be pushed to the central repository

        public ActivitiPluginTestEnvironment()
        {
            Credential = new NetworkCredential("real-username-here", "real-password-here");
            
            ServerBaseUri = new Uri("http://real-server-here.example.com:9080/real-folder-here/");
        }
    }
}
