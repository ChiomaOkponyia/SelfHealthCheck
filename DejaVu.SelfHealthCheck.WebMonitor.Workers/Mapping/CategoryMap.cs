using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Mapping
{
   public class CategoryMap:ClassMap<Category>
    {
        public CategoryMap()
          {
                Id(x => x.Id).GeneratedBy.Identity();
                Map(x => x.CategoryName).Not.Nullable();
                HasOne(x => x.Profile).Not.LazyLoad().Cascade.All();
            }
        }
}
