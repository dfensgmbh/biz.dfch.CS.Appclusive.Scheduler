/**
 * Copyright 2011-2016 d-fens GmbH
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
 *
 */

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.Utilities.General;
using biz.dfch.CS.Utilities.Logging;

namespace biz.dfch.CS.Appclusive.Scheduler.Core
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            var fn = Method.fn();
            Debug.WriteLine("{0}: Environment.UserInteractive '{1}'", fn, Environment.UserInteractive);

            if (Environment.UserInteractive)
            {
                var isInteractiveStartup = true;
                if(null != args && 1 <= args.Length)
                {
                    var arg0 = args[0].ToUpper();
                    Contract.Assert(!string.IsNullOrWhiteSpace(arg0));
                    if (arg0.Equals("ENCRYPT"))
                    {
                        var message = @"d-fens AppclusiveScheduler
Copyright (C) d-fens GmbH. Source code licensed under Apache 2.0 license.

Encrypting app.config credential section...";

                        Console.WriteLine(message);
                        new AppclusiveCredentialSectionManager().Encrypt();
                        isInteractiveStartup = false;
                    }
                    else if (arg0.Equals("DECRYPT"))
                    {
                        var message = @"d-fens AppclusiveScheduler
Copyright (C) d-fens GmbH. Source code licensed under Apache 2.0 license.

Decrypting app.config credential section...";
                        
                        Console.WriteLine(message);
                        new AppclusiveCredentialSectionManager().Decrypt();
                        isInteractiveStartup = false;
                    }
                    else if (arg0.Equals("HELP") || arg0.Equals("--HELP") || arg0.Equals("-HELP") || arg0.Equals("/HELP") 
                        || arg0.Equals("h") || arg0.Equals("/h") || arg0.Equals("-h") 
                        || arg0.Equals("?") || arg0.Equals("-?") || arg0.Equals("/?"))
                    {
                        var message = @"d-fens AppclusiveScheduler
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
                        Console.WriteLine(message);
                        isInteractiveStartup = false;
                    }
                }
                if(!isInteractiveStartup)
                {
                    return;
                }

                var service = new AppclusiveSchedulerService();

                Console.CancelKeyPress += delegate(object sender, ConsoleCancelEventArgs e)
                {
                    e.Cancel = true;
                    service.TerminateInteractiveService();
                };

                service.OnStartInteractive(args);
            }
            else
            {
                ServiceBase[] servicesToRun;
                servicesToRun = new ServiceBase[] 
                { 
                    new AppclusiveSchedulerService() 
                };
                ServiceBase.Run(servicesToRun);
            }
        }
    }
}
