using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Core
{
    public class Entity : IEntity
    {
        [JsonProperty]
        public virtual long Id { get; set; }
    }

    public interface IEntity
    {
        long Id { get; set; }
    }
}
