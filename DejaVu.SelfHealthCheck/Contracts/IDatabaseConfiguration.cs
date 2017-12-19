using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DejaVu.SelfHealthCheck.Contracts
{
    public interface IDatabaseConfiguration : ICheckConfiguration
    {
        string ConnectionString { get; set; }
        string[] TablesToCheck { get; set; }
        double ResponseTime { get; set; }
    }
}
