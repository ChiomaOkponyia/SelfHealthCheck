using DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace DejaVu.SelfHealthCheck.MonitorNew
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            bool setUpCommand = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["SetUpDejaVuCommands"]);
            if (setUpCommand)
            {
                ComponentLogic.SetUpAppForDejaVuCommands();
            }
        }
    }
}