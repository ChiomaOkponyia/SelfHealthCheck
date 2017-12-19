using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DejaVu.SelfHealthCheck.Contracts
{
    public interface IDatabaseCheck : IConfigure
    {
        //Database Connection Parameters Setup
        IDatabaseConnectedCheck On(string connectionString);
        IDatabaseConnectedCheck On(string server, string database, string userId, string password);
        IDatabaseConnectedCheck UsingNhibernateConfig();
    }

    public interface IDatabaseConnectedCheck : ICustomCheck<IDatabaseConnectedCheck>
    {
        //Table select Check
        IDatabaseConnectedCheck TestingSelectOn(params string[] tables);

        //Response Time Check
        IDatabaseConnectedCheck WithResponseTime(double maxResponseTime);
    }
}
