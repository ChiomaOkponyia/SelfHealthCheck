using DejaVu.SelfHealthCheck.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Core
{
    public class TreeCheckResult : ICheckResult
    {
        public string Id { get; set; } 
        public string AppID { get; set; }
        public string Title { get; set; }
        public CheckResultStatus Status { get; set; }
        public string AdditionalInformation { get; set; }
        public double TimeElasped { get; set; }
    }
}
