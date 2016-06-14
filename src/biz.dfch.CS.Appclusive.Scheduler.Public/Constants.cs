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
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace biz.dfch.CS.Appclusive.Scheduler.Public
{
    public class Constants
    {
        public const string PLUGIN_TYPE_DEFAULT = "Default";

        public class InvocationResultCodes
        {
            public const int ERROR_SUCCESS = 0x00000000;
            public const int ERROR_INVALID_FUNCTION = 0x00000001;
            public const int ERROR_SERVICE_NOT_ACTIVE = 0x00000426;
        }
    }
}
