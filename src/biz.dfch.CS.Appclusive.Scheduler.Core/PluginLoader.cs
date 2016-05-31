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
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Utilities.Logging;
using biz.dfch.CS.Appclusive.Scheduler.Public;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class PluginLoader
    {
        private readonly PluginLoaderConfiguration configuration;

        public bool IsInitialised { get; private set; }

        [ImportMany]
        private IEnumerable<Lazy<ISchedulerPlugin, ISchedulerPluginData>> pluginsAvailable;

        public PluginLoader()
        {
            var loader = new PluginLoaderConfigurationFromAppSettingsLoader();
            configuration = new PluginLoaderConfiguration(loader);
        }

        public PluginLoader(IConfigurationLoader loader)
        {
            configuration = new PluginLoaderConfiguration(loader);
        }

        public void Initialise()
        {
            IsInitialised = false;

            // An aggregate catalog that combines multiple catalogs
            var catalog = new AggregateCatalog();

            try
            {
                // Adds all the parts found in the given directory
                catalog.Catalogs.Add(new DirectoryCatalog(configuration.ExtensionsFolder));
            }
            catch (Exception ex)
            {
                Trace.WriteException(
                    string.Format("AppSettings.ExtensionsFolder: Loading extensions from '{0}' FAILED.", 
                        configuration.ExtensionsFolder)
                    , ex);
            }
            finally
            {
                // Adds all the parts found in the same assembly as this class
                catalog.Catalogs.Add(new AssemblyCatalog(typeof(PluginLoader).Assembly));
            }

            // Create the CompositionContainer with the parts in the catalog
            var container = new CompositionContainer(catalog);
            try
            {
                // Fill the imports of this object
                container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Trace.WriteException("ComposeParts FAILED", compositionException);
                throw;
            }

            IsInitialised = true;
        }

        public List<Lazy<ISchedulerPlugin, ISchedulerPluginData>> Load()
        {
            Contract.Requires(IsInitialised);
            Contract.Ensures(0 < Contract.Result<List<Lazy<ISchedulerPlugin, ISchedulerPluginData>>>().Count);

            var plugins = new List<Lazy<ISchedulerPlugin, ISchedulerPluginData>>();
            foreach(var plugin in pluginsAvailable.OrderByDescending(p => p.Metadata.Priority))
            {
                var isPluginToBeAdded =
                        (
                            configuration.PluginTypes.Contains("*")
                            ||
                            configuration.PluginTypes.Contains(plugin.Metadata.Type.ToLower())
                        )
                        &&
                        (
                            !plugins.Contains(plugin)
                        );

                if(!isPluginToBeAdded)
                {
                    continue;
                }
                
                plugins.Add(plugin);
            }

            return plugins;
        }
    }
}
