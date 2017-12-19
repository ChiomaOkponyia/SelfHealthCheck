using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DejaVu.SelfHealthCheck.Contracts
{
    public interface ICheckResult
    {
        string Title { get; set; }
        CheckResultStatus Status { get; set; }
        string AdditionalInformation { get; set; }
        double TimeElasped { get; set; }
    }

    public enum CheckResultStatus
    {
        Unknown,
        Up,
        PerfomanceDegraded,
        Down,
        HealthBeat
    }
}
