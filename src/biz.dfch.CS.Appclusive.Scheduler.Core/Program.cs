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
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        public static void Main(string[] args)
        {
            var fn = Method.fn();
            Debug.WriteLine("d-fens AppclusiveScheduler {0}. {1}: Environment.UserInteractive '{2}'", new ProgramHelp().GetVersion(), fn, Environment.UserInteractive);

            if (Environment.UserInteractive)
            {
                var isInteractiveStartup = true;
                if(null != args && 1 <= args.Length)
                {
                    var arg0 = args[0].TrimStart('-').TrimStart('/').ToUpper();
                    Contract.Assert(!string.IsNullOrWhiteSpace(arg0));
                    if (arg0.Equals("ENCRYPT"))
                    {
                        Console.WriteLine(new ProgramHelp().GetEncryptMessage());
                        new AppclusiveCredentialSectionManager().Encrypt();
                        isInteractiveStartup = false;
                    }
                    else if (arg0.Equals("DECRYPT"))
                    {
                        Console.WriteLine(new ProgramHelp().GetDecryptMessage());
                        new AppclusiveCredentialSectionManager().Decrypt();
                        isInteractiveStartup = false;
                    }
                    else if 
                    (
                        arg0.Equals("HELP") 
                        || 
                        arg0.Equals("H")
                        || 
                        arg0.Equals("?")
                    )
                    {
                        Console.WriteLine(new ProgramHelp().GetHelpMessage());
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
