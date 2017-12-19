using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Data;
using System.ServiceProcess;
using System.Security.Cryptography;
using System.Diagnostics;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic
{
    public class ComponentLogic : EntityLogic<Component>
    {
        public static Component GetByAppId(string appId)
        {
            try
            {
                return ComponentDb.GetByAppId(appId);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<Component> GetAllComponents()
        {
            try
            {
                return ComponentDb.GetAllComponents();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static IList<Component> GetParentComponents(Component childComponent)
        {
            try
            {
                return ComponentDb.GetParentComponents(childComponent);
            }
            catch (Exception)
            {
                throw;
            }
        }


        public static bool isExist(string appId, string appName, string exclusion, out string state)
        {
            try
            {
                state = string.Empty;
                var result = ComponentDb.GetByAppIdAndAppName(appId, appName, exclusion);
                if (result == null)
                {
                    return false;
                }
                else
                {
                    if (result.AppName.ToLower().Trim().Equals( appName.ToLower().Trim())) { state = "App Name"; }
                    else if (result.AppID == appId) { state = "App ID"; }
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public  bool isAppIDExist(string appID)
        {
            Component app = GetByAppId(appID);
            Trace.TraceInformation("Component with AppID {0} is {1} " ,appID, app == null);

            if (app != null)
            {
                return true;
            }
            return false;
        }
        public  string GenerateAppID()
        {
            int maxSize = 8;
            int minSize = 8;
            char[] chars = new char[62];
            string a;
            a = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890-_";
            chars = a.ToCharArray();
            int size = maxSize;
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            size = maxSize;
            data = new byte[size];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            { result.Append(chars[b % (chars.Length - 1) ]); }

            return result.ToString();
        }

        public static void SetUpAppForDejaVuCommands()
        {


            string machineName = Convert.ToString(System.Configuration.ConfigurationManager.AppSettings["DejaVuCommandsMachineName"]);
            if (string.IsNullOrEmpty(machineName))
            {
                machineName = "localhost";
            }
            var dejavuServices = ServiceController.GetServices(machineName).Where(x => x.ServiceName.ToLower().StartsWith("dejavu."));
            if (dejavuServices == null || !dejavuServices.Any()) return;
            List<Component> allComponents = GetAllComponents();
            if (allComponents == null)
            {
                allComponents = new List<Component>();
            }
            foreach (var dejavuService in dejavuServices)
            {
                string appId = dejavuService.ServiceName.Replace("DejaVu.", "").Replace("-1.0.0.0", "").Replace(" ", "");
                if (allComponents.Where(x => x.AppID == appId).Count() > 0) continue;
                Component component = new Component();
                component.AppID = appId;
                component.AppName = appId;
                AddNewEntity(component);
                allComponents.Add(component);
            }
        }
    }

}
