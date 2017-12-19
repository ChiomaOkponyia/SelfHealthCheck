using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DejaVu.SelfHealthCheck.Contracts
{
    public interface INetworkConfiguration : ICheckConfiguration
    {
        string HostName { get; set; }
        int Port { get; set; }
        bool SupportsMultipleConnections { get; set; }
        bool IsPersistentConnection { get; set; }
        double ResponseTime { get; set; }
        bool DontAttemptConnection { get; set; }
    }
}
