using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.Collector
{
    public class HealthMessageLogic
    {

        public static List<Messages.SelfHealthMessage> GetFailedChecks(string appID)
        {
            HealthMessageDAO dao = new HealthMessageDAO();
            return dao.ReturnLastUniqueMessagesWithStatusNotUp(appID);
        }
    }
}
