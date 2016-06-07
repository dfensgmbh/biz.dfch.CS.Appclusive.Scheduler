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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Scheduler.Public
{
    public class ConfigSectionManager
    {
        private const string PROTECTION_PROVIDER = "DataProtectionConfigurationProvider";

        private readonly string executablePathAndFileName;
        private readonly Configuration configuration;

        public ConfigSectionManager()
        {
            throw new NotImplementedException("Call constructor with exePath parameter");
        }

        public ConfigSectionManager(string exePath)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(exePath));
            Contract.Requires(File.Exists(exePath));
            Contract.Ensures(null != configuration);
            Contract.Ensures(!string.IsNullOrWhiteSpace(executablePathAndFileName));

            executablePathAndFileName = exePath;
            configuration = ConfigurationManager.OpenExeConfiguration(exePath);
        }

        public ConfigurationSection Get(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name), "Invalid section name specified");
            Contract.Ensures(null != Contract.Result<ConfigurationSection>(), "Configuration section not found");

            var configurationSection = configuration.GetSection(name);
            return configurationSection;
        }

        public void Add(string name, ConfigurationSection section)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name), "Invalid section name specified");
            Contract.Requires(null != section, "Invalid configuration section specified");

            var configurationSection = configuration.GetSection(name);
            Contract.Assert(null == configurationSection, "Configuration section already exists");

            configuration.Sections.Add(name, section);

            configuration.Save(ConfigurationSaveMode.Minimal, true);
        }

        public void Remove(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name), "Invalid section name specified");

            var configurationSection = configuration.GetSection(name);
            Contract.Assert(null != configurationSection, "Configuration section not found");

            configuration.Sections.Remove(name);

            configuration.Save(ConfigurationSaveMode.Minimal, true);
        }

        public void Encrypt(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name), "Invalid section name specified");

            var configurationSection = configuration.GetSection(name);
            Contract.Assert(null != configurationSection, "Configuration section not found");
            Contract.Assert(!configurationSection.SectionInformation.IsProtected, "Configuration section is already protected");

            configurationSection.SectionInformation.ProtectSection(PROTECTION_PROVIDER);

            configuration.Save(ConfigurationSaveMode.Minimal, true);
        }

        public void Decrypt(string name)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(name), "Invalid section name specified");

            var configurationSection = configuration.GetSection(name);
            Contract.Assert(null != configurationSection, "Configuration section not found");
            Contract.Assert(configurationSection.SectionInformation.IsProtected, "Configuration section is not protected");

            configurationSection.SectionInformation.UnprotectSection();

            configuration.Save(ConfigurationSaveMode.Minimal, true);
        }
    }
}
