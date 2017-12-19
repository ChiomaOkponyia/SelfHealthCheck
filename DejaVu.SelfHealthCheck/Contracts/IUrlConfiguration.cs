using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DejaVu.SelfHealthCheck.Contracts
{
    public interface IUrlConfiguration : ICheckConfiguration
    {
        string Url { get; set; }
        double ResponseTime { get; set; }
    }
}
