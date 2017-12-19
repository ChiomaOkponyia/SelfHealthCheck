using DejaVu.SelfHealthCheck.Contracts;
using DejaVu.SelfHealthCheck.Utility;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Data;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic;
using System.Diagnostics;

namespace DejaVu.SelfHealthCheck.MonitorNew.Controllers
{
    [System.Web.Http.RoutePrefix("api/HealthMessage")]

    public class HealthMessageController : ApiController
    {
        [System.Web.Http.Route("ReceiveMessage")]

        public void Process([FromBody]SelfHealthMessage message)
        {
            var connectionString = System.Configuration.ConfigurationManager.AppSettings["ConnectionString"];
            HealthMessageDAO messageDao = new HealthMessageDAO(connectionString);
            try
            {
                if (message.OverallStatus == CheckResultStatus.HealthBeat)
                {
                    Console.WriteLine("It's an heart Beat");
                    HealthMessageUtility.SendHeartBeat(message);
                }
                else
                {
                    Trace.TraceInformation("Health Messaged Received {0} , IP Address {1}" , message.AppID, message.IPAddress);

                    messageDao.Save(message);
                    Trace.TraceInformation("Health Messaged Saved....Proceeding to save or update status " + message.AppID);

                    //if u are configured as an app then save in the second table and send health message

                    if (new ComponentLogic().isAppIDExist(message.AppID))
                    {
                        new HealthMessageLogic().SaveOrUpdateHealthStatus(message);
                        Trace.TraceInformation("About to Send Health Message ");

                        HealthMessageUtility.SendHealthMessage(message);

                        Trace.TraceInformation("Health Message Sent ");

                    }
                }
            }
            catch (Exception ex)
            {
                new LogWriter(string.Format("Error Message- {0}: Stack Trace- {1} :Inner Exception- {2}", ex.Message, ex.StackTrace, ex.InnerException == null ? "" : ex.InnerException.Message));
            }
        }

        [System.Web.Http.Route("Test")]
        public string TestApi()
        {
            return "chioma";
        }
    }
}
