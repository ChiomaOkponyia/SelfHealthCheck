using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DejaVu.SelfHealthCheck.Contracts
{
    public interface ICustomCheck<T> : IConfigure
    {
        T Run(Func<ICheckConfiguration, ICheckResult, ICheckResult> customCheck);

        T Perform(Func<KeyValuePair<bool, string>> customCheck);
    }

    public interface ICustomConnectedCheck<T>
    {
        //Response Time Check
        ICustomCheck<T> WithResponseTime(double maxResponseTime);
    }

}
