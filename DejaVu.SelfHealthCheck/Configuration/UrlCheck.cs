using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Contracts;
using DejaVu.SelfHealthCheck.Engine;

namespace DejaVu.SelfHealthCheck.Configuration
{
    [CheckRunner(typeof(UrlCheckRunner))]
    public class UrlCheck : BaseCheck, IUrlCheck, IUrlConfiguration, IUrlConnectedCheck
    {
        public string Url
        {
            get;
            set;
        }

        public double ResponseTime
        {
            get;
            set;
        }

        public UrlCheck(IConfigure selfHealthCheckConfiguration, string title)
        {
            SelfHealthCheckConfiguration = selfHealthCheckConfiguration;
            Title = title;
        }

        public IUrlConnectedCheck On(string url)
        {
            this.Url = url;

            return this;
        }

        public IUrlConnectedCheck WithResponseTime(double maxResponseTime)
        {
            this.ResponseTime = maxResponseTime;

            return this;
        }




        public IUrlConnectedCheck Run(Func<ICheckConfiguration, ICheckResult, ICheckResult> customCheck)
        {
            AfterCheckAction = customCheck;

            return this;
        }


        public IUrlConnectedCheck Perform(Func<KeyValuePair<bool, string>> customCheck)
        {
            throw new NotImplementedException();
            
        }

    }
}
