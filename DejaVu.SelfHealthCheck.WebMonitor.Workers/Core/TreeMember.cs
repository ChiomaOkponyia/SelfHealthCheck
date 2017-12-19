using DejaVu.SelfHealthCheck.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Core
{
    public class TreeMember
    {
        public string Id { get; set; }
        public string AppID { get; set; }
        public bool HasSubComponents { get; set; }
        public string ParentComponentId { get; set; }
        public string AppName { get; set; }
        public IList<Component> Components { get; set; }
        public CheckResultStatus Status { get; set; }
        public DateTime DateChecked { get; set; }
        public string RowClientId { get; set; }
        public string StatusCellClientId { get; set; }
        public string DateCellClientId { get; set; }
    }
}
