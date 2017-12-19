using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Mapping
{
    public class ProfileMap:ClassMap<Profile>
    {
        public ProfileMap()
        {
            Id(x => x.Id).GeneratedBy.Identity();
            Map(x => x.ProfileName).Not.Nullable();
            HasMany(x => x.Categories).Not.LazyLoad().Cascade.All();
        }
    }
}
