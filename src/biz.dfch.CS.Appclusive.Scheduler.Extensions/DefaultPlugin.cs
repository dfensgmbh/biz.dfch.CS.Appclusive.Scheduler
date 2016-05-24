using System.Collections.Specialized;
using System.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;
using System.Diagnostics;
using biz.dfch.CS.Appclusive.Scheduler.Contracts;

namespace biz.dfch.CS.Appclusive.Scheduler.Extensions
{
    [Export(typeof(ISchedulerPlugin))]
    [ExportMetadata("Type", "Default")]
    public class DefaultPlugin : ISchedulerPlugin
    {
        public StringDictionary Configuration { get; set; }

        public void Log(string message)
        {
            Trace.WriteLine(message);

            return;
        }

        public bool UpdateConfiguration(StringDictionary configuration)
        {
            var fReturn = false;
        
            var message = new StringBuilder();
            message.AppendLine("DefaultPlugin.UpdatingConfiguration ...");
            message.AppendLine();

            foreach(KeyValuePair<string, object> item in configuration)
            {
                message.AppendFormat("{0}: '{1}'", item.Key, item.Value ?? item.Value.ToString());
                message.AppendLine();
            }
            message.AppendLine("DefaultPlugin.UpdatingConfiguration COMPLETED.");
            message.AppendLine();
            
            Trace.WriteLine(message.ToString());

            fReturn = true;
            
            return fReturn;
        }

        public bool Invoke(StringDictionary data, ref JobResult jobResult)
        {
            Contract.Requires("1" == jobResult.Version);
            Contract.Ensures(jobResult.IsValid());

            var fReturn = false;

            var message = new StringBuilder();
            message.AppendLine("DefaultPlugin.Invoke ...");
            message.AppendLine();

            foreach(KeyValuePair<string, object> item in data)
            {
                message.AppendFormat("{0}: '{1}'", item.Key, item.Value ?? item.Value.ToString());
                message.AppendLine();
            }
            message.AppendLine("DefaultPlugin.Invoke() COMPLETED.");
            message.AppendLine();
            
            Trace.WriteLine(message.ToString());

            fReturn = true;
            
            jobResult.Succeeded = fReturn;
            jobResult.Code = 1;
            jobResult.Message = "DefaultPlugin.Invoke COMPLETED and logged the intended operation to a tracing facility.";
            jobResult.Description = message.ToString();
            jobResult.InnerJobResult = null;

            return fReturn;
        }
    }
}
