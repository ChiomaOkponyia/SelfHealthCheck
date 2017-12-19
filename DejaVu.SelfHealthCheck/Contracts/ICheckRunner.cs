using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DejaVu.SelfHealthCheck.Contracts
{
    public interface ICheckRunner
    {
        ICheckResult Check(ICheckConfiguration checkDetails);
    }
}
