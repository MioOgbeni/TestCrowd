using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Interface.Repository.Tests
{
    public interface ITestsRepository: IRepositoryBase<Model.Tests.Tests>
    {
        bool IsTestTakened(TestCase testCase, Tester tester);

        TestsStatus GetTestStatus(TestCase testCase, Tester tester);

        IList<Model.Tests.Tests> GetAvailableEntities(out int totalCount, Tester tester, IList<TestsStatus> statuses, SoftwareType swType = null, TestCategory testCat = null, int startIndex = 0, int requestedCount = 0, string searchTerm = "");
        Task<Tuple<IList<Model.Tests.Tests>, int>> GetAvailableEntitiesAsync(Tester tester, IList<TestsStatus> statuses, SoftwareType swType = null, TestCategory testCat = null, int startIndex = 0, int requestedCount = 0, string searchTerm = "");

        Model.Tests.Tests GetByTestCaseForTester(Tester tester, TestCase testCase);
        Task<Model.Tests.Tests> GetByTestCaseForTesterAsync(Tester tester, TestCase testCase);

        IList<Model.Tests.Tests> GetTestsForCompany(Company company, out int totalCount, out int filteredCount, TestCase testCase = null, TestStatus status = null, int startIndex = 0, int requestedCount = 0, string orderPropertyName = null, bool asc = true);

        int GetCountByStatus(IList<TestsStatus> statuses, Tester tester);
        Task<int> GetCountByStatusAsync(IList<TestsStatus> statuses, Tester tester);
    }
}