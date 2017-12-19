using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Core;
using DejaVu.SelfHealthCheck.WebMonitor.Workers.Data;

namespace DejaVu.SelfHealthCheck.WebMonitor.Workers.Logic
{
    public class EntityLogic<T> where T : class, IEntity, new()
    {
        public static bool AddNewEntity(T entity)
        {
            try
            {
                EntityDb<T>.Save(entity);
                EntityDb<T>.CommitChanges();
                EntityDb<T>.CloseSession();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool EditEntity(T entity)
        {
            try
            {
                EntityDb<T>.SaveOrUpdate(entity);
                EntityDb<T>.CommitChanges();
                EntityDb<T>.CloseSession();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool AddNewWithoutCommit(T entity)
        {
            try
            {
                EntityDb<T>.Save(entity);
               
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool EditWithoutCommit(T entity)
        {
            try
            {
                EntityDb<T>.SaveOrUpdate(entity);
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static bool Commit()
        {
            try
            {
                EntityDb<T>.CommitChanges();
                EntityDb<T>.CloseSession();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<T> GetEntities(Func<T, bool> query)
        {
            try
            {
                List<T> results = new List<T>();
                results = EntityDb<T>.GetByLinqQuery(query);
                EntityDb<T>.CloseSession();
                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }
         
        public static List<T> PagedGetEntities(System.Linq.Expressions.Expression<Func<T, bool>> query, int start, int pageSize, out int totalCount)
        {
            try
            {
                List<T> results = new List<T>();
                results = EntityDb<T>.PagedGetByLinqQuery(query, start, pageSize, out totalCount);
                EntityDb<T>.CloseSession();
                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<T> GetAllEntities()
        {
            try
            {
                List<T> results = new List<T>();
                results = EntityDb<T>.GetAll();
                EntityDb<T>.CloseSession();
                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static List<T> PagedGetAllEntities(int start, int pageSize, out int totalCount)
        {
            try
            {
                List<T> results = new List<T>();
                results = EntityDb<T>.PagedGetAll(start, pageSize, out totalCount);
                EntityDb<T>.CloseSession();
                return results;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
