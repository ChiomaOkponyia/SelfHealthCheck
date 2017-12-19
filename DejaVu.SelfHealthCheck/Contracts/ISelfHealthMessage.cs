using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.Contracts
{
   public interface ISelfHealthMessage
    {
         DateTime DateChecked { get; set; }
        string AppID { get; set; }
         List<ICheckResult> Results { get; set; }

         CheckResultStatus OverallStatus { get; set; }

        string Title { get; set; }

         double TimeElapsed { get; set; }

         string AdditionalInformation { get; set; }

        string IPAddress { get; set; }
        DateTime NextCheckTime { get; set; }
    }
}
