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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions.Tests
{
    public class TestEnvironmentSection : ConfigurationSection
    {
        public const string SECTION_NAME = "TestEnvironment";

        //<configuration>

        //<!-- Configuration section-handler declaration area. -->
        //  <configSections>
        //      <section 
        //        name="TestEnvironment" 
        //        type="biz.dfch.CS.Appclusive.Scheduler.Core.Tests.TestEnvironmentSection, biz.dfch.CS.Appclusive.Scheduler.Core.Tests" 
        //        allowLocation="true" 
        //        allowDefinition="Everywhere"
        //      />
        //      <!-- Other <section> and <sectionGroup> elements. -->
        //  </configSections>

        //  <!-- Configuration section settings area. -->
        //  <TestEnvironment 
        //      username="."
        //      password="P@ssw0rd"
        //      serverBaseUri="http://www.example.com:9080/activiti-rest/service"
        //  />

        //</configuration>

        public TestEnvironmentSection Load()
        {
            var section = new TestEnvironmentSection();
            try
            {
                section = ConfigurationManager.GetSection(TestEnvironmentSection.SECTION_NAME) as TestEnvironmentSection;
                credential = new NetworkCredential(Username, Password);
            }
            catch
            {
                // do nothing and return default settings
            }
            return section;
        }

        private NetworkCredential credential;
        public NetworkCredential Credential
        {
            get
            {
                return new NetworkCredential(Username, Password);
            }
            set
            {
                credential = value;
            }
        }

        [ConfigurationProperty("applicationName", DefaultValue = "AppclusiveSchedulerPlugin.ExternalWorkflow", IsRequired = false)]
        public string ApplicationName
        {
            get
            {
                return (string) this["applicationName"]; 
            }
            set
            { 
                this["applicationName"] = value; 
            }
        }

        [ConfigurationProperty("serverBaseUri", DefaultValue = "http://www.example.com:9080/activiti-rest/service", IsRequired = false)]
        public Uri ServerBaseUri
        {
            get
            {
                return (Uri) this["serverBaseUri"]; 
            }
            set
            { 
                this["serverBaseUri"] = value; 
            }
        }

        [ConfigurationProperty("password", DefaultValue = "kermit", IsRequired = false)]
        public string Password
        {
            get
            { 
                return (string) this["password"]; 
            }
            set
            { 
                this["password"] = value; 
            }
        }

        [ConfigurationProperty("username", DefaultValue = "kermit", IsRequired = false)]
        public string Username
        {
            get
            { 
                return (string) this["username"]; 
            }
            set
            { 
                this["username"] = value; 
            }
        }
    }
}
