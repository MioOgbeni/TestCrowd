using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using TestCrowd.DataAccess.Interface;
using TestCrowd.DataAccess.Interface.Repository.Tests;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Repository.Tests
{
    public class TestsRepository:RepositoryBase<Model.Tests.Tests>,ITestsRepository
    {
        public bool IsTestTakened(TestCase testCase, Tester tester)
        {
            var test = Session.CreateCriteria<Model.Tests.Tests>().Add(Restrictions.Eq(nameof(Model.Tests.Tests.Test), testCase)).Add(Restrictions.Eq(nameof(Model.Tests.Tests.Tester), tester)).Add(Restrictions.Disjunction().Add(Restrictions.Eq(nameof(Model.Tests.Tests.Status), TestsStatus.Takened)).Add(Restrictions.Eq(nameof(Model.Tests.Tests.Status), TestsStatus.Finished)).Add(Restrictions.Eq(nameof(Model.Tests.Tests.Status), TestsStatus.Reviewed))).UniqueResult<Model.Tests.Tests>();
            return test != null;
        }

        public TestsStatus GetTestStatus(TestCase testCase, Tester tester)
        {
            try
            {
                return Session.CreateCriteria<Model.Tests.Tests>().Add(Restrictions.Eq(nameof(Model.Tests.Tests.Test), testCase)).Add(Restrictions.Eq(nameof(Model.Tests.Tests.Tester), tester)).UniqueResult<Model.Tests.Tests>().Status;
            }
            catch (Exception e)
            {
                return TestsStatus.Null;
            }           
        }

        public IList<Model.Tests.Tests> GetAvailableEntities(out int totalCount, Tester tester, IList<TestsStatus> statuses, SoftwareType swType = null, TestCategory testCat = null, int startIndex = 0, int requestedCount = 0, string searchTerm = "")
        {
            Model.Tests.Tests testsAlias = null;

            var restriction = Restrictions.Disjunction();

            foreach (var status in statuses)
            {
                restriction.Add(Restrictions.Where<Model.Tests.Tests>(x => x.Status == status));
            }

            var subquery = QueryOver.Of<TestCase>()
                .Where(sq => sq.Id == testsAlias.Test.Id)
                .And(sq => sq.Name.IsInsensitiveLike(searchTerm, MatchMode.Anywhere));

            if (swType != null)
            {
                subquery.And(sq => sq.SoftwareType == swType);
            }

            if (testCat != null)
            {
                subquery.And(sq => sq.Category == testCat);
            }

            subquery.Select(sq => sq.Id);

            var query = Session.QueryOver<Model.Tests.Tests>(() => testsAlias)
                .WithSubquery.WhereExists(subquery)
                .Where(restriction)
                .And(() => testsAlias.Tester == tester);

            totalCount = query.List<Model.Tests.Tests>().Count;

            if (requestedCount > 0)
            {
                query.Skip(startIndex).Take(requestedCount);
            }

            return query.List<Model.Tests.Tests>();
        }

        public async Task<Tuple<IList<Model.Tests.Tests>, int>> GetAvailableEntitiesAsync(Tester tester, IList<TestsStatus> statuses, SoftwareType swType = null, TestCategory testCat = null, int startIndex = 0, int requestedCount = 0, string searchTerm = "")
        {
            Model.Tests.Tests testsAlias = null;

            var restriction = Restrictions.Disjunction();

            foreach (var status in statuses)
            {
                restriction.Add(Restrictions.Where<Model.Tests.Tests>(x => x.Status == status));
            }

            var subquery = QueryOver.Of<TestCase>()
                .Where(sq => sq.Id == testsAlias.Test.Id)
                .And(sq => sq.Name.IsInsensitiveLike(searchTerm, MatchMode.Anywhere));

            if (swType != null)
            {
                subquery.And(sq => sq.SoftwareType == swType);
            }

            if (testCat != null)
            {
                subquery.And(sq => sq.Category == testCat);
            }

            subquery.Select(sq => sq.Id);

            var query = Session.QueryOver<Model.Tests.Tests>(() => testsAlias)
                .WithSubquery.WhereExists(subquery)
                .Where(restriction)
                .And(() => testsAlias.Tester == tester);

            var totalCount = query.List<Model.Tests.Tests>().Count;

            if (requestedCount > 0)
            {
                query.Skip(startIndex).Take(requestedCount);
            }

            var entitiesTask = await query.ListAsync<Model.Tests.Tests>();

            return new Tuple<IList<Model.Tests.Tests>, int>(entitiesTask, totalCount);
        }

        public Model.Tests.Tests GetByTestCaseForTester(Tester tester, TestCase testCase)
        {
            return Session.CreateCriteria<Model.Tests.Tests>().Add(Restrictions.Eq(nameof(Model.Tests.Tests.Tester), tester)).Add(Restrictions.Eq(nameof(Model.Tests.Tests.Test), testCase)).UniqueResult<Model.Tests.Tests>();
        }

        public async Task<Model.Tests.Tests> GetByTestCaseForTesterAsync(Tester tester, TestCase testCase)
        {
            return await Session.CreateCriteria<Model.Tests.Tests>().Add(Restrictions.Eq(nameof(Model.Tests.Tests.Tester), tester)).Add(Restrictions.Eq(nameof(Model.Tests.Tests.Test), testCase)).UniqueResultAsync<Model.Tests.Tests>();
        }

        public Model.Tests.Tests GetByTestCaseForTesterByStatus(Tester tester, TestCase testCase, TestsStatus status)
        {
            return Session.CreateCriteria<Model.Tests.Tests>().Add(Restrictions.Eq(nameof(Model.Tests.Tests.Tester), tester)).Add(Restrictions.Eq(nameof(Model.Tests.Tests.Test), testCase)).Add(Restrictions.Eq(nameof(Model.Tests.Tests.Status), status)).UniqueResult<Model.Tests.Tests>();
        }

        public async Task<Model.Tests.Tests> GetByTestCaseForTesterByStatusAsync(Tester tester, TestCase testCase, TestsStatus status)
        {
            return await Session.CreateCriteria<Model.Tests.Tests>().Add(Restrictions.Eq(nameof(Model.Tests.Tests.Tester), tester)).Add(Restrictions.Eq(nameof(Model.Tests.Tests.Test), testCase)).Add(Restrictions.Eq(nameof(Model.Tests.Tests.Status), status)).UniqueResultAsync<Model.Tests.Tests>();
        }

        public IList<Model.Tests.Tests> GetTestsForCompany(Company company, out int totalCount, out int filteredCount, TestCase testCase = null, TestStatus status = null, int startIndex = 0, int requestedCount = 0, string orderPropertyName = null, bool asc = true)
        {
            Model.Tests.Tests testsAlias = null;

            var query = Session.QueryOver<Model.Tests.Tests>(() => testsAlias)
                .WithSubquery.WhereExists(
                    QueryOver.Of<TestCase>()
                        .Where(sq => sq.Id == testsAlias.Test.Id)
                        .And(sq => sq.Creator == company)
                        .Select(sq => sq.Id))
                .Where(Restrictions.Disjunction()
                    .Add((() => testsAlias.Status == TestsStatus.Finished))
                    .Add((() => testsAlias.Status == TestsStatus.Rejected))
                    .Add(() => testsAlias.Status == TestsStatus.Reviewed));

            if (testCase != null)
            {
                query.And(() => testsAlias.Test == testCase);
            }

            if (status != null)
            {
                query.And(() => testsAlias.TestStatus == status);
            }

            filteredCount = query.List<Model.Tests.Tests>().Count;

            if (requestedCount > 0)
            {
                query.Skip(startIndex).Take(requestedCount);
            }

            totalCount = query.List<Model.Tests.Tests>().Count;

            return query.List<Model.Tests.Tests>();
        }

        public int GetCountByStatus(IList<TestsStatus> statuses, Tester tester)
        {
            ICriteria criteria = Session.CreateCriteria<Model.Tests.Tests>().Add(Restrictions.Eq(nameof(Model.Tests.Tests.Tester), tester));

            var restriction = Restrictions.Disjunction();
            foreach (var status in statuses)
            {
                restriction.Add(Restrictions.Eq(nameof(Model.Tests.Tests.Status), status));
            }

            criteria.Add(restriction);

            return criteria.SetProjection(Projections.RowCount()).UniqueResult<int>();
        }

        public async Task<int> GetCountByStatusAsync(IList<TestsStatus> statuses, Tester tester)
        {
            ICriteria criteria = Session.CreateCriteria<Model.Tests.Tests>().Add(Restrictions.Eq(nameof(Model.Tests.Tests.Tester), tester));

            var restriction = Restrictions.Disjunction();
            foreach (var status in statuses)
            {
                restriction.Add(Restrictions.Eq(nameof(Model.Tests.Tests.Status), status));
            }

            criteria.Add(restriction);

            return await criteria.SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
        }
    }
}