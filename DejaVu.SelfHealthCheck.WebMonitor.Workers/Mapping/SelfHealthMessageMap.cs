using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Mapping;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Mapping
{
    class SelfHealthMessageMap : ClassMap<SelfHealthMessage>
    {
        public SelfHealthMessageMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.AppID).Not.Nullable();
            Map(x => x.Title);
            Map(x => x.TimeElapsed);
            Map(x => x.DateChecked);
            Map(x => x.AdditionalInformation);
            Map(x => x.OverallStatus);
            Map(x => x.IPAddress);
        }

    }
}
