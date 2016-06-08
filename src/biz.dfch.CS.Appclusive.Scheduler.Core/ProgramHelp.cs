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


namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    public class ProgramHelp
    {
        public const string ENCRYPT_MESSAGE = @"d-fens AppclusiveScheduler
Copyright (C) d-fens GmbH. Source code licensed under Apache 2.0 license.

Encrypting app.config credential section...";

        public const string DECRYPT_MESSAGE = @"d-fens AppclusiveScheduler
Copyright (C) d-fens GmbH. Source code licensed under Apache 2.0 license.

Decrypting app.config credential section...";

        public const string HELP_MESSAGE = @"d-fens AppclusiveScheduler
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
Show this helpd screen

When started interactively, you can press Ctrl-C at any time to shutdown.
";

        public const string INTERACTIVE_MESSAGE = @"d-fens AppclusiveScheduler
Copyright (C) d-fens GmbH. Source code licensed under Apache 2.0 license.

Starting service interactively. Press Ctrl-C to abort ...";
    }
}
