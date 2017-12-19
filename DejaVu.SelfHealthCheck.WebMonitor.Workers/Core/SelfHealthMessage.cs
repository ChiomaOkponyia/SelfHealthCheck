using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DejaVu.SelfHealthCheck.Contracts;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Core
{
   public class SelfHealthMessage: Entity,ISelfHealthMessage
    {
        public virtual DateTime DateChecked { get; set; }
        public virtual string AppID { get; set; }
        public virtual List<ICheckResult> Results { get; set; }

        public virtual CheckResultStatus OverallStatus { get; set; }

        public virtual string Title { get; set; }

        public virtual double TimeElapsed { get; set; }

        public virtual string AdditionalInformation { get; set; }

        public virtual string IPAddress { get; set; }

        public virtual DateTime NextCheckTime { get; set; }
    }
}
