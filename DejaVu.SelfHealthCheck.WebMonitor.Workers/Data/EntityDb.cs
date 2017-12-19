using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
//using NHibernate.ByteCode.Castle;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Data
{
    public class EntityDb<T> where T : class, IEntity, new()
    {
        private static Type persistentType
        {
            get
            {
                return typeof(T);
            }
        }

        protected static NHibernateSessionManager DatabaseManager
        {
            get
            {
                return NHibernateSessionManager.Instance;
            }
        }

        public static T GetById(int id)
        {
            T entry;
            try
            {
                var session = DatabaseManager.GetSession();
                entry = (T)session.Get(persistentType.Name, id);
            }
            catch (Exception)
            {
                throw;
            }

            return entry;
        }

        public static List<T> GetAll()
        {
            List<T> allEntries = new List<T>();
            try
            {
                var session = DatabaseManager.GetSession();
                allEntries = session.Query<T>().ToList();
            }
            catch (Exception)
            {
                throw;
            }

            return allEntries;
        }

        public static List<T> PagedGetAll(int start, int pageSize, out int totalCount)
        {
            List<T> allEntries = new List<T>();
            try
            {
                var session = DatabaseManager.GetSession();
                var query = session.Query<T>();
                var results = query.Skip(start).Take(pageSize).ToFuture();
                var resultsCount = query.ToFutureValue(x => x.Count());
                if (results != null)
                {
                    allEntries = results.ToList();
                }
                totalCount = resultsCount.Value;
            }
            catch (Exception)
            {
                throw;
            }

            return allEntries;
        }

        public static List<T> GetByLinqQuery(Func<T, bool> query)
        {
            var results = new List<T>();
            try
            {
                var session = DatabaseManager.GetSession();
                var tmp = session.Query<T>().Where(query);
                if (tmp != null)
                {
                    results = tmp.ToList();
                }

                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<T> PagedGetByLinqQuery(System.Linq.Expressions.Expression<Func<T, bool>> query, int start, int pageSize, out int totalCount)
        {
            var results = new List<T>();
            try
            {
                var session = DatabaseManager.GetSession();
                var linqQuery = session.Query<T>().Where(query);
                var tmp = linqQuery.Skip(start).Take(pageSize).ToFuture();
                var tmpCount = linqQuery.ToFutureValue(x => x.Count());
                if (tmp != null)
                {
                    results = tmp.ToList();
                }
                totalCount = tmpCount.Value;
                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static T Save(T entity)
        {
            try
                {
                    var session = DatabaseManager.GetSession();
                    DatabaseManager.BeginTransaction();
                    session.Save(entity);
                }
                catch (Exception)
                {
                    //DatabaseManager.RollbackTransaction();
                    throw;
                }
                return entity;
            }
        

        public static T SaveOrUpdate(T entity)
        {
            

                try
                {
                var session = DatabaseManager.GetSession();

                    DatabaseManager.BeginTransaction();
                    session.SaveOrUpdate(entity);
                }
                catch (Exception)
                {
                    //DatabaseManager.RollbackTransaction();
                    throw;
                }
            

            return entity;
        }

        public static void CommitChanges()
        {
            try
            {
                if (DatabaseManager.HasOpenTransaction())
                {
                    DatabaseManager.CommitTransaction();
                    DatabaseManager.CloseSession();
                }
                else
                {
                    DatabaseManager.GetSession().Flush();
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void CloseSession()
        {
            try
            {
                DatabaseManager.CloseSession();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static ISession BuildSession()
        {
            try
            {
                return DatabaseManager.GetSession();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
