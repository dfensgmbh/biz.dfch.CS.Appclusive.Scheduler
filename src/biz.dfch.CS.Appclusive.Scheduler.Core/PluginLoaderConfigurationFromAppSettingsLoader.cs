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
using System.Configuration;
using biz.dfch.CS.Appclusive.Public;
using biz.dfch.CS.Appclusive.Scheduler.Public;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using biz.dfch.CS.Appclusive.Public.Configuration;
using biz.dfch.CS.Appclusive.Public.Plugins;
using biz.dfch.CS.Utilities.Logging;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class PluginLoaderConfigurationFromAppSettingsLoader : IConfigurationLoader
    {
        public void Initialise(BaseDto configuration, DictionaryParameters parameters)
        {
            Contract.Assert(configuration is PluginLoaderConfiguration);

            var cfg = configuration as PluginLoaderConfiguration;

            // 1. Get folder from where to load extensions
            var extensionsFolder = ConfigurationManager.AppSettings[SchedulerAppSettings.Keys.EXTENSIONS_FOLDER];
            if (!Path.IsPathRooted(extensionsFolder))
            {
                extensionsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, extensionsFolder);
            }
            Contract.Assert(Directory.Exists(extensionsFolder));

            cfg.ExtensionsFolder = extensionsFolder;

            // 2. Load plugin names to be loaded
            var pluginTypes = ConfigurationManager.AppSettings[SchedulerAppSettings.Keys.PLUGIN_TYPES];
            
            if(string.IsNullOrWhiteSpace(pluginTypes))
            {
                pluginTypes = biz.dfch.CS.Appclusive.Scheduler.Public.Constants.PLUGIN_TYPE_DEFAULT;
            }

            var pluginTypesToBeLoaded = pluginTypes
                .ToLower()
                .Split(',')
                .Distinct()
                .OrderBy(p => p)
                .ToList();
            Contract.Assert(0 < pluginTypesToBeLoaded.Count());
            
            if(pluginTypesToBeLoaded.Contains(PluginLoader.LOAD_ALL_PATTERN))
            {
                pluginTypesToBeLoaded.Clear();
                pluginTypesToBeLoaded.Add(PluginLoader.LOAD_ALL_PATTERN);
            }
            cfg.PluginTypes = pluginTypesToBeLoaded;

            Trace.WriteLine("SchedulerPluginLoader. ExtensionsFolder: '{0}'. PluginTypes '{1}'.", cfg.ExtensionsFolder, cfg.PluginTypes);
        }
    }
}
