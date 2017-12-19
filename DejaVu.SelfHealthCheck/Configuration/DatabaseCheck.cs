using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DejaVu.SelfHealthCheck.Contracts;
using DejaVu.SelfHealthCheck.Engine;

namespace DejaVu.SelfHealthCheck.Configuration
{
    [CheckRunner(typeof(DatabaseCheckRunner))]
    public class DatabaseCheck : BaseCheck, IDatabaseCheck, IDatabaseConfiguration, IDatabaseConnectedCheck
    {
        public DatabaseCheck(IConfigure selfHealthCheckConfiguration, string title)
        {
            SelfHealthCheckConfiguration = selfHealthCheckConfiguration;
            Title = title;
        }

        public IDatabaseConnectedCheck On(string connectionString)
        {
            this.ConnectionString = connectionString;
            return this;
        }

        public IDatabaseConnectedCheck On(string server, string database, string userId, string password)
        {
            //Build connection string
            string builtConnectionString = "";

            this.ConnectionString = builtConnectionString;
            return this;
        }

        public IDatabaseConnectedCheck UsingNhibernateConfig()
        {
            //Get connection string from NHibernate
            string nhibernateConnectionString = "";

            this.ConnectionString = nhibernateConnectionString;
            return this;
        }

        

        public string ConnectionString
        {
            get;
            set;
        }

        public string[] TablesToCheck
        {
            get;
            set;
        }

        public double ResponseTime
        {
            get;
            set;
        }

        public IDatabaseConnectedCheck TestingSelectOn(params string[] tables)
        {
            this.TablesToCheck = tables;
            return this;
        }

        public IDatabaseConnectedCheck WithResponseTime(double maxResponseTime)
        {
            this.ResponseTime = maxResponseTime;
            return this;
        }

        public IDatabaseConnectedCheck Run(Func<ICheckConfiguration, ICheckResult, ICheckResult> customCheck)
        {
            AfterCheckAction = customCheck;

            return this;
        }


        public IDatabaseConnectedCheck Perform(Func<KeyValuePair<bool, string>> customCheck)
        {
            throw new NotImplementedException();
        }
    }
}
