using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Contracts;
using DejaVu.SelfHealthCheck.Engine;

namespace DejaVu.SelfHealthCheck.Configuration
{
    [CheckRunner(typeof(NetworkCheckRunner))]
    public class NetworkCheck : BaseCheck, INetworkCheck, INetworkConfiguration, INetworkConnectedCheck
    {
        public NetworkCheck(IConfigure selfHealthCheckConfiguration, string title)
        {
            SelfHealthCheckConfiguration = selfHealthCheckConfiguration;
            Title = title;
        }

        public string HostName
        {
            get;
            set;
        }

        public int Port
        {
            get;
            set;
        }

        public bool SupportsMultipleConnections
        {
            get;
            set;
        }

        public double ResponseTime
        {
            get;
            set;
        }

        public bool DontAttemptConnection
        {
            get;
            set;
        }

        public bool IsPersistentConnection
        {
            get;
            set;
        }

        public INetworkConnectedCheck On(string hostname, int port, bool isPersistentConnection = false, bool supportsMultipleConnections = false)
        {
            this.HostName = hostname;
            this.Port = port;
            this.SupportsMultipleConnections = supportsMultipleConnections;
            this.IsPersistentConnection = isPersistentConnection;
            return this;
        }

        public INetworkConnectedCheck NoConnect()
        {
            this.DontAttemptConnection = true;

            return this;
        }

        public INetworkConnectedCheck WithResponseTime(double maxResponseTime)
        {
            this.ResponseTime = maxResponseTime;

            return this;
        }

        public INetworkConnectedCheck Run(Func<ICheckConfiguration, ICheckResult, ICheckResult> customCheck)
        {
            AfterCheckAction = customCheck;

            return this;
        }


        public INetworkConnectedCheck Perform(Func<KeyValuePair<bool, string>> customCheck)
        {
            throw new NotImplementedException();
        }

    }
}
