using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Contracts;
using DejaVu.SelfHealthCheck.Engine;

namespace DejaVu.SelfHealthCheck
{
    public static class ConfigureExtensions
    {
        public static IRunner InitializeRunner(this IConfigure selfHealthCheckConfiguration)
        {
            return Runner.Initialize(selfHealthCheckConfiguration.SelfHealthCheckConfiguration as IConfiguration);
        }
    }
}
