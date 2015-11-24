/**
 * Copyright 2011-2015 d-fens GmbH
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
using System.Diagnostics.Contracts;
using System.Linq;
// Install-Package Microsoft.Net.Http
// https://www.nuget.org/packages/Microsoft.Net.Http
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using biz.dfch.CS.Utilities.Logging;
using biz.dfch.CS.Appclusive.Api.Core;
using biz.dfch.CS.Appclusive.Api.Diagnostics;

namespace biz.dfch.CS.Appclusive.Scheduler
{
    class ScheduledTaskWorker
    {
        private const int _updateIntervalMinutesDefault = 5;
        private int _updateIntervalMinutes;
        private const int _serverNotReachableRetriesDefault = 3;
        private int _serverNotReachableRetryMinutes;
        bool _fInitialised = false;
        DateTime _lastInitialised;
        private DateTime _lastUpdate = DateTime.Now;
        private Timer _stateTimer = null;
        private TimerCallback _timerDelegate;
        private List<ScheduledTask> _list = new List<ScheduledTask>();
        private Uri _uri;
        private Diagnostics _svcDiagnostics;
        private Core _svcCore;
        private string _managementUri = Environment.MachineName;

        private bool _active = true;
        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public ScheduledTaskWorker(string Uri, string ManagementUri, int UpdateIntervalMinutes, int ServerNotReachableRetries)
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            this.Initialise(Uri, ManagementUri, UpdateIntervalMinutes, ServerNotReachableRetries, true);
        }
        public bool UpdateScheduledTasks()
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            var fReturn = false;

            if (!_active) return fReturn;

            ManagementUri mgmtUri = null;
            try
            {
                var Now = DateTime.Now;
                // DFTODO - read from App.config
                mgmtUri = _svcCore.ManagementUris
                    .Where
                    (
                        e => e.Name.Equals(_managementUri, StringComparison.OrdinalIgnoreCase) &&
                        e.Type.Equals("biz.dfch.CS.Appclusive.Scheduler", StringComparison.OrdinalIgnoreCase)
                    )
                    .SingleOrDefault();

                if(null == mgmtUri)
                {
                    Debug.WriteLine("{0}: ManagementUri not found at '{1}'. Will retry later.", _managementUri, _svcDiagnostics.BaseUri);
                    if (_serverNotReachableRetryMinutes <= (Now - _lastUpdate).TotalMinutes)
                    {
                        throw new TimeoutException();
                    }
                    goto Success;
                }

                var jtoken = JToken.Parse(mgmtUri.Value);
                lock (_list)
                {
                    _list.Clear();
                    if (jtoken is JArray)
                    {
                        var ja = JArray.Parse(mgmtUri.Value);
                        foreach (var j in ja)
                        {
                            Debug.WriteLine(string.Format("{0}: Adding '{1}' ...", _managementUri, mgmtUri.Value));
                            _list.Add(extractTask(j));
                        }
                    }
                    else if (jtoken is JObject)
                    {
                        Debug.WriteLine(string.Format("{0}: Adding '{1}' ...", _managementUri, mgmtUri.Value));
                        _list.Add(extractTask(jtoken));
                    }
                }
            }
            catch(InvalidOperationException ex)
            {
                Trace.WriteException(ex.Message, ex);
                Debug.WriteLine(string.Format("{0}: ManagementUri not found at '{1}'. Aborting ...", _managementUri, _svcDiagnostics.BaseUri.AbsoluteUri));
                throw;
            }
            catch (TimeoutException ex)
            {
                Trace.WriteException(ex.Message, ex);
                Debug.WriteLine(string.Format("{0}: Timeout retrieving ManagementUri at '{1}'. Aborting ...", _managementUri, _svcDiagnostics.BaseUri.AbsoluteUri));
                throw;
            }
            finally
            {
                if(null != mgmtUri)
                {
                    _svcDiagnostics.Detach(mgmtUri);
                }
            }
Success :
            _lastInitialised = DateTime.Now;
            fReturn = true;
            return fReturn;
        }

        private ScheduledTask extractTask(JToken taskParameters)
        {
            Contract.Requires(null != taskParameters);

            var task = new ScheduledTask(taskParameters.ToString());
            var mgmtCredential = _svcCore.ManagementCredentials
                .Where
                (
                    e => e.Name.Equals(task.Parameters.ManagementCredential, StringComparison.OrdinalIgnoreCase)
                )
                .Single();

            task.Username = mgmtCredential.Username;
            task.Password = mgmtCredential.Password;

            _svcCore.Detach(mgmtCredential);
            return task;
        }

        protected bool Initialise(string uri, string managementUri, int updateIntervalMinutes, int serverNotReachableRetries, bool fThrowException)
        {
            Contract.Requires(!string.IsNullOrWhiteSpace(uri));
            Contract.Requires(!string.IsNullOrWhiteSpace(managementUri));
            Contract.Requires(0 <= updateIntervalMinutes);
            Contract.Requires(0 <= serverNotReachableRetries);

            Contract.Ensures(Contract.OldValue(uri) == Contract.ValueAtReturn(out uri));
            Contract.Ensures(Contract.OldValue(managementUri) == Contract.ValueAtReturn(out managementUri));
            Contract.Ensures(Contract.OldValue(updateIntervalMinutes) == Contract.ValueAtReturn(out updateIntervalMinutes));
            Contract.Ensures(Contract.OldValue(serverNotReachableRetries) == Contract.ValueAtReturn(out serverNotReachableRetries));

            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            var fReturn = false;
            if (_fInitialised) return fReturn;

            try
            {
                _updateIntervalMinutes = (0 != updateIntervalMinutes) ? updateIntervalMinutes : _updateIntervalMinutesDefault;
                _serverNotReachableRetryMinutes = _updateIntervalMinutes * (0 != serverNotReachableRetries ? serverNotReachableRetries : _serverNotReachableRetriesDefault);

                _uri = new Uri(uri);
                Debug.WriteLine(string.Format("Uri: '{0}'", this._uri.AbsoluteUri));
                _managementUri = managementUri;

                _svcDiagnostics = new Diagnostics(new Uri(string.Format("{0}Diagnostics", _uri.AbsoluteUri)));
                _svcDiagnostics.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                _svcDiagnostics.Format.UseJson();

                _svcCore = new Core(new Uri(string.Format("{0}api/Core", _uri.AbsoluteUri)));
                _svcCore.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                _svcCore.Format.UseJson();

                UpdateScheduledTasks();

                _timerDelegate = new TimerCallback(this.RunTasks);
                //var MilliSecondsToWait = (60 - DateTime.now.Second) * 1000;
                //Debug.WriteLine(string.Format("Waiting {0}ms for begin of next minute ...", MilliSecondsToWait));
                //System.Threading.Thread.Sleep(MilliSecondsToWait);
                _stateTimer = new Timer(_timerDelegate, null, 1000, (1000 * 60) - 20);
                
                fReturn = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(string.Format("{0}@{1}: {2}\r\n{3}", ex.GetType().Name, ex.Source, ex.Message, ex.StackTrace));
                if (fThrowException)
                {
                    throw;
                }
                else
                {
                    fReturn = false;
                }
            }
            finally
            {
                // N/A
            }
            this._fInitialised = fReturn;
            this._active = fReturn;
            return fReturn;
        }

        ~ScheduledTaskWorker()
        {
            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);

            if (null != this._stateTimer)
            {
                _stateTimer.Dispose();
            }
        }

        // The state object is necessary for a TimerCallback.
        protected void RunTasks(object stateObject)
        {
            var fReturn = false;
            var now = DateTime.Now;

            if (!_active || !_fInitialised) return;

            Debug.WriteLine("{0}:{1}.{2}", this.GetType().Namespace, this.GetType().Name, System.Reflection.MethodBase.GetCurrentMethod().Name);
            try
            {
                lock (_list)
                {
                    //Debug.WriteLine(string.Format("Iterating list ... '{0}'", _list.Count));
                    foreach (var task in _list)
                    {
                        //var nextSchedule = task.GetNextSchedule();
                        //Debug.WriteLine(string.Format("Checked '{0}' [{1}].", task.Parameters.CommandLine, nextSchedule.ToString()));
                        if (task.IsScheduledToRun(now))
                        {
                            Debug.WriteLine(string.Format("Invoking '{0}' as '{1}' [{2}] ...", task.Parameters.CommandLine, task.Username, task.NextSchedule.ToString()));

                            // DFTODO - check if this call is blocking
                            biz.dfch.CS.Utilities.Process.StartProcess(task.Parameters.CommandLine, task.Parameters.WorkingDirectory, task.Credential);
                        }
                    }
                }

                if (_updateIntervalMinutes <= (now - _lastInitialised).TotalMinutes)
                {
                    fReturn = UpdateScheduledTasks();
                }
            }
            catch (TimeoutException ex)
            {
                Trace.WriteException(ex.Message, ex);
                var msg = string.Format("{0}: Timeout retrieving ManagementUri at '{1}'. Aborting ...", _managementUri, _svcDiagnostics.BaseUri.AbsoluteUri);
                Trace.WriteLine(msg);
                Environment.FailFast(msg);
                throw;
            }
            catch (Exception ex)
            {
                Trace.WriteException(ex.Message, ex);
                throw;
            }
        }
    }
}
