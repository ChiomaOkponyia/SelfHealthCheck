using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using Microsoft.AspNet.SignalR;
using Raven.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.SignalR
{
    public class AsyncResultUpdater
    {
        static bool newAppArrived = false;
        static string newAppId = string.Empty;
        static string newConnectionId = string.Empty;
        public static void UpdateAsync(string connectionId, string appId)
        {
            using (IDocumentSession session = DejaVu.SelfHealthCheck.WebMonitor.Workers.RavenDB.RavenStore.Store.OpenSession())
            {
                var apps = session.Query<Component>().Where(x => x.AppID == appId).ToList();
                if(apps.Count > 0)
                {
                    newAppArrived = true;
                    newAppId = appId;
                    newConnectionId = connectionId;
                    Thread.Sleep(1000);
                    Task task = new Task(() => ProcessDataAsync(connectionId, appId));
                    task.Start();
                }
                else
                {
                    GlobalHost.ConnectionManager.GetConnectionContext<MonitorHub>().Connection.Send(connectionId, "App Not Found.");
                    Thread.Sleep(1000);
                }
            }           
        }
        static async void ProcessDataAsync(string connectionId, string appId)
        {
            newAppArrived = false;
            int count = 0;
            string msg = "Fetching Results...";
            GlobalHost.ConnectionManager.GetConnectionContext<MonitorHub>().Connection.Send(connectionId, msg);          
            while (true)
            {
                if (newAppArrived && newConnectionId == connectionId) break;

                using (IDocumentSession session = DejaVu.SelfHealthCheck.WebMonitor.Workers.RavenDB.RavenStore.Store.OpenSession())
                {              
                    List<TreeCheckResult> allResults = session.Query<TreeCheckResult>().Where(x => x.AppID == appId).ToList();
                    string resultAsString = DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic.TreeListMemberLogic.HtmlizeResults(allResults);
                    GlobalHost.ConnectionManager.GetConnectionContext<MonitorHub>().Connection.Send(connectionId, resultAsString);
                    Thread.Sleep(4000);
                }
            } 
            
        }
    }
}
