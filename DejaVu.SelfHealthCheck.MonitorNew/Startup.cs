using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
[assembly: OwinStartup(typeof(DejaVu.SelfHealthCheck.MonitorNew.Startup))] 
namespace DejaVu.SelfHealthCheck.MonitorNew
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Any connection or hub wire up and configuration should go here
            //app.UseCors(CorsOptions.AllowAll);

            app.UseCors(CorsOptions.AllowAll);
            var hubConfig = new HubConfiguration();
            hubConfig.EnableDetailedErrors = true;
            app.MapSignalR(hubConfig);
        }
    }
}
