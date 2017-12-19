using DejaVu.SelfHealthCheck.WebMonitor.Workers.Data;
using System;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic
{
    public class HealthMessageLogic
    {

        public static List<SelfHealthMessage> GetFailedChecks(string appID)
        {
            HealthMessageDAO dao = new HealthMessageDAO();
            return dao.ReturnLastUniqueMessagesWithStatusNotUp(appID);
        }
        public static List<SelfHealthMessage> GetFailedChecks(string appID, string IPAddress)
        {
            return new HealthMessageDAO().GetMostRecentFailedHealthMessageChecks(appID, IPAddress);
        }
        public void SaveOrUpdateHealthStatus(SelfHealthMessage healthMessage)
        {
            HealthMessageDAO dao = new HealthMessageDAO();

            SelfHealthMessage healthStatus = dao.GetCheckTypeForApp(healthMessage);

            if(healthStatus == null)
            {
                Trace.TraceInformation("Health Status about to save");

                dao.SaveHealthStatus(healthMessage);
                Trace.TraceInformation("Health Status Saved Succesfully");

            }
            else
            {
                Trace.TraceInformation("Health Status about to update");

                dao.UpdateHealthStatus(healthMessage);
                Trace.TraceInformation("Health Status Updated Succesfully");

            }

        }

        public List<string> GetIPAddressesForApp(string appId)
        {
            return new HealthMessageDAO().GetAllIPAddressesForApp(appId);
        }
    }
}
