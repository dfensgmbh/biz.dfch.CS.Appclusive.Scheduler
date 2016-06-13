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
using biz.dfch.CS.Appclusive.Api;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Public.Plugins;
using System.ComponentModel.DataAnnotations;
using System.Configuration;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    [Export(typeof(IAppclusivePlugin))]
    [ExportMetadata("Type", "ExternalWorkflow")]
    [ExportMetadata("Priority", int.MinValue)]
    [ExportMetadata("Role", "default")]
    public class ActivitiPlugin : SchedulerPluginBase
    {
        private string managementUriName { get; set;}

        private ActivitiPluginConfiguration configuration;
        public override DictionaryParameters Configuration 
        { 
            get
            {
                return new DictionaryParameters().Convert(configuration);
            }
            set
            {
                var result = UpdateConfiguration(value);
                configuration = result;
            }
        }

        private ActivitiPluginConfiguration UpdateConfiguration(DictionaryParameters parameters)
        {
            Contract.Requires(parameters.IsValid());

            // check for endpoint
            var hasAppclusiveEndpointsKey = parameters.ContainsKey(typeof(AppclusiveEndpoints).ToString());
            Contract.Assert(hasAppclusiveEndpointsKey);
            
            var endpoints = parameters.First(p => p.Key == typeof(AppclusiveEndpoints).ToString())
                .Value as AppclusiveEndpoints;
            Contract.Assert(null != endpoints);
            parameters.Remove(typeof(AppclusiveEndpoints).ToString());
            parameters.Add("Endpoints", endpoints);

            // ManagementUri
            //Type                   : json
            //Value                  : {"ServerUri":"http://172.19.115.38:9080/activiti-rest/service/"}
            //ManagementCredentialId : 6
            //Id                     : 9
            //Tid                    : ad8f50df-2a5d-4ea5-9fcc-05882f16a9fe
            //Name                   : biz.dfch.PS.Activiti.Client.Setting
            //Description            :
            //CreatedById            : 1
            //ModifiedById           : 1
            //Created                : 12/15/2015 11:46:45 AM +01:00
            //Modified               : 12/15/2015 11:46:45 AM +01:00
            //RowVersion             : {0, 0, 0, 0...}
            //ManagementCredential   :
            //Tenant                 :
            //CreatedBy              :
            //ModifiedBy             :

            var configurationManager = new ActivitiPluginConfigurationManager();

            // get Activiti server
            var managementUri = configurationManager
                .GetManagementUri(endpoints.Core.ManagementUris, managementUriName);
            
            var activitiPluginConfigurationManagementUri = BaseDto
                .DeserializeObject<ActivitiPluginConfigurationManagementUri>(managementUri.Value);
            parameters.Add("ServerBaseUri", activitiPluginConfigurationManagementUri.ServerUri);

            // get Activiti credentials
            var managementCredential = configurationManager
            .GetManagementCredential(endpoints.Core.ManagementCredentials, managementUri.ManagementCredentialId.Value);

            var networkCredential = configurationManager.GetCredential(managementCredential);
            parameters.Add("Credential", networkCredential);

            configuration = parameters.Convert<ActivitiPluginConfiguration>();
            Contract.Assert(configuration.IsValid());

            return configuration;
        }

        public override bool Initialise(DictionaryParameters parameters, IAppclusivePluginLogger logger, bool activate)
        {
            var result = false;

            // get name of ManagementUri
            managementUriName = new ActivitiPluginConfigurationManager().GetManagementUriName();
                
            Configuration = parameters;
            if(configuration.IsValid())
            {
                return result;
            }

            result = base.Initialise(parameters, logger, activate);
            return result;
        }

        public override bool Invoke(DictionaryParameters parameters, IInvocationResult jobResult)
        {
            var fReturn = false;

            if(!IsActive)
            {
                var message = "Plugin inactive";
                var description = string.Format("ExternalWorkflow: ActivityId '{0}'.", Trace.CorrelationManager.ActivityId.ToString());

                Logger.Warn("{0} {1}. Nothing to do.", description, message);

                jobResult.Succeeded = fReturn;
                jobResult.Code = 1;
                jobResult.Description = description;
                jobResult.Message = message;
            }
            
            var invocationParameters = parameters.Convert<ActivitiPluginInvokeParameters>();

            try
            {
                Logger.Debug("ExternalWorkflow: ActivityId '{0}'. Invoke ...", Trace.CorrelationManager.ActivityId.ToString(), "");

                var result = invocationParameters.JobId;
                fReturn = true;

                var message = string.Format("JobId: '{0}'", invocationParameters.JobId);
                var description = string.Format("ExternalWorkflow: ActivityId '{0}'.", Trace.CorrelationManager.ActivityId.ToString());

                if (fReturn)
                {
                    Logger.Info("ExternalWorkflow: ActivityId '{0}'. Invoke SUCCEEDED.", Trace.CorrelationManager.ActivityId.ToString(), "");
                    jobResult.Code = 0;
                }
                else
                {
                    Logger.Error("ExternalWorkflow: ActivityId '{0}'. Invoke FAILED.", Trace.CorrelationManager.ActivityId.ToString(), "");
                    jobResult.Code = 1;
                }

                jobResult.Succeeded = fReturn;
                jobResult.Description = description;
                jobResult.Message = message;
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
