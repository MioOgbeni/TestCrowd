using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Interface.Repository.Tests
{
    public interface ITestCaseRepository : IRepositoryBase<TestCase>
    {
        IList<TestCase> GetEntitiesForCompany(out int totalCount, out int filteredCount, int startIndex, int requestedCount, string filterColumn, string filterValue, string orderPropertyName, bool asc, Company currentUser);
        Task<Tuple<IList<TestCase>, int, int>> GetEntitiesForCompanyAsync(int startIndex, int requestedCount, string filterColumn, string filterValue, string orderPropertyName, bool asc, Company currentUser);

        IList<TestCase> GetAllForCompany(Company currentUser);
        Task<IList<TestCase>> GetAllForCompanyAsync(Company currentUser);

        int GetCountForCompany(Company currentUser = null);
        Task<int> GetCountForCompanyAsync(Company currentUser = null);

        IList<TestCase> GetFilteredForCompanyNotInGroup(Company currentUser, string filterColumn = null, string filterValue = null);
        Task<IList<TestCase>> GetFilteredForCompanyNotInGroupAsync(Company currentUser, string filterColumn = null, string filterValue = null);

        TestCase GetByIdForCompanyNotInGroup(Company currentUser, Guid id);
        Task<TestCase> GetByIdForCompanyNotInGroupAsync(Company currentUser, Guid id);

        IList<TestCase> GetAvailableEntities(out int totalCount, DateTime availableTo, Tester tester, SoftwareType swType = null, TestCategory testCat = null, int startIndex = 0, int requestedCount = 0, string searchTerm = "");
        Task<Tuple<IList<TestCase>, int>> GetAvailableEntitiesAsync(DateTime availableTo, Tester tester, SoftwareType swType = null, TestCategory testCat = null, int startIndex = 0, int requestedCount = 0, string searchTerm = "");
    }
}