using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Contracts;

namespace DejaVu.SelfHealthCheck.Configuration
{
    public abstract class BaseCheck : IConfigure, ICheckConfiguration
    {
        public string Title { get; set; }

        public Func<ICheckConfiguration, ICheckResult, ICheckResult> AfterCheckAction
        {
            get;
            set;
        }

        public IConfigure SelfHealthCheckConfiguration
        {
            get;
            protected set;
        }

        public T With<T, R>(string title)
            where T : IConfigure
            where R : class,T, ICheckConfiguration
        {
            return this.SelfHealthCheckConfiguration.With<T, R>(title);
        }

        public IConfigure WithCheckInterval(double interval)
        {
            return this.SelfHealthCheckConfiguration.WithCheckInterval(interval);
        }
        public IConfigure WithIPAddress(string IPAddress)
        {
            return this.SelfHealthCheckConfiguration.WithIPAddress(IPAddress);
        }
        public INetworkCheck WithNetworkCheck(string title)
        {
            return this.SelfHealthCheckConfiguration.WithNetworkCheck(title);
        }

        public IDatabaseCheck WithDatabaseCheck(string title)
        {
            return this.SelfHealthCheckConfiguration.WithDatabaseCheck(title);
        }

        public IUrlCheck WithUrlCheck(string title)
        {
            return this.SelfHealthCheckConfiguration.WithUrlCheck(title);
        }

        public ICustomCheck<CustomCheck> WithCustomCheck(string title)
        {
            return this.SelfHealthCheckConfiguration.WithCustomCheck(title);
        }

       
    }
}
