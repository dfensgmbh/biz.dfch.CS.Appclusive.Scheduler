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
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Scheduler.Public
{
    public class AppclusiveCredentialSection : ConfigurationSection
    {
        public const string SECTION_NAME = "AppclusiveCredential";

        //<configuration>

        //<!-- Configuration section-handler declaration area. -->
        //  <configSections>
        //      <section 
        //        name="AppclusiveCredential" 
        //        type="biz.dfch.CS.Appclusive.Scheduler.Public.AppclusiveCredentialSection, biz.dfch.CS.Appclusive.Scheduler.Public" 
        //        allowLocation="true" 
        //        allowDefinition="Everywhere"
        //      />
        //      <!-- Other <section> and <sectionGroup> elements. -->
        //  </configSections>

        //  <!-- Configuration section settings area. -->
        //  <AppclusiveCredential 
        //      username="Administrator"
        //      password="P@ssw0rd"
        //      domain="."
        //  >
        //  </AppclusiveCredential>

        //</configuration>

        [ConfigurationProperty("username", DefaultValue = "", IsRequired = false)]
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

        [ConfigurationProperty("password", DefaultValue = "", IsRequired = false)]
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

        [ConfigurationProperty("domain", DefaultValue = ".", IsRequired = false)]
        public string Domain
        {
            get
            { 
                return (string) this["domain"]; 
            }
            set
            { 
                this["domain"] = value; 
            }
        }
    }
}
