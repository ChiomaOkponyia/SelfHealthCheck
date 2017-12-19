using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic
{
    public class CategoryLogic : EntityLogic<Category>
    {
       
        public static bool IsExists(string c)
        {
            try
            {
                if (!CategoryDb.CheckByName(c))
                {
                    return false;
                }
                else { return true; }
            }
            catch
            {
                return true;
            }
        }
    }
}
