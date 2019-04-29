using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Interface.Repository
{
    public interface IApplicationUserRepository<T> : IRepositoryBase<T>, IUserStore<T, Guid>, IUserRoleStore<T, Guid> where T : ApplicationUser
    {
        T GetByUserName(string userName);
        Task<T> GetByUserNameAsync(string userName);

        bool IsUserExist(string userName);
        Task<bool> IsUserExistAsync(string userName);

        bool IsUserWithEmailExist(string userName, string email);
        Task<bool> IsUserWithEmailExistAcync(string userName, string email);

        IList<T> GetEntities(out int totalCount, out int filteredCount, int startIndex, int requestedCount, string filterColumn, string filterValue, string orderPropertyName, bool asc, string currentUserId);
        Task<Tuple<IList<T>, int, int>> GetEntitiesAsync(int startIndex, int requestedCount, string filterColumn, string filterValue, string orderPropertyName, bool asc, string currentUserId);

    }
}