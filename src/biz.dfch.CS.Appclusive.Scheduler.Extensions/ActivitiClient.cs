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
using biz.dfch.CS.Activiti.Client;
using biz.dfch.CS.Appclusive.Public;
using System.Collections;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    public class ActivitiClient : ProcessEngine
    {
        public ActivitiClient(Uri server, string applicationName = "", int timeoutSec = 0)
            : base(server, applicationName, timeoutSec)
        {
            // N/A
        }

        public new bool Login(NetworkCredential credential)
        {
            base.Login(credential);

            var result = base.IsLoggedIn();
            return result;
        }

        public new bool Login(string username, string password)
        {
            base.Login(username, password);

            var result = base.IsLoggedIn();
            return result;
        }

        public string GetDefinitionId(string definitionKey)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(definitionKey));
            Contract.Ensures(null != Contract.Result<string>(), "Workflow definitionKey does not exist");

            var processDefinitionResponse = GetWorkflowDefinitionByKey(definitionKey, true);
            Contract.Assert(null != processDefinitionResponse, "Workflow definitionKey does not exist");

            var processDefinitionResponseData = processDefinitionResponse.data.FirstOrDefault();
            Contract.Assert(null != processDefinitionResponseData, "Workflow definitionKey cannot be resolved to definitionId");

            var definitionId = processDefinitionResponseData.id;
            return definitionId;
        }
    }
}
