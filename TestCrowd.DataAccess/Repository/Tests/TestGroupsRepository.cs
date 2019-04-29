using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate.Criterion;
using TestCrowd.DataAccess.Interface.Repository.Tests;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Repository.Tests
{
    public class TestGroupsRepository:RepositoryBase<TestGroups>, ITestGroupsRepository
    {
        public bool IsTestGroupTakened(TestGroup testGroup, Tester tester)
        {
            var test = Session.CreateCriteria<TestGroups>().Add(Restrictions.Eq(nameof(TestGroups.TestGroup), testGroup)).Add(Restrictions.Eq(nameof(TestGroups.Status), GroupStatus.Takened)).Add(Restrictions.Eq(nameof(TestGroups.Tester), tester)).UniqueResult<TestGroups>();
            return test != null;
        }

        public GroupStatus GetGroupStatus(TestGroup testGroup, Tester tester)
        {
            try
            {
                return Session.CreateCriteria<TestGroups>().Add(Restrictions.Eq(nameof(TestGroups.TestGroup), testGroup)).Add(Restrictions.Eq(nameof(TestGroups.Tester), tester)).UniqueResult<TestGroups>().Status;

            }
            catch (Exception e)
            {
                return GroupStatus.Null;
            }
        }

        public IList<TestGroups> GetAvailableEntities(out int totalCount, Tester tester, GroupStatus status, int startIndex = 0, int requestedCount = 0, string searchTerm = "")
        {
            TestGroups testGroupsAlias = null;

            var subquery = QueryOver.Of<TestGroup>()
                .Where(sq => sq.Id == testGroupsAlias.TestGroup.Id)
                .And(sq => sq.Name.IsInsensitiveLike(searchTerm, MatchMode.Anywhere))
                .Select(sq => sq.Id);

            var query = Session.QueryOver<TestGroups>(() => testGroupsAlias)
                .WithSubquery.WhereExists(subquery)
                .Where(() => testGroupsAlias.Tester == tester)
                .And(() => testGroupsAlias.Status == status);

            totalCount = query.List<TestGroups>().Count;

            if (requestedCount > 0)
            {
                query.Skip(startIndex).Take(requestedCount);
            }

            return query.List<TestGroups>();
        }

        public async Task<Tuple<IList<TestGroups>, int>> GetAvailableEntitiesAsync(Tester tester, GroupStatus status, int startIndex = 0, int requestedCount = 0, string searchTerm = "")
        {
            TestGroups testGroupsAlias = null;

            var subquery = QueryOver.Of<TestGroup>()
                .Where(sq => sq.Id == testGroupsAlias.TestGroup.Id)
                .And(sq => sq.Name.IsInsensitiveLike(searchTerm, MatchMode.Anywhere))
                .Select(sq => sq.Id);

            var query = Session.QueryOver<TestGroups>(() => testGroupsAlias)
                .WithSubquery.WhereExists(subquery)
                .Where(() => testGroupsAlias.Tester == tester)
                .And(() => testGroupsAlias.Status == status);

            var totalCount = query.List<TestGroups>().Count;

            if (requestedCount > 0)
            {
                query.Skip(startIndex).Take(requestedCount);
            }

            var entitiesTask = await query.ListAsync<TestGroups>();

            return new Tuple<IList<TestGroups>, int>(entitiesTask, totalCount);
        }

        public TestGroups GetByTestGroupForTester(Tester tester, TestGroup testGroup)
        {
            return Session.CreateCriteria<TestGroups>().Add(Restrictions.Eq(nameof(TestGroups.Tester), tester)).Add(Restrictions.Eq(nameof(TestGroups.TestGroup), testGroup)).UniqueResult<TestGroups>();
        }

        public async Task<TestGroups> GetByTestGroupForTesterAsync(Tester tester, TestGroup testGroup)
        {
            return await Session.CreateCriteria<TestGroups>().Add(Restrictions.Eq(nameof(TestGroups.Tester), tester)).Add(Restrictions.Eq(nameof(TestGroups.TestGroup), testGroup)).UniqueResultAsync<TestGroups>();
        }
    }
}