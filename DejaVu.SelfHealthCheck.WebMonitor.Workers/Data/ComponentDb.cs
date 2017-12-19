using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using System.Collections;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Data
{
    public class ComponentDb : EntityDb<Component>
    {
        public static List<Component> Find(string appName, string appID, int page, int pageSize, string sort, string direction, out int totalItemsCount)
        {
            List<Component> result = new List<Component>();
            ISession session = BuildSession();
            try
            {
                ICriteria criteria = session.CreateCriteria(typeof(Component));


                //Order in Ascending order of Name
                if (!String.IsNullOrEmpty(appID) && !String.IsNullOrEmpty(appID.Trim()))
                {
                    criteria.Add(Expression.Or(Expression.Like("AppName", appID.Trim(), MatchMode.Anywhere),Expression.Eq("AppID", appID)));
                }
                result = criteria.List<Component>() as List<Component>;

                //if (!String.IsNullOrEmpty(appID) && !String.IsNullOrEmpty(appID.Trim()))
                //{
                //    criteria.Add(Expression.Eq("AppID", appID));
                //}

                //result = criteria.List<Component>() as List<Component>;

                if (result != null)
                    totalItemsCount = result.Count;
                else totalItemsCount = 0;
                result = new List<Component>();
              

                //Before doing the sorting, i get a count Criteria so that it doesnt crash.
                ICriteria countCriteria = CriteriaTransformer.Clone(criteria).SetProjection(Projections.RowCountInt64());
                ICriteria listCriteria = CriteriaTransformer.Clone(criteria).SetFirstResult(page * pageSize).SetMaxResults(pageSize);


                //This section then performs the sort operations on the list. Sorting defaults to the Name column
                if (direction == "Default")
                {
                    //listCriteria.AddOrder(Order.Asc("LastName"));
                    listCriteria.AddOrder(Order.Asc("Id"));
                }
                else
                {
                    if (direction == "DESC")
                    {
                        listCriteria.AddOrder(Order.Desc(sort));
                    }
                    else
                    {
                        listCriteria.AddOrder(Order.Asc(sort));
                    }
                }
                //Add the two criteria to the session and retrieve their result.
                //IList allResults = session.CreateMultiCriteria().Add(listCriteria).Add(countCriteria).List();
                IList allResults = listCriteria.List();
                foreach (var o in allResults)
                {
                    result.Add((Component)o);
                }

            }
            catch
            {
                throw;
            }

            return result;

        }



        public static Component GetByAppId(string appId)
        {
            Component result = null;
            try
            {
                var session = DatabaseManager.GetSession();
                result = session.QueryOver<Component>().Where(x => x.AppID == appId).SingleOrDefault();
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }


        public static Component GetByAppIdAndAppName(string appId, string appName, string exclusion)
        {
            Component result = null;
            try
            {
                var session = DatabaseManager.GetSession();
                var query  = session.QueryOver<Component>().Where(x => x.AppID == appId || x.AppName == appName);
                if (!string.IsNullOrEmpty(exclusion))
                {
                    query.Where(x => x.Id != long.Parse(exclusion));
                }
                result = query.SingleOrDefault();

            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }
        public static IList<Component> GetParentComponents(Component childComponent)
        {
            IList<Component> result = null;
            try
            {
                var session = DatabaseManager.GetSession();
                result = session.Query<Component>().Where(x => x.ChildrenComponents.Contains(childComponent)).ToList();
            }
            catch (Exception)
            {
                throw;
            }

            return result;
        }

        public static List<Component> GetAllComponents()
        {
            List<Component> result = new List<Component>();
            ISession session = BuildSession();
            try
            {
                ICriteria criteria = session.CreateCriteria(typeof(Component));
                result = criteria.List<Component>() as List<Component>;
                List<Component> childrenComp;
                foreach(Component c in result)
                {

                }
            }
            catch
            {
                throw;
            }

            return result;

        }

    }
}
