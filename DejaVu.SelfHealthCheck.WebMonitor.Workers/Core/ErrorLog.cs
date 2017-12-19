using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Core
{
    public class ErrorLog
    {
        public string Id { get; set; }
        public string AppID { get; set; }
        public string ErrorMessage { get; set; }
    }
}
