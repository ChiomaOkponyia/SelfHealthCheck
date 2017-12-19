using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Raven.Client;
using Raven.Client.Embedded;
using Raven.Client.Document;
using NServiceBus;
using DejaVu.SelfHealthCheck.Events;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.SignalR;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic;

namespace DejaVu.SelfHealthCheck.WebMonitor
{
    public class HealthEventHandler : System.Web.HttpApplication, IHandleMessages<IMessageReceivedEvent>
    {
        public IBus Bus { get; set; }
        public void Handle(IMessageReceivedEvent e)
        {
            GlobalHost.ConnectionManager.GetConnectionContext<MonitorHub>().Connection.Broadcast("messageReceived");
            //USING RAVEN DB EMBEDDED
            using (IDocumentSession session = Global.Store.OpenSession())
            {
                try
                {
                    if (session.Query<TreeListMember>().Where(x => x.AppID == e.Message.AppID).Any())
                    {
                        TreeListMember member = session.Query<TreeListMember>().Where(x => x.AppID == e.Message.AppID).First();
                        member.DateChecked = e.Message.DateChecked;
                        member.Results = e.Message.Results;
                        member.Status = e.Message.OverallStatus;
                        GlobalHost.ConnectionManager.GetConnectionContext<MonitorHub>().Connection.Broadcast("afterProcessing");
                        session.SaveChanges();
                        //:: SignalR CLIENT ::
                        var hubContext = GlobalHost.ConnectionManager.GetConnectionContext<MonitorHub>();
                        hubContext.Connection.Broadcast(JsonConvert.SerializeObject(member));
                    }


                    else
                    {
                        Component c = new Component();
                        using (var datacontext = new ServiceMonitorDataContext())
                        {
                            var comp = datacontext.Components.Where(x => x.AppID == e.Message.AppID);
                            if (comp.Any())
                            {
                                c.AppID = comp.First().AppID;
                                c.AppName = comp.First().AppName;
                                c.HasSubComponents = comp.First().HasSubComponents == true ? true : false;
                                c.ParentComponentId = comp.First().ParentComponentId;
                            }
                        }

                        TreeListMember m = new TreeListMember()
                        {
                            AppID = e.Message.AppID,
                            AppName = c.AppName,
                            DateChecked = e.Message.DateChecked,
                            HasSubComponents = c.HasSubComponents,
                            ParentComponentId = c.ParentComponentId,
                            Results = e.Message.Results,
                            Status = e.Message.OverallStatus
                        };
                        session.Store(m);
                        //:: SignalR CLIENT ::
                        var hubContext = GlobalHost.ConnectionManager.GetConnectionContext<MonitorHub>();
                        hubContext.Connection.Broadcast("reloadPage");
                    }
                }
                catch (Exception exp)
                {
                    GlobalHost.ConnectionManager.GetConnectionContext<MonitorHub>().Connection.Broadcast("error:" + exp.Message);
                }
            }
        }
    }
}