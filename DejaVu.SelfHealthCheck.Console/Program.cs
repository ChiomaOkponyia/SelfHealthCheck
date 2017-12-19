using DejaVu.SelfHealthCheck.Configuration;
using DejaVu.SelfHealthCheck.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var appId = "3CCA1576-5D00-4005-917D-DFBBD0D1C7A8";
            string IPAddress = "62.173.32.67";
            var config = Configure
                .AsSelfHealthCheckFor(appId)
                .WithCheckInterval(5000)
                .WithIPAddress(IPAddress)
                .WithDatabaseCheck("Database Check")
                    .On("server=.;initial catalog=DejaVuSelfHealthCheck;user id=sa;password=P@ssw0rd;")
                    .TestingSelectOn("dbo.Component")
                    .WithResponseTime(2000)
                    .Run((x, y) =>
                    {
                        if (y.Status == CheckResultStatus.Up) y.AdditionalInformation = "MOST DEF. UP";
                        return y;
                    })
                      .WithDatabaseCheck("Database Check 2")
                    .On("server=.;initial catalog=DejaVuSelfHealthCheck;user id=sa;password=P@ssw0rd;")
                    .TestingSelectOn("dbo.Profile")
                    .WithResponseTime(2000)
                    .Run((x, y) =>
                    {
                        if (y.Status == CheckResultStatus.Up) y.AdditionalInformation = "MOST DEF. UP";
                        return y;
                    })
                .WithUrlCheck("URL Fail Check")
                    .On("https://www.google.com.ng")
                    .WithResponseTime(15000)

                .WithUrlCheck("URL  Check2")
                    .On("https://www.google.com.ng")
                    .WithResponseTime(15000)
                .WithCustomCheck("Custom Check")
                .Perform(() =>
                {
                    bool check = true;
                    if (check)
                    {
                        return new KeyValuePair<bool, string>(true, "Sucessfully Checked");
                    }
                    else
                    {
                        return new KeyValuePair<bool, string>(false, "Error while checking");
                    }
                })
                .WithResponseTime(5000)
                .InitializeRunner();
            System.Threading.Thread.Sleep(900000);
        }
    }
     
}
