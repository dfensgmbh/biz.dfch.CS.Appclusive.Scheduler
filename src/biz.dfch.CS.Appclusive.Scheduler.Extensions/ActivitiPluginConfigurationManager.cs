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
using System.Configuration;
using System.Data.Services.Client;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Appclusive.Api.Core;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    public class ActivitiPluginConfigurationManager
    {
        public string GetManagementUriName()
        {
            Contract.Ensures(!string.IsNullOrWhiteSpace(Contract.Result<string>()));

            var managementUriName = SchedulerAppSettings.Keys.EXTERNAL_WORKFLOW_MANAGEMENT_URI_NAME;
            
            var managementUriValue = ConfigurationManager.AppSettings[managementUriName];
            return managementUriValue;
        }

        public ManagementUri GetManagementUri(DataServiceQuery<ManagementUri> managementUris, string name)
        {
            Contract.Requires(null != managementUris);
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Ensures(null != Contract.Result<ManagementUri>());
            Contract.Ensures(Contract.Result<ManagementUri>().ManagementCredentialId.HasValue);

            var result = managementUris
                .Where(e => e.Name == name)
                .FirstOrDefault();
            
            return result;
        }

        public ManagementCredential GetManagementCredential(DataServiceQuery<ManagementCredential> managementCredentials, long id)
        {
            Contract.Requires(null != managementCredentials);
            Contract.Requires(0 < id);
            Contract.Ensures(null != Contract.Result<ManagementCredential>());

            var result = managementCredentials
                .Where(e => e.Id == id)
                .FirstOrDefault();
            
            return result;
        }

        public NetworkCredential GetCredential(ManagementCredential managementCredential)
        {
            Contract.Requires(null != managementCredential);
            Contract.Requires(managementCredential.Password != managementCredential.EncryptedPassword);
            Contract.Ensures(null != Contract.Result<NetworkCredential>());

            var credential = new NetworkCredential(managementCredential.Username, managementCredential.Password);
            return credential;
        }
    }
}
