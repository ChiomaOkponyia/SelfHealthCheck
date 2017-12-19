using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NServiceBus;
using DejaVu.SelfHealthCheck.Messages;
using DejaVu.SelfHealthCheck.Events;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.SignalR;
using Raven.Client;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using DejaVu.SelfHealthCheck.Contracts;
using System.Transactions;

namespace DejaVu.SelfHealthCheck.Web
{
    public class HealthMessageHandler : IHandleMessages<SelfHealthMessage>
    {
        public IBus Bus { get; set; }
        public void Handle(SelfHealthMessage message)
        {
            Console.WriteLine();
            Console.WriteLine("[{0:dd-MMM-yyyy hh:mm tt}] {1} Status: {2}", message.DateChecked, message.AppID, message.OverallStatus);
            foreach (var res in message.Results)
            {
                Console.WriteLine("{0}\t{1}\t({2}ms)\t[{3}]", res.Title, res.Status, res.TimeElasped, res.AdditionalInformation);
            }
            Console.WriteLine("---------------------------------------------------");
            Console.WriteLine();
            //Bus.Send<SelfHealthMessage>(x =>
            //    {
            //        x.AppID = message.AppID;
            //        x.DateChecked = message.DateChecked;
            //        x.OverallStatus = message.OverallStatus;
            //        x.Results = message.Results;
            //    });
            //Bus.Send("DejaVu.SelfHealthCheck.WebMonitor.Workers", new SelfHealthMessage() 
            //{ 
            //    AppID = message.AppID,
            //    DateChecked = message.DateChecked,
            //    OverallStatus = message.OverallStatus,
            //    Results = message.Results
            //});
            //Bus.Publish<IMessageReceivedEvent>(e => e.Message = message);
            GlobalHost.ConnectionManager.GetConnectionContext<MonitorHub>().Connection.Broadcast("messageReceived");
            DejaVu.SelfHealthCheck.WebMonitor.Workers.RavenDB.RavenStore.CustomInitializeStore();
            using (IDocumentSession session = DejaVu.SelfHealthCheck.WebMonitor.Workers.RavenDB.RavenStore.Store.OpenSession())
            {
                try
                {
                    Component theComponent = new Component();
                    theComponent = session.Query<Component>().Where(x => x.AppID == message.AppID).FirstOrDefault();
                    if (theComponent == null)
                    {
                    }
                    else
                    {
                        using (var transaction = new TransactionScope())
                        {
                            TreeCheckResult checkResult = new TreeCheckResult();
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
                            theComponent.DateChecked = message.DateChecked.ToString("dd-MMM-yyyy hh:mm:ss tt");
                            theComponent.Status = GetOverallStatus(newMemberResults).ToString();
                            session.SaveChanges();
                            transaction.Complete();
                        }
                        var appId = theComponent.AppID;
                        if (appId.StartsWith("{")) appId = "" + appId.Substring(1, appId.Length - 1);
                        if (appId.EndsWith("}")) appId = appId.Substring(0, appId.Length - 1) + "";
                        theComponent.AppID = appId;
                        GlobalHost.ConnectionManager.GetConnectionContext<MonitorHub>().Connection.Broadcast(JsonConvert.SerializeObject(theComponent));
                    }
                }
                catch (Exception e)
                {
                    ErrorLog log = new ErrorLog()
                    {
                        AppID = message.AppID,
                        ErrorMessage = e.Message
                    };
                    session.Store(log);
                }
            }
        }
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
