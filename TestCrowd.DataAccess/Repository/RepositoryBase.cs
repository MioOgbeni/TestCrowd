using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using TestCrowd.DataAccess.Interface;
using TestCrowd.DataAccess.Interface.Repository;

namespace TestCrowd.DataAccess.Repository
{
    public class RepositoryBase<T> : IRepositoryBase<T> where T :class, IEntity
    {
        protected ISession Session;

        public RepositoryBase()
        {
            Session = NHibernateHelper.Session;
        }

        public IList<T> GetAll()
        {
            return Session.QueryOver<T>().List();
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await Session.QueryOver<T>().ListAsync();
        }

        public IList<T> GetEntities(out int totalCount, out int filteredCount, int startIndex = 0, int requestedCount = 0, string filterColumn = null, string filterValue = null, string orderPropertyName = null,
            bool asc = true)
        {
            ICriteria criteria = Session.CreateCriteria<T>();

            if (!string.IsNullOrEmpty(filterColumn))
            {
                criteria.Add(Restrictions.Like(filterColumn, $"%{filterValue}%"));
            }

            filteredCount = criteria.List<T>().Count;

            if (requestedCount > 0)
            {
                criteria.SetFirstResult(startIndex).SetMaxResults(requestedCount);
            }

            if (!string.IsNullOrEmpty(orderPropertyName))
            {
                criteria.AddOrder(asc ? Order.Asc(orderPropertyName) : Order.Desc(orderPropertyName));
            }

            totalCount = GetCount();
            return criteria.List<T>();
        }

        public async Task<Tuple<IList<T>, int, int>> GetEntitiesAsync(int startIndex = 0, int requestedCount = 0, string filterColumn = null, string filterValue = null, string orderPropertyName = null, bool asc = true)
        {
            ICriteria criteria = Session.CreateCriteria<T>();

            int filteredCount;

            if (!string.IsNullOrEmpty(filterColumn))
            {
                criteria.Add(Restrictions.Like(filterColumn, $"%{filterValue}%"));

                filteredCount = criteria.ListAsync<T>().Result.Count;
            }
            else
            {
                filteredCount = await GetCountAsync();
            }

            if (requestedCount > 0)
            {
                criteria.SetFirstResult(startIndex).SetMaxResults(requestedCount);
            }

            if (!string.IsNullOrEmpty(orderPropertyName))
            {
                criteria.AddOrder(asc ? Order.Asc(orderPropertyName) : Order.Desc(orderPropertyName));
            }

            var entitiesTask = await criteria.ListAsync<T>();
            var countTask = await GetCountAsync();

            return new Tuple<IList<T>, int, int>(entitiesTask, countTask, filteredCount);
        }

        public void Delete(T entity)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                Session.Delete(entity);
                transaction.Commit();
            }
        }

        public async Task DeleteAsync(T entity)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                await Session.DeleteAsync(entity);
                transaction.Commit();
            }
        }

        public object Create(T entity)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                object o = Session.Save(entity);
                transaction.Commit();
                return o;
            }
        }

        public async Task<object> CreateAsync(T entity)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                object o = await Session.SaveAsync(entity);
                transaction.Commit();
                return o;
            }
        }

        public void Update(T entity)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                Session.Update(entity);
                transaction.Commit();
            }
        }

        public async Task UpdateAsync(T entity)
        {
            using (ITransaction transaction = Session.BeginTransaction())
            {
                await Session.UpdateAsync(entity);
                transaction.Commit();
            }
        }

        public T GetById(Guid id)
        {
            return Session.CreateCriteria<T>().Add(Restrictions.Eq(nameof(IEntity.Id), id)).UniqueResult<T>();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await Session.CreateCriteria<T>().Add(Restrictions.Eq(nameof(IEntity.Id), id)).UniqueResultAsync<T>();
        }

        public int GetCount()
        {
            return Session.CreateCriteria<T>().SetProjection(Projections.RowCount()).UniqueResult<int>();
        }

        public async Task<int> GetCountAsync()
        {
            return await Session.CreateCriteria<T>().SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
        }

        public IList<T> GetLatestEntities(string datePropertyName, int requestedCount, out int totalCount)
        {
            totalCount = GetCount();
            return Session.CreateCriteria<T>().AddOrder(Order.Desc(datePropertyName)).SetMaxResults(requestedCount).List<T>();
        }

        public async Task<Tuple<IList<T>, int>> GetLatestEntitiesAsync(string datePropertyName, int requestedCount)
        {
            var countTask = GetCountAsync();
            var entitiesTask = Session.CreateCriteria<T>().AddOrder(Order.Desc(datePropertyName)).SetMaxResults(requestedCount).ListAsync<T>();

            return new Tuple<IList<T>, int>(await entitiesTask, await countTask);
        }
    }
}