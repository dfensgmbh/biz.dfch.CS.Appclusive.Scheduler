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

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Scheduler.Public
{
    [ContractClass(typeof(ContractClassForILogger))]
    public interface ILogger
    {
        void Write(string format, params object[] args);
        
        void WriteLine(string format, params object[] args);
        
        void Debug(string format, params object[] args);
        
        void Info(string format, params object[] args);
        
        void Notice(string format, params object[] args);
        
        void Warn(string format, params object[] args);
        
        void Error(string format, params object[] args);
        
        void Alert(string format, params object[] args);
        
        void Critical(string format, params object[] args);
        
        void Emergency(string format, params object[] args);
    }
}
