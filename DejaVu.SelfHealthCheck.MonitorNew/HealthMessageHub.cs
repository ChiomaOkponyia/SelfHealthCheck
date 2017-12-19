using DejaVu.SelfHealthCheck.Utility;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic;

using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.MonitorNew
{
    public class HealthMessageHub : Hub
    {

        public void LoadAllApplications()
        {
            List<Component> components = ComponentLogic.GetAllComponents();

            foreach (var component  in components)
            {
                List<AppStatus> replicaAppStatus = null;
                List<string> subComponentIPs = new HealthMessageLogic().GetIPAddressesForApp(component.AppID);
                var data = JsonConvert.SerializeObject(component);
                if (subComponentIPs == null || subComponentIPs.Count() == 0)
                {
                    Clients.All.loadAllApllications(data, null, null);
                    continue;
                }

                Trace.TraceInformation("About to Get Status based on Replica apps");
                replicaAppStatus = HealthMessageUtility.GetStatusBasedOnReplicas(component, subComponentIPs);
                Trace.TraceInformation("Replica App Status Done ");

                //string data = JsonConvert.SerializeObject(component);
                Clients.All.loadAllApllications(data, JsonConvert.SerializeObject(subComponentIPs), replicaAppStatus );

            }


        }

        public void LoadApplicationInfo(string appId)
        {
            Component component = ComponentLogic.GetByAppId(appId);
            string data = JsonConvert.SerializeObject(component);
            Clients.All.loadApplicationInfo(data);
        }


        public void SendChildStatusMessageToParentApps(string appId, string status)
        {
            Component component = ComponentLogic.GetByAppId(appId);
            if (component != null)
            {

                var parentComponents = ComponentLogic.GetParentComponents(component);
                if (parentComponents != null && parentComponents.Count > 0)
                {
                    foreach (var parent in parentComponents)
                    {
                        AppStatus AggregatedStatus = AppStatus.Up;
                        if (status == "Down" ||  parent.HeartBeatStatus == HeartBeatStatus.Down )
                        {
                            AggregatedStatus = AppStatus.Down;
                        }
                        else
                        {
                            //check through health message status and other children component messages to determine final status
                            //what of if the parent app is down, not sending heart beats this check would then not be sufficient
                            var checkStatus = HealthMessageUtility.GetStatusBasedOnHealthChecks(parent.AppID);
                            var appStatus = HealthMessageUtility.GetStatusBasedOnChildrenApps(parent);
                           // AggregatedStatus = HealthMessageUtility.ReturnAggregateStatus(checkStatus, appStatus);
                        }
                        //Update the Status of the Application in the database
                        parent.Status = AggregatedStatus;
                        parent.LastUpdate = DateTime.Now;
                        ComponentLogic.EditEntity(parent);
                        Clients.All.reportAppStatus(parent.AppID, AggregatedStatus.ToString());
                    }
                }
            }

        }

        public void UpdateHeartBeat(string appId, string status)
        { 
            //we save the heartbeat status
            Component component = ComponentLogic.GetByAppId(appId);
            if (component != null)
            {
                component.HeartBeatStatus = (HeartBeatStatus)Enum.Parse(typeof(HeartBeatStatus), status);
                if ((HeartBeatStatus)Enum.Parse(typeof(HeartBeatStatus), status) == HeartBeatStatus.Down)
                {
                    component.Status = AppStatus.Down;
                }
                ComponentLogic.EditEntity(component);
            }
        }
    }

    public class HealthMessageUtility
    {
      
        public static void SendHealthMessage(SelfHealthMessage healthMessage)
        {
          //  new HealthMessageLogic().SaveOrUpdateHealthStatus(healthMessage);
            Trace.TraceInformation("Health Messaged Received " + healthMessage.AppID + "Status is " + healthMessage.OverallStatus);
            AppStatus AggregatedStatus = AppStatus.Up;
            string failedMessagesJson = string.Empty;

            Trace.TraceInformation("About to get status based on health checks " + healthMessage.AppID);

            //Step 1: Get Aggregate Status of AppID depeding on status of related health messages
            var checkStatus = GetStatusBasedOnHealthChecks(healthMessage.AppID, healthMessage.IPAddress);
            Trace.TraceInformation("Check Status is " + checkStatus);

            Trace.TraceInformation("About to get Component " + healthMessage.AppID);

            //Step 2: Check if there is an App set up with this ID if there is, get agg status of APP depending on status of Child Apps.
            Component component = ComponentLogic.GetByAppId(healthMessage.AppID);
            if (component != null)
            {
                var appStatus = GetStatusBasedOnChildrenApps(component, AggregatedStatus);
                AggregatedStatus = ReturnAggregateStatus(checkStatus, appStatus);
                //Update the Status of the Application in the database
                component.Status = AggregatedStatus;
                component.LastUpdate = DateTime.Now;
                ComponentLogic.EditEntity(component);
            }
            Trace.TraceInformation("About to send info to client  " + healthMessage.AppID);
            string status = AggregatedStatus.ToString();
            var connect = GlobalHost.ConnectionManager.GetHubContext<HealthMessageHub>();
            connect.Clients.All.test();

            var message = JsonConvert.SerializeObject(healthMessage);
            connect.Clients.All.sendHealthMessage(message); // to health message ui
            connect.Clients.All.sendHealthStatus(message,AggregatedStatus.ToString());//to monitor ui
            Trace.TraceInformation("Health Info has been sent to client " );

        }
      
        public static List<AppStatus> GetStatusBasedOnReplicas(Component component, List<string> subComponentIPs)
        {
            List<AppStatus> replicaStatus = new List<AppStatus>();
            Trace.TraceInformation("Before Loop");
            if (subComponentIPs != null )
            {
                foreach (var replica in subComponentIPs)
                {
                    Trace.TraceInformation("In Loop" + replica);

                    replicaStatus.Add(GetStatusBasedOnHealthChecks(component.AppID, replica));

                    Trace.TraceInformation("done Loop");

                }
            }

            return replicaStatus;
        }
       

        public static void SendHeartBeat(SelfHealthMessage healthMessage)
        {
            GlobalHost.ConnectionManager.GetHubContext<HealthMessageHub>().Clients.All.sendHealthBeat(healthMessage.AppID, String.Format("{0:d/M/yyyy HH:mm:ss}", healthMessage.DateChecked));
        }

        public static AppStatus ReturnAggregateStatus(AppStatus childrenStatus, List<AppStatus> replicas)
        {
            AppStatus overallStatus = AppStatus.Up;
            var downReplicas = false;
            if (replicas != null && replicas.Count != 0)
            {
                 downReplicas = replicas.Contains(AppStatus.Down);
            }
                if (childrenStatus == AppStatus.Down && downReplicas)
                {
                    return AppStatus.PerformanceDown;
                }
                if (childrenStatus == AppStatus.Down)
                {

                    return AppStatus.Down;
                }
                if (downReplicas)
                {
                    return AppStatus.ReplicaDown;
                }
                if (childrenStatus == AppStatus.PerformanceDegraded || replicas.Contains(AppStatus.PerformanceDegraded))
                {
                    return AppStatus.PerformanceDegraded;
                }
            
            return overallStatus;

        }
        public static AppStatus ReturnAggregateStatus (AppStatus childrenStatus, AppStatus replica)
        {
            if(childrenStatus == AppStatus.Down && replica == AppStatus.Down)
            {
                return AppStatus.PerformanceDown;
            }
            if(childrenStatus == AppStatus.Down)
            {
                return AppStatus.ChildDown;
            }
            if(replica == AppStatus.Down)
            {
                return AppStatus.ReplicaDown;
            }
            if (childrenStatus == AppStatus.PerformanceDegraded || replica== AppStatus.PerformanceDegraded)
            {
                return AppStatus.PerformanceDegraded;
            }

            return AppStatus.Up;
        }

        public static AppStatus GetStatusBasedOnHealthChecks(string appID)
        {
            AppStatus AggregatedStatus = AppStatus.Up;
            string failedMessagesJson = string.Empty;
            List<SelfHealthMessage> unHealthyMessages = HealthMessageLogic.GetFailedChecks(appID);
            //Step 1: Get Aggregate Status of AppID depeding on status of related health messages
            if (unHealthyMessages != null && unHealthyMessages.Count > 0)
            {
                var failedMessages = unHealthyMessages.Where(x => x.OverallStatus == Contracts.CheckResultStatus.Down || x.OverallStatus == Contracts.CheckResultStatus.Unknown);
                if (failedMessages != null && failedMessages.Count() > 0)
                {
                    AggregatedStatus = AppStatus.Down;
                }
                else
                {
                    AggregatedStatus = AppStatus.PerformanceDegraded;
                }
                failedMessagesJson = JsonConvert.SerializeObject(unHealthyMessages);
            }
            return AggregatedStatus;
        }
        public static AppStatus GetStatusBasedOnHealthChecks(string appID, string IPAddress)
        {
            AppStatus AggregatedStatus = AppStatus.Up;
            string failedMessagesJson = string.Empty;
            List<SelfHealthMessage> unHealthyMessages = HealthMessageLogic.GetFailedChecks(appID, IPAddress);
            //Step 1: Get Aggregate Status of AppID depeding on status of related health messages
            if (unHealthyMessages != null && unHealthyMessages.Count > 0)
            {
                var failedMessages = unHealthyMessages.Where(x => x.OverallStatus == Contracts.CheckResultStatus.Down || x.OverallStatus == Contracts.CheckResultStatus.Unknown);
                if (failedMessages != null && failedMessages.Count() > 0)
                {
                    AggregatedStatus = AppStatus.Down;
                }
                else
                {
                    AggregatedStatus = AppStatus.PerformanceDegraded;
                }
                //failedMessagesJson = JsonConvert.SerializeObject(unHealthyMessages);
            }
            return AggregatedStatus;
        }


        public static AppStatus GetStatusBasedOnChildrenApps(Component component, AppStatus defaultStatus = AppStatus.Up)
        {
            AppStatus AggregatedStatus = defaultStatus;
            if (component.ChildrenComponents != null && component.ChildrenComponents.Count > 0)
            {
                var faultyComponents = component.ChildrenComponents.Where(x => x.Status != AppStatus.Up);
                if (faultyComponents != null && faultyComponents.Count() > 0)
                {
                    var downComponents = faultyComponents.Where(x => x.Status == AppStatus.Down);
                    
                    if (downComponents != null && downComponents.Count() > 0)
                    {
                        AggregatedStatus = AppStatus.ChildDown;
                    }
                    else
                    {
                        AggregatedStatus = AppStatus.PerformanceDegraded;
                    }
                }
            }
            return AggregatedStatus;
        }

    }
}
