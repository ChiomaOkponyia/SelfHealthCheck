//using DejaVu.SelfHealthCheck.Messages;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;

using System;
using DejaVu.SelfHealthCheck.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DejaVu.SelfHealthCheck.MonitorNew;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Data;

namespace DejavuSelfHealthCheckPopulator
{
    class Program
    {
        static void Main(string[] args)
        {
            SelfHealthMessage demo = new SelfHealthMessage();
            demo.AdditionalInformation = "Just Received A Health Check";
            demo.AppID = "3";
            demo.DateChecked = DateTime.Now;
            demo.Title = "This is a demo health message";
            demo.OverallStatus = CheckResultStatus.Up;
            demo.TimeElapsed = 3;
            new HealthMessageDAO().Save(demo);
            HealthMessageUtility.SendHealthMessage(demo);
            Console.ReadLine();
        }
    }
}
