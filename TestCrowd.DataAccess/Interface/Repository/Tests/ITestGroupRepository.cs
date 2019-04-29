using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Interface.Repository.Tests
{
    public interface ITestGroupRepository :IRepositoryBase<TestGroup>
    {
        IList<TestGroup> GetEntitiesForCompany(out int totalCount, out int filteredCount, int startIndex, int requestedCount, string filterColumn, string filterValue, string orderPropertyName, bool asc, Company currentUser);
        Task<Tuple<IList<TestGroup>, int, int>> GetEntitiesForCompanyAsync(int startIndex, int requestedCount, string filterColumn, string filterValue, string orderPropertyName, bool asc, Company currentUser);

        int GetCountForCompany(Company currentUser = null);
        Task<int> GetCountForCompanyAsync(Company currentUser = null);

        IList<TestGroup> GetAvailableEntities(out int totalCount, Tester tester, DateTime availableTo, int startIndex = 0, int requestedCount = 0, string searchTerm = null);
        Task<Tuple<IList<TestGroup>, int>> GetAvailableEntitiesAsync(Tester tester, DateTime availableTo, int startIndex = 0, int requestedCount = 0, string searchTerm = null);
    }
}