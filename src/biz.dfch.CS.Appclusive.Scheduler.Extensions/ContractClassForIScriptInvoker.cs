/**
 * Copyright 2015 d-fens GmbH
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
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Management.Automation;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    [ContractClassFor(typeof(IScriptInvoker))]
    public abstract class ContractClassForIScriptInvoker : IScriptInvoker
    {
        PowerShell IScriptInvoker.Powershell
        {
            get
            {
                Contract.Ensures(null != Contract.Result<PowerShell>());

                return default(PowerShell);
            }
        }

        // disable warning for missing call to dispose as this is only a ContractClass
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            return;
        }

        public List<string> RunPowershell(string pathToScriptFile, Dictionary<string, object> parameters)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(pathToScriptFile));
            Contract.Requires(null != parameters);

            return default(List<string>);
        }

        public bool RunPowershell(string pathToScriptFile, Dictionary<string, object> parameters, ref List<object> scriptResult)
        {
            Contract.Requires(File.Exists(pathToScriptFile));
            Contract.Requires(null != parameters);
            Contract.Requires(null != scriptResult);
            Contract.Ensures(null != scriptResult);

            return default(bool);
        }

        public bool RunPowershell(string pathToScriptFile, Dictionary<string, object> parameters, ref List<string> scriptResult)
        {
            return default(bool);
        }
    }
}
