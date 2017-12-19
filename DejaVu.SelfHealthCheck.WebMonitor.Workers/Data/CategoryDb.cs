using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Data
{
    public class CategoryDb : EntityDb<Category>
    {
        public static bool CheckByName(string c)
        {
            var session = DatabaseManager.GetSession();
            var criteria = session.CreateCriteria<Category>();
            criteria = criteria.Add(Restrictions.Eq("CategoryName", c));
            //Profile profile = new Profile();
            if (criteria.List<Category>().Count == 0)
            {
                return false;
            }
            else return true;
        }
    }
}
