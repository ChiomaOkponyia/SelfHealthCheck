using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Transactions;
using NServiceBus;
using DejaVu.SelfHealthCheck.Contracts;
using DejaVu.SelfHealthCheck.Messages;
using Raven.Client;
using Raven.Client.Embedded;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using Microsoft.AspNet.SignalR;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.SignalR;
using Newtonsoft.Json;

namespace DejaVu.SelfHealthCheck.WebMonitor
{
    public class HealthMessageHandler : IHandleMessages<SelfHealthMessage>
    {
        public void Handle(SelfHealthMessage message)
        {
            GlobalHost.ConnectionManager.GetConnectionContext<MonitorHub>().Connection.Broadcast("messageReceived");
            using (IDocumentSession session = Global.Store.OpenSession())
            {
                try
                {
                    if (session.Query<TreeMember>().Where(x => x.AppID == message.AppID).Any())
                    {
                        TreeMember theMember = session.Load<TreeMember>(session.Query<TreeMember>().Where(x => x.AppID == message.AppID).First().Id);
                        using (var transaction = new TransactionScope())
                        {                          
                            theMember.DateChecked = message.DateChecked;
                            List<TreeCheckResult> memberResults = session.Query<TreeCheckResult>().Where(x => x.AppID == message.AppID).ToList();
                            foreach (var result in message.Results)
                            {
                                if (memberResults.Any(x => x.Title == result.Title))
                                {
                                    TreeCheckResult formerResult = session.Load<TreeCheckResult>(session.Query<TreeCheckResult>().Where(x => x.AppID == message.AppID && x.Title == result.Title).First().Id);
                                    formerResult.Status = result.Status;
                                    formerResult.AdditionalInformation = result.AdditionalInformation;
                                    formerResult.TimeElasped = result.TimeElasped;
                                    session.SaveChanges();
                                }
                                else
                                {
                                    TreeCheckResult newResult = new TreeCheckResult();
                                    newResult.AppID = message.AppID;
                                    newResult.Title = result.Title;
                                    newResult.Status = result.Status;
                                    newResult.AdditionalInformation = result.AdditionalInformation;
                                    newResult.TimeElasped = result.TimeElasped;
                                    session.Store(newResult);
                                    session.SaveChanges();
                                }
                                //session.SaveChanges();
                            }
                            List<TreeCheckResult> newMemberResults = session.Query<TreeCheckResult>().Where(x => x.AppID == message.AppID).ToList();
                            theMember.Status = GetOverallStatus(newMemberResults);
                            session.SaveChanges();
                            transaction.Complete();
                        }
                            
                        //:: SignalR CLIENT ::
                        GlobalHost.ConnectionManager.GetConnectionContext<MonitorHub>().Connection.Broadcast("afterProcessing");
                        var hubContext = GlobalHost.ConnectionManager.GetConnectionContext<MonitorHub>();
                        hubContext.Connection.Broadcast(JsonConvert.SerializeObject(theMember));
                        //}
                    }


                    else
                    {
                        Component c = new Component();
                        using (var datacontext = new ServiceMonitorDataContext())
                        {
                            var comp = datacontext.Components.Where(x => x.AppID == message.AppID);
                            if (comp.Any())
                            {
                                c.AppID = comp.First().AppID;
                                c.AppName = comp.First().AppName;
                                c.HasSubComponents = comp.First().HasSubComponents == true ? true : false;
                                c.ParentComponentId = comp.First().ParentComponentId;
                            }
                        }
                        using (var transaction = new TransactionScope())
                        {
                            TreeMember m = new TreeMember()
                            {
                                AppID = message.AppID,
                                AppName = c.AppName,
                                DateChecked = message.DateChecked,
                                HasSubComponents = c.HasSubComponents,
                                ParentComponentId = c.ParentComponentId,
                                Status = message.OverallStatus
                            };
                            session.Store(m);

                            foreach (var result in message.Results)
                            {
                                TreeCheckResult r = new TreeCheckResult()
                                {
                                    AppID = message.AppID,
                                    Title = result.Title,
                                    Status = result.Status,
                                    AdditionalInformation = result.AdditionalInformation,
                                    TimeElasped = result.TimeElasped
                                };
                                session.Store(r);
                            };
                            session.SaveChanges();
                            transaction.Complete();
                        }
                        
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

        /// <summary>
        /// They return the lowest status among a list of results.
        /// If a set of result contains 'DOWN' status, the overall status will be 'DOWN'
        /// no matter how many 'UP's are there.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        //protected CheckResultStatus GetOverallStatus(List<ICheckResult> results)
        //{
        //    CheckResultStatus overallStatus = CheckResultStatus.Unknown;
        //    foreach (var result in results)
        //    {
        //        if ((int)result.Status > (int)overallStatus) overallStatus = result.Status; 
        //    }
        //    return overallStatus;
        //}
        protected CheckResultStatus GetOverallStatus(List<TreeCheckResult> results)
        {
            CheckResultStatus overallStatus = CheckResultStatus.Unknown;
            foreach (var result in results)
            {
                if ((int)result.Status > (int)overallStatus) overallStatus = result.Status;
            }
            return overallStatus;
        }
    }
}