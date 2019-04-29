using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using TestCrowd.DataAccess.Interface;
using TestCrowd.DataAccess.Interface.Repository;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Repository
{
    public class ApplicationUserRepository<T> : RepositoryBase<T>, IApplicationUserRepository<T> where T : ApplicationUser
    {
        public void Dispose()
        {
        }

        public Task CreateAsync(T user)
        {
            throw new NotImplementedException();
        }

        public Task<T> FindByIdAsync(Guid userId)
        {
            return GetByIdAsync(userId);
        }

        public Task<T> FindByNameAsync(string userName)
        {
            return GetByUserNameAsync(userName);
        }

        public Task AddToRoleAsync(T user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(T user, string roleName)
        {
            throw new NotImplementedException();
        }

        public Task<IList<string>> GetRolesAsync(T user)
        {
            IList<string> roles = new List<string>(1);
            roles.Add(user.Role.Name);
            return Task.FromResult(roles);
        }

        public Task<bool> IsInRoleAsync(T user, string roleName)
        {
            return Task.FromResult(user.Role.Name == roleName);
        }

        public T GetByUserName(string userName)
        {
            return Session.CreateCriteria<T>().Add(Restrictions.Eq(nameof(ApplicationUser.UserName), userName)).UniqueResult<T>();
        }

        public async Task<T> GetByUserNameAsync(string userName)
        {
            return await Session.CreateCriteria<T>().Add(Restrictions.Eq(nameof(ApplicationUser.UserName), userName)).UniqueResultAsync<T>();
        }

        public bool IsUserExist(string userName)
        {
            var user = Session.CreateCriteria<T>().Add(Restrictions.Eq(nameof(ApplicationUser.UserName), userName)).UniqueResult<T>();
            return user != null;
        }

        public async Task<bool> IsUserExistAsync(string userName)
        {
            var user = await Session.CreateCriteria<T>().Add(Restrictions.Eq(nameof(ApplicationUser.UserName), userName)).UniqueResultAsync<T>();
            return user != null;
        }

        public bool IsUserWithEmailExist(string userName, string email)
        {
            var user = Session.CreateCriteria<T>().Add(Restrictions.Eq(nameof(ApplicationUser.UserName), userName)).Add(Restrictions.Eq(nameof(ApplicationUser.Email), email)).UniqueResult<T>();
            return user != null;
        }

        public async Task<bool> IsUserWithEmailExistAcync(string userName, string email)
        {
            var user = await Session.CreateCriteria<T>().Add(Restrictions.Eq(nameof(ApplicationUser.UserName), userName)).Add(Restrictions.Eq(nameof(ApplicationUser.Email), email)).UniqueResultAsync<T>();
            return user != null;
        }

        public IList<T> GetEntities(out int totalCount, out int filteredCount, int startIndex = 0, int requestedCount = 0, string filterColumn = null, string filterValue = null, string orderPropertyName = null,
            bool asc = true, string currentUserId = null)
        {
            ICriteria criteria = Session.CreateCriteria<T>();

            if (!string.IsNullOrEmpty(currentUserId))
            {
                criteria.Add(Restrictions.Not(Restrictions.Eq(nameof(IEntity.Id), Guid.Parse(currentUserId))));
            }

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

        public async Task<Tuple<IList<T>, int, int>> GetEntitiesAsync(int startIndex = 0, int requestedCount = 0, string filterColumn = null, string filterValue = null, string orderPropertyName = null, bool asc = true, string currentUserId = null)
        {
            ICriteria criteria = Session.CreateCriteria<T>();

            if (!string.IsNullOrEmpty(currentUserId))
            {
                criteria.Add(Restrictions.Not(Restrictions.Eq(nameof(IEntity.Id), Guid.Parse(currentUserId))));
            }

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
    }
}