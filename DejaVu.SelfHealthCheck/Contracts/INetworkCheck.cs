using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DejaVu.SelfHealthCheck.Contracts
{
    public interface INetworkCheck : IConfigure
    {
        //Network Connection Parameters Setup
        INetworkConnectedCheck On(string hostname, int port, bool isPersistentConnection = false, bool supportsMultipleConnections = false);
    }

    /// <summary>
    /// Network Checks when Connection has been established or validated
    /// </summary>
    public interface INetworkConnectedCheck : ICustomCheck<INetworkConnectedCheck>
    {
        //Connect Check
        /// <summary>
        /// There will be no attempt to connect to the specified hostname:port to confirm the status. This will only check that the host is listening on the port
        /// </summary>
        /// <returns></returns>
        INetworkConnectedCheck NoConnect();

        //Response Time Check
        /// <summary>
        /// Specify what the acceptable response time for the connection to be established 
        /// </summary>
        /// <param name="maxResponseTime">The max acceptable response time in milli-seconds</param>
        /// <returns></returns>
        INetworkConnectedCheck WithResponseTime(double maxResponseTime);
    }
}
