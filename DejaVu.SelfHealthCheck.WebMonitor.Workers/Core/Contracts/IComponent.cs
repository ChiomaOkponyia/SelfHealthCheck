using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Core
{
    public interface IComponent : IEntity
    {
        string AppID { get; set; }
        bool HasSubComponents { get; set; }
        string ParentComponentId { get; set; }
        string AppName { get; set; }
    }
}
