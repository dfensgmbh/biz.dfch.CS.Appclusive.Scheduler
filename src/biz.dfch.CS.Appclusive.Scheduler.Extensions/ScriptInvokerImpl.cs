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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Management.Automation;
using biz.dfch.CS.Appclusive.Public.Plugins;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    public class ScriptInvokerImpl : IScriptInvoker
    {
        internal ScriptInvokerImpl()
        {
            Powershell = PowerShell.Create();
        }

        internal ScriptInvokerImpl(PowerShell powershell)
        {
            Powershell = powershell;
        }

        private PowerShell powershell;
        public PowerShell Powershell 
        { 
            get
            {
                // no need for contract check as this is covered by interface contract
                return powershell;
            }
            private set
            {
                Contract.Requires(null != value);

                powershell = value;
            }
        }

        public IAppclusivePluginLogger Logger { get; set; }

        public List<string> RunPowershell(string pathToScriptFile, Dictionary<string, object> parameters)
        {
            try
            {
                Contract.Assert(File.Exists(pathToScriptFile));

                var scriptResult = new List<string>();
                Powershell.Commands.Clear();
                Powershell
                    .AddCommand(pathToScriptFile)
                    .AddParameters(parameters)
                ;
                var scriptInvokerImplPowerShellResult = Powershell.Invoke();
                Contract.Assert(null != scriptInvokerImplPowerShellResult);

                foreach (var result in scriptInvokerImplPowerShellResult)
                {
                    // DFTODO - add check if result is null
                    var line = result.BaseObject.ToString();
                    scriptResult.Add(line);
                }

                return scriptResult;
            }
            catch (Exception ex)
            {
                var message = "";
                Logger.Critical(string.Format("{0}@{1}: '{2}'\r\n[{3}]\r\n{4}", ex.GetType().Name, ex.Source, message, ex.Message, ex.StackTrace));

                return null;
            }
        }

        // Note: this overload of RunPowerShell has different semantics than the original implementation.
        // i.e. it does NOT return null in case of an exception, but returns true/false
        public bool RunPowershell(string pathToScriptFile, Dictionary<string, object> parameters, ref List<object> scriptResult)
        {
            var fReturn = false;

            try
            {
                // Host.InstanceId is only available AFTER calling Invoke()
                // so we prepopulate it with an empty guid
                var hostInstanceId = Guid.Empty.ToString();

                Logger.Info(string.Format("[Host.InstanceId: {0}] {1}", hostInstanceId, pathToScriptFile));

                var scriptInvokerImplPowerShellResult = Powershell
                    .AddCommand(pathToScriptFile)
                    .AddParameters(parameters)
                    .Invoke();

                try
                {
                    // the type of 'Host' is 'InternalHost' which is an internal data type
                    // therefore we use a dynamic data type
                    dynamic host = Powershell.Runspace.SessionStateProxy.GetVariable("Host");
                    if (null != host && null != host.InstanceId)
                    {
                        hostInstanceId = host.InstanceId.ToString();
                    }
                }
                catch
                {
                    // intentionally ignore exception
                }

                if(Powershell.HadErrors)
                {
                    var errorMessage = new System.Text.StringBuilder();
                    errorMessage.AppendLine(string.Format("[Host.InstanceId: {0}] {1}", hostInstanceId, pathToScriptFile));

                    var error = Powershell.Runspace.SessionStateProxy.GetVariable("error") as ArrayList;
                    if(null != error)
                    {
                        for (var c = 0; c < error.Count; c++ )
                        {
                            var errorRecord = error[c];
                            errorMessage.AppendFormat("$Error[{0}]: ", c);
                            errorMessage.AppendLine(errorRecord.ToString());
                        }
                    }

                    Logger.Error(errorMessage.ToString());
                    return fReturn;
                }

                if(null == scriptInvokerImplPowerShellResult)
                {
                    Logger.Error("[Host.InstanceId: {0}] {1}", hostInstanceId, pathToScriptFile);
                    return fReturn;
                }

                foreach (var result in scriptInvokerImplPowerShellResult)
                {
                    if(null == result)
                    {
                        scriptResult.Add(default(object));
                    }
                    else
                    {
                        var item = result.BaseObject;
                        scriptResult.Add(item);
                    }
                }

                if(1 == scriptResult.Count && null == scriptResult[0])
                {
                    scriptResult.Clear();
                }

                Logger.Info(string.Format("[Host.InstanceId: {0}] {1}", hostInstanceId, pathToScriptFile));

                fReturn = true;
            }
            catch(TypeInitializationException ex)
            {
                Exception relevantEx = ex.InnerException ?? (ex as Exception);
                var message = ex.Message;
                Logger.Critical(string.Format("{0}@{1}: '{2}'\r\n[{3}]\r\n{4}", relevantEx.GetType().Name, relevantEx.Source, message, relevantEx.Message, relevantEx.StackTrace));
            }
            catch (Exception ex)
            {
                var message = "";
                Logger.Critical(string.Format("{0}@{1}: '{2}'\r\n[{3}]\r\n{4}", ex.GetType().Name, ex.Source, message, ex.Message, ex.StackTrace));
            }
            
            return fReturn;
        }

        public bool RunPowershell(string pathToScriptFile, Dictionary<string, object> parameters, ref List<string> scriptResult)
        {
            var result = RunPowershell(pathToScriptFile, parameters, ref scriptResult);
            if(!result)
            {
                return result;
            }

            for(var c = 0; c < scriptResult.Count; c++)
            {
                scriptResult[c] = scriptResult[c].ToString();
            }

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
                Powershell.Dispose();
            }

            isDisposed = true;

            return;
        }
        
        ~ScriptInvokerImpl()
        {
            Dispose(false);
        }

        #endregion
   }
}
