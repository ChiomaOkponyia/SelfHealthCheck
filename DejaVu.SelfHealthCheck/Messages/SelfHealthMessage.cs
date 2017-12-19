using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;
using DejaVu.SelfHealthCheck.Contracts;
using DejaVu.SelfHealthCheck.Engine;

namespace DejaVu.SelfHealthCheck.Messages
{
    public class SelfHealthMessage : IMessage, ISelfHealthMessage
    {
        public DateTime DateChecked { get; set; }
        public string AppID { get; set; }
        public List<ICheckResult> Results { get; set; }

        public CheckResultStatus OverallStatus { get; set; }

        public string Title { get; set; }

        public double TimeElapsed { get; set; }

        public string AdditionalInformation { get; set; }
        public string IPAddress { get; set; }
        public DateTime NextCheckTime { get; set; }


    }
}
