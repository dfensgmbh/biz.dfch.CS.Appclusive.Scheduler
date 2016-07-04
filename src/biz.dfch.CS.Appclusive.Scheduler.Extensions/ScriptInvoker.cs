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
using System.Linq;
using System.Management.Automation;
using biz.dfch.CS.Appclusive.Public.Plugins;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    public class ScriptInvoker : IScriptInvoker
    {
        private readonly ScriptInvokerImpl scriptInvokerImpl;

        public PowerShell Powershell
        {
            get
            {
                return scriptInvokerImpl.Powershell;
            }
        }

        public ScriptInvoker(IAppclusivePluginLogger logger)
        {
            scriptInvokerImpl = new ScriptInvokerImpl();
            scriptInvokerImpl.Logger = logger;
        }

        public List<string> RunPowershell(string pathToScriptFile, Dictionary<string, object> parameters)
        {
            return scriptInvokerImpl.RunPowershell(pathToScriptFile, parameters);
        }

        public bool RunPowershell(string pathToScriptFile, Dictionary<string, object> parameters, ref List<object> scriptResult)
        {
            var result = scriptInvokerImpl.RunPowershell(pathToScriptFile, parameters, ref scriptResult);
            return result;
        }

        public bool RunPowershell(string pathToScriptFile, Dictionary<string, object> parameters, ref List<string> scriptResult)
        {
            var result = scriptInvokerImpl.RunPowershell(pathToScriptFile, parameters, ref scriptResult);
            return result;
        }

        #region === IDisposable implementation

        private bool isDisposed;
 
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if(disposing)
            {
                this.scriptInvokerImpl.Dispose();
            }

            isDisposed = true;

            return;
        }
        

        ~ScriptInvoker()
        {
            Dispose(false);
        }

        #endregion
    }
}
