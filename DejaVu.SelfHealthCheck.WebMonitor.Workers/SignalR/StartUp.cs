using Microsoft.Owin;
using Owin;
[assembly: OwinStartup(typeof(DejaVu.SelfHealthCheck.WebMonitor.Workers.SignalR.Startup))]
namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.SignalR
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR("/monitorx", typeof(DejaVu.SelfHealthCheck.WebMonitor.Workers.SignalR.MonitorHub), new Microsoft.AspNet.SignalR.ConnectionConfiguration());
            app.MapSignalR("/resultx", typeof(DejaVu.SelfHealthCheck.WebMonitor.Workers.SignalR.ResultHub), new Microsoft.AspNet.SignalR.ConnectionConfiguration());
        }
    }
}
