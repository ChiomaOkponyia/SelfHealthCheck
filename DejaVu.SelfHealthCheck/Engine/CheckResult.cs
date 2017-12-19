using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Contracts;

namespace DejaVu.SelfHealthCheck.Engine
{
    public class CheckResult : ICheckResult
    {

        public string Title
        {
            get;
            set;
        }

        public CheckResultStatus Status
        {
            get;
            set;
        }

        public string AdditionalInformation
        {
            get;
            set;
        }

        public double TimeElasped
        {
            get;
            set;
        }
    }
}
