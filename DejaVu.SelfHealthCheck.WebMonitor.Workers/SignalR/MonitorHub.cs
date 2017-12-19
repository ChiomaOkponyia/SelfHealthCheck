using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using Newtonsoft.Json;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.SignalR
{
    public class MonitorHub : PersistentConnection
    {
        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            if (data == "rubbish") return Connection.Broadcast("messageReceived");
            return base.OnReceived(request, connectionId, data);
        }
    }
}
