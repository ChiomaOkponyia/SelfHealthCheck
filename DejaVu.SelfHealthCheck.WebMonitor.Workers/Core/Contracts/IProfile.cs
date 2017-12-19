using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Core
{
    public interface IProfile: IEntity
    {
        string ProfileName { get; set; }
        IList<Category> Categories { get; set; }
    }
}
