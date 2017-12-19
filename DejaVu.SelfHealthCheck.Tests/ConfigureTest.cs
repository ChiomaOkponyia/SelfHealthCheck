using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DejaVu.SelfHealthCheck.Configuration;
using DejaVu.SelfHealthCheck.Contracts;
using DejaVu.SelfHealthCheck;
using DejaVu.SelfHealthCheck.Engine;

namespace DejaVu.SelfHealthCheck.Tests
{
    [TestClass]
    public class ConfigureTest
    {
        [TestMethod]
        public void DatabaseSetupTest()
        {
            var appId = "app";

            var config = Configure
                .AsSelfHealthCheckFor(appId)
                .WithDatabaseCheck("Db Check")
                    .On("server=;database=;user id=;password=;")
                    .WithResponseTime(200);

            var configInfo = config.SelfHealthCheckConfiguration as IConfiguration;

            var configT = Configure
               .AsSelfHealthCheckFor(appId)
               .With<IDatabaseCheck, DatabaseCheck>("Db Check")
                   .On("server=;database=;user id=;password=;")
                   .WithResponseTime(200);

            var configInfoT = configT.SelfHealthCheckConfiguration as IConfiguration;

            var config2 = Configure
                .AsSelfHealthCheckFor(appId)
                .WithDatabaseCheck("Db 2 Check")
                   .On("server", "database", "userId", "password")
                   .WithResponseTime(100);

            var configInfo2 = config2.SelfHealthCheckConfiguration as IConfiguration;

            var config3 = Configure
                .AsSelfHealthCheckFor(appId)
                .WithDatabaseCheck("Db 3 Check")
                   .UsingNhibernateConfig()
                   .WithResponseTime(500);

            var configInfo3 = config3.SelfHealthCheckConfiguration as IConfiguration;

            var config4 = Configure
                .AsSelfHealthCheckFor(appId)
                .WithDatabaseCheck("Db 4 Check")
                    .On("server", "database", "userId", "password")
                    .TestingSelectOn("Customers", "Accounts")
                    .WithResponseTime(100)
                .WithDatabaseCheck("Db 5 Check")
                    .On("server", "database", "userId", "password")
                    .WithResponseTime(100);

            var configInfo4 = config4.SelfHealthCheckConfiguration as IConfiguration;


        }

        [TestMethod]
        public void NetworkSetupTest()
        {
            var appId = "app";

            var config = Configure
                .AsSelfHealthCheckFor(appId)
                .WithNetworkCheck("Network 1 Check")
                    .On("hostname", 1000)
                    .WithResponseTime(200);

            var configInfo = config.SelfHealthCheckConfiguration as IConfiguration;

            var config2 = Configure
                .AsSelfHealthCheckFor(appId)
                .WithNetworkCheck("Network 2 Check")
                    .On("hostname", 1000, true)
                    .NoConnect()
                    .WithResponseTime(200);

            var configInfo2 = config2.SelfHealthCheckConfiguration as IConfiguration;
        }

        [TestMethod]
        public void UrlSetupTest()
        {
            var appId = "app";

            var config = Configure
                .AsSelfHealthCheckFor(appId)
                .WithUrlCheck("Google URL Check")
                    .On("http://www.google.com")
                    .WithResponseTime(2000);



            var configInfo = config.SelfHealthCheckConfiguration as IConfiguration;

            config.InitializeRunner();

            System.Threading.Thread.Sleep(900000);
        }


        [TestMethod]
        public void AppTest()
        {
            var appId = "{3CCA1576-5D00-4005-917D-DFBBD0D1C7A8}";

            var config = Configure
                .AsSelfHealthCheckFor(appId)
                .WithCheckInterval(5000)
                .WithDatabaseCheck("Database Check")
                    .On("server=.;initial catalog=AppZoneSwitch;user id=sa;password=P@ssw0rd;")
                    .TestingSelectOn("Transactions")
                    .WithResponseTime(2000)
                    .Run((x, y) =>
                        {
                            if (y.Status == CheckResultStatus.Up) y.AdditionalInformation = "MOST DEF. UP";
                            return y;
                        })
                //.WithNetworkCheck("Router Check")
                //    .On("165.233.246.12", 3389)
                //   .WithResponseTime(1000)
                .WithNetworkCheck("Internet Check")
                    .On("62.173.32.45", 3389)
                    .WithResponseTime(10)
                .WithUrlCheck("MFB Service Check")
                    .On("http://localhost/managedservices/services/mfbservice.svc")
                    .WithResponseTime(5000)
                .WithUrlCheck("URL Fail Check")
                    .On("http://localhost/managedservices/services/notthere")
                    .WithResponseTime(5000)
                .WithCustomCheck("Custom Check")
                .Perform(() =>
                {
                    bool check = false;
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
            //config.InitializeRunner();

            System.Threading.Thread.Sleep(900000);
        }
    }
}
