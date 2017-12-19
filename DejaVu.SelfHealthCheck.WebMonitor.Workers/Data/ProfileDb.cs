using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using NHibernate.Criterion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Data
{
    public class ProfileDb : EntityDb<Profile>
    {
        public static bool CheckByName(string profileName)
        {
            var session = DatabaseManager.GetSession();
            var criteria = session.CreateCriteria<Profile>();
            criteria = criteria.Add(Restrictions.Eq("ProfileName", profileName));
            //Profile profile = new Profile();
            if (criteria.List<Profile>().Count == 0)
            {
                return false;
                //var query = session.QueryOver<Profile>();
                //try
                //{
                //    profile = query.Where(x => x.ProfileName.ToUpper() == profileName.ToUpper()).SingleOrDefault();

                //}
                //catch(Exception ex)
                //{

                //}

            }
            else
            {
                return true;
            }
        }
    }
}
