using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Data;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic
{
   public class ProfileLogic:EntityLogic<Profile>
    {
        public static bool IsExists(string profileName)
        {
            try
            {
                if (!ProfileDb.CheckByName(profileName))
                {
                    return false;
                }
                else { return true; }
            }
            catch(Exception ex)
            {
                return true;
            }
        }
    }
}
