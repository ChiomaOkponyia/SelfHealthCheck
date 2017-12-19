using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Contracts;
using NServiceBus;

namespace DejaVu.SelfHealthCheck.Configuration
{
    public class Configure : IConfigure, IConfiguration
    {
        private const double DEFAULT_CHECK_INTERVAL = 5000;

        public IConfigure SelfHealthCheckConfiguration
        {
            get
            {
                return this; 
            }
        }

        public double CheckInterval
        {
            get;
            set;
        }

        public string AppID
        {
            get;
            set;
        }

        public string IPAddress
        {
            get;
            set;
        }
        public DateTime NextCheckTime
        {
            get;
            set;
        }
        public IList<ICheckConfiguration> Checks
        {
            get;
            set;
        }

        private Configure(string appId)
        {
            AppID = appId; 
            this.CheckInterval = DEFAULT_CHECK_INTERVAL;
            this.Checks = new List<ICheckConfiguration>();
        }
        
        public static IConfigure AsSelfHealthCheckFor(string applicationId)
        {
            //Initialize Bus
          //  Global.Init();

            return new Configure(applicationId);
        }
        public static IConfigure AsSelfHealthCheckFor(string applicationId, IBus serviceBusToUse)
        {
            //Initialize Bus
            //Global.Bus = serviceBusToUse;
            return new Configure(applicationId);
        }



        public IConfigure WithCheckInterval(double interval)
        {
            this.CheckInterval = interval;
            return this;
        }

        public IConfigure WithIPAddress(string IPAddress)
        {
            this.IPAddress = IPAddress;
            return this;
        }


        public T With<T, R>(string title)
            where T : IConfigure
            where R : class,T, ICheckConfiguration
        {
            R check = Activator.CreateInstance(typeof(R), this) as R;
            this.Checks.Add(check);
            return check;
        }

        public INetworkCheck WithNetworkCheck(string title)
        {
            var networkCheck = new NetworkCheck(this, title);
            this.Checks.Add(networkCheck);
            return networkCheck;
        }

        public IDatabaseCheck WithDatabaseCheck(string title)
        {
            var databaseCheck = new DatabaseCheck(this, title);
            this.Checks.Add(databaseCheck);
            return databaseCheck;
        }

        public IUrlCheck WithUrlCheck(string title)
        {
            var urlCheck = new UrlCheck(this, title);
            this.Checks.Add(urlCheck);
            return urlCheck;
        }

        public ICustomCheck<CustomCheck> WithCustomCheck(string title)
        {
            var customCheck = new CustomCheck(this, title);
            this.Checks.Add(customCheck);
            return customCheck;
        }

       

      

    }
}
