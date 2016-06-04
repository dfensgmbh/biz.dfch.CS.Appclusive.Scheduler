﻿/**
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

using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace biz.dfch.CS.Appclusive.Scheduler.Public
{
    public class PluginLoaderConfiguration : BaseDto
    {
        [Required]
        public string ExtensionsFolder { get; set; }

        public List<string> PluginTypes { get; set; }

        public List<Assembly> Assemblies { get; set; }

        public PluginLoaderConfiguration()
        {
            // N/A
        }

        public PluginLoaderConfiguration(IConfigurationLoader loader)
        {
            Contract.Requires(null != loader);

            loader.Initialise(this, null);

            return;
        }

        public PluginLoaderConfiguration(Action<BaseDto> loader)
        {
            Contract.Requires(null != loader);

            loader(this);

            return;
        }
    }
}
