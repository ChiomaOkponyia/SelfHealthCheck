using DejaVu.SelfHealthCheck.Messages;
using DejaVu.SelfHealthCheck.Utility;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.Collector
{
    public class HealthMessageHub : Hub
    {

        public void LoadAllApplications()
        {
            List<Component> components = ComponentLogic.GetAllComponents();
            string data = JsonConvert.SerializeObject(components);
            Clients.All.loadAllApllications(data);
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
                            AggregatedStatus = HealthMessageUtility.ReturnAggregateStatus(checkStatus, appStatus);
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
            AppStatus AggregatedStatus = AppStatus.Up;
            string failedMessagesJson = string.Empty;

            //Step 1: Get Aggregate Status of AppID depeding on status of related health messages
            var checkStatus = GetStatusBasedOnHealthChecks(healthMessage.AppID);

            //Step 2: Check if there is a App set up with this ID if there is, get agg status of APP depending on status of Child Apps.
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

            string status = AggregatedStatus.ToString();
            GlobalHost.ConnectionManager.GetHubContext<HealthMessageHub>().Clients.All.addHealthMessage(healthMessage.AppID,
                                                                                                        healthMessage.Results[0].Title, 
                                                                                                        String.Format("{0:d/M/yyyy HH:mm:ss}",healthMessage.DateChecked), 
                                                                                                        String.Format("{0:HH:mm:ss}", healthMessage.DateChecked), 
                                                                                                        Convert.ToString(healthMessage.OverallStatus), 
                                                                                                        healthMessage.Results[0].TimeElasped, 
                                                                                                        healthMessage.Results[0].AdditionalInformation, 
                                                                                                        AggregatedStatus.ToString(), 
                                                                                                        failedMessagesJson);

        }

        public static void SendHealthBeat(SelfHealthMessage healthMessage)
        {
            GlobalHost.ConnectionManager.GetHubContext<HealthMessageHub>().Clients.All.sendHealthBeat(healthMessage.AppID, String.Format("{0:d/M/yyyy HH:mm:ss}", healthMessage.DateChecked));
        }

        public static AppStatus ReturnAggregateStatus(AppStatus firstStatus, AppStatus secondStatus)
        {
            if (firstStatus == AppStatus.Down || secondStatus == AppStatus.Down)
            {
                return AppStatus.Down;
            }
            else if (firstStatus == AppStatus.PerformanceDegraded || secondStatus == AppStatus.PerformanceDegraded)
            {
                return AppStatus.PerformanceDegraded;
            }
            else
            {
                return AppStatus.Up;
            }

        }

        public static AppStatus GetStatusBasedOnHealthChecks(string appID)
        {
            AppStatus AggregatedStatus = AppStatus.Up;
            string failedMessagesJson = string.Empty;
            List<Messages.SelfHealthMessage> unHealthyMessages = HealthMessageLogic.GetFailedChecks(appID);
            //Step 1: Get Aggregate Status of AppID depeding on status of related health messages
            if (unHealthyMessages != null && unHealthyMessages.Count > 0)
            {
                var failedMessages = unHealthyMessages.Where(x => x.OverallStatus == Contracts.CheckResultStatus.Down);
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
                        AggregatedStatus = AppStatus.Down;
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
