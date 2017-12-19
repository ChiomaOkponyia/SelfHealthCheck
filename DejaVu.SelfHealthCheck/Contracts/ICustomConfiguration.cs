using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DejaVu.SelfHealthCheck.Contracts
{
    public interface ICustomConfiguration : ICheckConfiguration
    {
        Func<KeyValuePair<bool, string>> TheCustomCheck { get; set; }
        double ResponseTime { get; set; }
    }
}
