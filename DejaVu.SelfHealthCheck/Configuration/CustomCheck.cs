using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Contracts;
using DejaVu.SelfHealthCheck.Engine;

namespace DejaVu.SelfHealthCheck.Configuration
{
    [CheckRunner(typeof(CustomCheckRunner))]
    public class CustomCheck : BaseCheck, ICustomCheck<CustomCheck>, ICustomConfiguration, ICustomConnectedCheck<CustomCheck>
    {
        public double ResponseTime
        {
            get;
            set;
        }

        public Func<KeyValuePair<bool, string>> TheCustomCheck
        {
            get;
            set;
        }

        public CustomCheck(IConfigure selfHealthCheckConfiguration, string title)
        {
            SelfHealthCheckConfiguration = selfHealthCheckConfiguration;
            Title = title;
        }

        public ICustomCheck<CustomCheck> WithResponseTime(double maxResponseTime)
        {
            this.ResponseTime = maxResponseTime;
            return this;
        }

        public CustomCheck Perform(Func<KeyValuePair<bool, string>> customCheck)
        {
            TheCustomCheck = customCheck;
            return this;
        }

        public CustomCheck Run(Func<ICheckConfiguration, ICheckResult, ICheckResult> customCheck)
        {
            AfterCheckAction = customCheck;

            return this;
        }


    }
}
