using Microsoft.Owin.Hosting;
using NServiceBus.Features;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.Collector
{
    class StartStopTask : FeatureStartupTask, IDisposable
    {

        IDisposable SignalR;
        protected override void OnStart()
        {

            string url = ConfigurationManager.AppSettings["SignalrUrl"];
            SignalR = WebApp.Start(url);
        }

        protected override void OnStop()
        {
            SignalR.Dispose();
        }

        public void Dispose()
        {
        }

    }
}
