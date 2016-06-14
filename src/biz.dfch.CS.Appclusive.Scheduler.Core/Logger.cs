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
using System.Linq;
using biz.dfch.CS.Appclusive.Public.Plugins;
using biz.dfch.CS.Utilities.Logging;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class Logger : IAppclusivePluginLogger
    {
        public void Write(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Trace.Write(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Trace.WriteLine(message);
        }

        public void Debug(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Trace.WriteLine(message);
        }

        public void Info(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Trace.WriteLine(message);
        }

        public void Notice(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Trace.WriteLine(message);
        }

        public void Warn(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Trace.WriteLine(message);
        }

        public void Error(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Trace.WriteLine(message);
        }

        public void Alert(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Trace.WriteLine(message);
        }

        public void Critical(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Trace.WriteLine(message);
        }

        public void Emergency(string format, params object[] args)
        {
            var message = string.Format(format, args);
            Trace.WriteLine(message);
        }
    }
}
