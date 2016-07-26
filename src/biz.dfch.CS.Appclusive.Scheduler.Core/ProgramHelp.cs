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
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class ProgramHelp
    {
        const string ENCRYPT_MESSAGE = @"d-fens AppclusiveScheduler {0} {1}
Copyright (C) d-fens GmbH. Source code licensed under Apache 2.0 license.

Encrypting app.config credential section...";

        public string GetEncryptMessage()
        {
            return string.Format(ENCRYPT_MESSAGE, GetVersion().ToString(), GetArchitecture());
        }

        const string DECRYPT_MESSAGE = @"d-fens AppclusiveScheduler {0} {1}
Copyright (C) d-fens GmbH. Source code licensed under Apache 2.0 license.

Decrypting app.config credential section...";

        public string GetDecryptMessage()
        {
            return string.Format(DECRYPT_MESSAGE, GetVersion().ToString(), GetArchitecture());
        }

        const string HELP_MESSAGE = @"d-fens AppclusiveScheduler {0} {1}
Copyright (C) d-fens GmbH. Source code licensed under Apache 2.0 license.

The AppclusiveScheduler is a Windows service application that executes 
ScheduledJob instances defined in the Appclusive inventory.

To install the service use the 'InstallUtil' from the Microsoft .NET Framework. 
Please make sure to use the version that corresponds with the version this 
executable was built with.

Usage: AppclusiveScheduler [ENCRYPT | DECRYPT | HELP | URI | MGMTURINAME]

ENCRYPT:
Encrypt the AppclusiveCredential configuration section

DECRYPT: 
Decrypt the AppclusiveCredential configuration section

URI:
Specify the Appclusive base uri (and override its app.config setting)

MGMTURI:
Specify the Appclusive ManagementUri (and override its app.config setting)

HELP:
Show this help screen

When started interactively, you can press Ctrl-C at any time to shutdown.
";

        public string GetHelpMessage()
        {
            return string.Format(HELP_MESSAGE, GetVersion().ToString(), GetArchitecture());
        }

        const string INTERACTIVE_MESSAGE = @"d-fens AppclusiveScheduler {0} {1}
Copyright (C) d-fens GmbH. Source code licensed under Apache 2.0 license.

Starting service interactively. Press Ctrl-C to abort ...";

        public string GetInteractiveMessage()
        {
            return string.Format(INTERACTIVE_MESSAGE, GetVersion().ToString(), GetArchitecture());
        }

        public Version GetVersion()
        {
            Contract.Ensures(null != Contract.Result<Version>());

            var version = this.GetType().Assembly.GetName().Version;
            return version;
        }

        public string GetArchitecture()
        {
            var application = Environment.Is64BitProcess ? "x64" : "x86";
            var operatingSystem = Environment.Is64BitOperatingSystem ? "x64" : "x86";

            var result = string.Format("[{0}/{1}]", application, operatingSystem);
            return result;
        }

    }
}
