using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DejaVu.SelfHealthCheck.Contracts
{
    public interface ICheckConfiguration
    {
        string Title { get; set; }
        Func<ICheckConfiguration, ICheckResult, ICheckResult> AfterCheckAction { get; set; }
    }
}
