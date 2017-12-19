using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DejaVu.SelfHealthCheck.Contracts
{
    public interface IUrlCheck : IConfigure
    {
        //Network Connection Parameters Setup
        IUrlConnectedCheck On(string url);
    }

    public interface IUrlConnectedCheck : ICustomCheck<IUrlConnectedCheck>
    {
        //Connect Check

        //Response Time Check
        IUrlConnectedCheck WithResponseTime(double maxResponseTime);
    }
}
