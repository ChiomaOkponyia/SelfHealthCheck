using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using FluentNHibernate.Mapping;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Mapping
{
    public class ComponentMap : ClassMap<Component>
    {
        public ComponentMap()
        {
            Id(x => x.Id);
            Map(x => x.AppID).Unique();
            Map(x => x.AppName);
            Map(x => x.Status);
            Map(x => x.HeartBeatStatus);
            Map(x => x.LastUpdate).Nullable();
            HasManyToMany(x => x.ChildrenComponents)
            .ParentKeyColumn("ParentId")
            .ChildKeyColumn("ChildId")
            .Table("Component_ChildComponent");
            //Map(x => x.HasSubComponents).Not.Nullable();
            Map(x => x.ParentComponentId);
            //Map(x => x.Status);
            //Map(x => x.DateChecked);
            //Map(x => x.IsRootComponent).Not.Nullable();
            //HasManyToMany(x => x.ChildrenComponents).Not.LazyLoad().Cascade.All();
        }
    }
}
