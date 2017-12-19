using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Contracts;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace DejaVu.SelfHealthCheck.Engine
{
    public class NetworkCheckRunner : ICheckRunner
    {
        public ICheckResult Check(ICheckConfiguration checkDetails)
        {
            var networkDetails = checkDetails as INetworkConfiguration;

            ICheckResult result = new CheckResult()
            {
                Title = networkDetails.Title
            };

            //TODO: Network Check
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();

            IPEndPoint[] endPoints = ipProperties.GetActiveTcpListeners();
            TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();

            //Check Existing Connections
            bool connectionExists = false;
            foreach (var tcpConnection in tcpConnections)
            {
                if ((tcpConnection.LocalEndPoint.Address.Equals(IPAddress.Parse(networkDetails.HostName)) && tcpConnection.LocalEndPoint.Port == networkDetails.Port)
                || (tcpConnection.RemoteEndPoint.Address.Equals(IPAddress.Parse(networkDetails.HostName)) && tcpConnection.RemoteEndPoint.Port == networkDetails.Port))
                {
                    result.Status = tcpConnection.State == TcpState.Established ? CheckResultStatus.Up : CheckResultStatus.Down;
                    connectionExists = true;
                    break;
                }
            }

            Stopwatch timer = new Stopwatch();

            //Attempt connection
            if (networkDetails.IsPersistentConnection && !connectionExists)
            {
                result.AdditionalInformation = "No Persistent connection found";

                result.Status = CheckResultStatus.Down;
            }
            else if (!networkDetails.DontAttemptConnection)
            {
                if ((networkDetails.SupportsMultipleConnections && connectionExists) || !connectionExists)
                {
                    using (var client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                    {
                        try
                        {
                            timer.Start();
                            client.Connect(networkDetails.HostName, networkDetails.Port);
                            timer.Stop();

                            result.TimeElasped = Convert.ToDouble(timer.ElapsedMilliseconds);

                            result.Status = networkDetails.ResponseTime > result.TimeElasped ? CheckResultStatus.Up : CheckResultStatus.PerfomanceDegraded;
                        }
                        catch (SocketException ex)
                        {
                            timer.Stop();
                            result.TimeElasped = Convert.ToDouble(timer.ElapsedMilliseconds);

                            result.AdditionalInformation = string.Format("{0} - {1}", ex.SocketErrorCode, ex.Message);

                            result.Status = CheckResultStatus.Down;
                        }
                    }
                }
            }
            
            if (networkDetails.AfterCheckAction != null)
            {
                 result = networkDetails.AfterCheckAction.Invoke(networkDetails, result);
            }

            return result;
        }
    }
}
