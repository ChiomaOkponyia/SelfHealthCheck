using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.RavenDB;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic;
using Newtonsoft.Json;
using Raven.Client;
using System.Transactions;
using System.Threading;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.SignalR
{
    public class ResultHub : PersistentConnection
    {
        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            
            AsyncResultUpdater.UpdateAsync(connectionId, data);
            return Connection.Send(connectionId, "awaiting request...");

        }
    }
}
