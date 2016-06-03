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
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Scheduler.Public
{
    public class PluginLoader
    {
        public const string LOAD_ALL_PATTERN = "*";

        private PluginLoaderConfiguration configuration;

        public bool IsInitialised { get; private set; }

        [ImportMany]
        private IEnumerable<Lazy<IAppclusivePlugin, IAppclusivePluginData>> pluginsAvailable;

        public PluginLoader()
        {
            // N/A
        }

        public PluginLoader(IConfigurationLoader loader)
        {
            configuration = new PluginLoaderConfiguration(loader);
        }

        public void Initialise(IConfigurationLoader loader)
        {
            Contract.Requires(null != loader);

            configuration = new PluginLoaderConfiguration(loader);
            
            Initialise();
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Initialise()
        {
            Contract.Assert(null != configuration);

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
                var message = string.Format("AppSettings.ExtensionsFolder: Loading extensions from '{0}' FAILED.", 
                        configuration.ExtensionsFolder);
                Trace.WriteLine(string.Format("{0}@{1}: '{2}'\r\n[{3}]\r\n{4}", 
                    ex.GetType().Name, ex.Source, message, ex.Message, ex.StackTrace));
            }
            finally
            {
                // Adds all the parts found in the assembly that called this class
                catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetCallingAssembly()));
            }

            // Add all other assemblies defined in configuration
            if(null != configuration.Assemblies)
            {
                foreach(var assembly in configuration.Assemblies)
                {
                    try
                    {
                        catalog.Catalogs.Add(new AssemblyCatalog(assembly));
                    }
                    catch(Exception ex)
                    {
                        var message = string.Format("Adding assembly '{0}' to catalogue FAILED.",
                            assembly.FullName);
                        Trace.WriteLine(string.Format("{0}@{1}: '{2}'\r\n[{3}]\r\n{4}", 
                            ex.GetType().Name, ex.Source, message, ex.Message, ex.StackTrace));
                    }
                }
            }

            // Create the CompositionContainer with the parts in the catalog
            var container = new CompositionContainer(catalog);
            try
            {
                // Fill the imports of this object
                container.ComposeParts(this);
            }
            catch (CompositionException ex)
            {
                var message = "ComposeParts FAILED";
                Trace.WriteLine(string.Format("{0}@{1}: '{2}'\r\n[{3}]\r\n{4}", 
                    ex.GetType().Name, ex.Source, message, ex.Message, ex.StackTrace));
                throw;
            }

            IsInitialised = true;
        }

        public List<Lazy<IAppclusivePlugin, IAppclusivePluginData>> Load()
        {
            Contract.Requires(IsInitialised);
            Contract.Ensures(0 < Contract.Result<List<Lazy<IAppclusivePlugin, IAppclusivePluginData>>>().Count);

            var plugins = new List<Lazy<IAppclusivePlugin, IAppclusivePluginData>>();
            foreach(var plugin in pluginsAvailable.OrderByDescending(p => p.Metadata.Priority))
            {
                var isPluginToBeAdded =
                        (
                            configuration.PluginTypes.Contains(LOAD_ALL_PATTERN)
                            ||
                            configuration.PluginTypes.Contains(plugin.Metadata.Type, StringComparer.InvariantCultureIgnoreCase)
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

        public List<Lazy<IAppclusivePlugin, IAppclusivePluginData>> InitialiseAndLoad(IConfigurationLoader loader)
        {
            Contract.Requires(null != loader);

            configuration = new PluginLoaderConfiguration(loader);
            
            var result = InitialiseAndLoad();
            return result;
        }

        public List<Lazy<IAppclusivePlugin, IAppclusivePluginData>> InitialiseAndLoad()
        {
            Initialise();

            var result = Load();
            return result;
        }
    }
}
