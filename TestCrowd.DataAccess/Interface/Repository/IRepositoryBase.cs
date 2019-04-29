using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestCrowd.DataAccess.Interface.Repository
{
    public interface IRepositoryBase<T> where T: class, IEntity
    {
        IList<T> GetAll();
        Task<IList<T>> GetAllAsync();

        IList<T> GetEntities(out int totalCount, out int filteredCount, int startIndex, int requestedCount, string filterColumn, string filterValue, string orderPropertyName, bool asc);
        Task<Tuple<IList<T>, int, int>> GetEntitiesAsync(int startIndex, int requestedCount, string filterColumn, string filterValue, string orderPropertyName, bool asc);

        void Delete(T entity);
        Task DeleteAsync(T entity);

        object Create(T entity);
        Task<object> CreateAsync(T entity);

        void Update(T entity);
        Task UpdateAsync(T entity);

        T GetById(Guid id);
        Task<T> GetByIdAsync(Guid id);

        int GetCount();
        Task<int> GetCountAsync();

        IList<T> GetLatestEntities(string datePropertyName, int requestedCount, out int totalCount);
        Task<Tuple<IList<T>, int>> GetLatestEntitiesAsync(string datePropertyName, int requestedCount);
    }
}