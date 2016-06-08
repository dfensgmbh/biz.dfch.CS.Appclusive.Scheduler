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

using biz.dfch.CS.Appclusive.Scheduler.Public;
using System.Diagnostics.Contracts;
using System.Reflection;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class AppclusiveCredentialSectionManager
    {
        private readonly AppclusiveCredentialSection section = new AppclusiveCredentialSection();

        private readonly string schedulerPathAndFile;

        public AppclusiveCredentialSectionManager()
        {
            schedulerPathAndFile = Assembly.GetExecutingAssembly().Location;
        }

        public AppclusiveCredentialSectionManager(string username, string password, string domain)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(username));
            Contract.Requires(!string.IsNullOrWhiteSpace(password));
            Contract.Requires(!string.IsNullOrWhiteSpace(domain));

            section.Username = username;
            section.Password = password;
            section.Domain = domain;

            schedulerPathAndFile = Assembly.GetExecutingAssembly().Location;
        }

        public AppclusiveCredentialSection GetSection()
        {
            Contract.Ensures(null != Contract.Result<AppclusiveCredentialSection>());

            var result = GetSection(AppclusiveCredentialSection.SECTION_NAME);
            return result;
        }

        public AppclusiveCredentialSection GetSection(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name));
            Contract.Ensures(null != Contract.Result<AppclusiveCredentialSection>());

            var configSectionManager = new ConfigSectionManager(schedulerPathAndFile);
            
            var result = configSectionManager.Get(AppclusiveCredentialSection.SECTION_NAME);
            return result as AppclusiveCredentialSection;
        }

        public void Decrypt()
        {
            var configSectionManager = new ConfigSectionManager(schedulerPathAndFile);

            configSectionManager.Decrypt(AppclusiveCredentialSection.SECTION_NAME);
        }

        public void Encrypt()
        {
            var configSectionManager = new ConfigSectionManager(schedulerPathAndFile);

            configSectionManager.Encrypt(AppclusiveCredentialSection.SECTION_NAME);
        }
    }
}
