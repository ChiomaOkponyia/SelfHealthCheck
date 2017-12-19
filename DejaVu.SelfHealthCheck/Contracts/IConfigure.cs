using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Configuration;

namespace DejaVu.SelfHealthCheck.Contracts
{
    public interface IConfigure
    {
        T With<T, R>(string title)
            where T : IConfigure
            where R : class,T, ICheckConfiguration;

        IConfigure SelfHealthCheckConfiguration { get; }
        IConfigure WithCheckInterval(double interval);
        IConfigure WithIPAddress(string IPAddress);
        INetworkCheck WithNetworkCheck(string title);
        IDatabaseCheck WithDatabaseCheck(string title);
        IUrlCheck WithUrlCheck(string title);
        ICustomCheck<CustomCheck> WithCustomCheck(string title);
    }
}
