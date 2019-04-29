using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using TestCrowd.DataAccess.Interface.Repository.Tests;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Repository.Tests
{
    public class TestGroupRepository:RepositoryBase<TestGroup>, ITestGroupRepository
    {
        public IList<TestGroup> GetEntitiesForCompany(out int totalCount, out int filteredCount, int startIndex = 0, int requestedCount = 0, string filterColumn = null, string filterValue = null, string orderPropertyName = null,
            bool asc = true, Company currentUser = null)
        {
            ICriteria criteria = Session.CreateCriteria<TestGroup>();

            if (currentUser != null)
            {
                criteria.Add(Restrictions.Eq(nameof(TestGroup.Creator), currentUser));
            }

            if (!string.IsNullOrEmpty(filterColumn))
            {
                criteria.Add(Restrictions.Like(filterColumn, $"%{filterValue}%"));
            }

            filteredCount = criteria.List<TestGroup>().Count;

            if (requestedCount > 0)
            {
                criteria.SetFirstResult(startIndex).SetMaxResults(requestedCount);
            }

            if (!string.IsNullOrEmpty(orderPropertyName))
            {
                criteria.AddOrder(asc ? Order.Asc(orderPropertyName) : Order.Desc(orderPropertyName));
            }

            totalCount = GetCount();
            return criteria.List<TestGroup>();
        }

        public async Task<Tuple<IList<TestGroup>, int, int>> GetEntitiesForCompanyAsync(int startIndex = 0, int requestedCount = 0, string filterColumn = null, string filterValue = null, string orderPropertyName = null, bool asc = true, Company currentUser = null)
        {
            ICriteria criteria = Session.CreateCriteria<TestGroup>();

            if (currentUser != null)
            {
                criteria.Add(Restrictions.Eq(nameof(TestGroup.Creator), currentUser));
            }

            int filteredCount;

            if (!string.IsNullOrEmpty(filterColumn))
            {
                criteria.Add(Restrictions.Like(filterColumn, $"%{filterValue}%"));

                filteredCount = criteria.ListAsync<TestGroup>().Result.Count;
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

            var entitiesTask = await criteria.ListAsync<TestGroup>();
            var countTask = await GetCountAsync();

            return new Tuple<IList<TestGroup>, int, int>(entitiesTask, countTask, filteredCount);
        }

        public int GetCountForCompany(Company currentUser = null)
        {
            if (currentUser != null)
            {
                return Session.CreateCriteria<TestGroup>().Add(Restrictions.Eq(nameof(TestGroup.Creator), currentUser)).SetProjection(Projections.RowCount()).UniqueResult<int>();
            }

            return Session.CreateCriteria<TestGroup>().SetProjection(Projections.RowCount()).UniqueResult<int>();
        }

        public async Task<int> GetCountForCompanyAsync(Company currentUser = null)
        {
            if (currentUser != null)
            {
                return await Session.CreateCriteria<TestGroup>().Add(Restrictions.Eq(nameof(TestGroup.Creator), currentUser)).SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
            }

            return await Session.CreateCriteria<TestGroup>().SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
        }

        public IList<TestGroup> GetAvailableEntities(out int totalCount, Tester tester, DateTime availableTo, int startIndex = 0, int requestedCount = 0, string searchTerm = null)
        {
            TestGroup testGroupAlias = null;

            var query = Session.QueryOver<TestGroup>(() => testGroupAlias)
                .WithSubquery.WhereNotExists(
                    QueryOver.Of<TestGroups>()
                        .Where(sq => sq.TestGroup.Id == testGroupAlias.Id)
                        .And(sq => sq.Tester == tester)
                        .Select(sq => sq.Id))
                .Where(() => testGroupAlias.Name.IsInsensitiveLike(searchTerm, MatchMode.Anywhere))
                .And(() => testGroupAlias.AvailableTo >= availableTo);

            totalCount = query.List<TestGroup>().Count;

            if (requestedCount > 0)
            {
                query.Skip(startIndex).Take(requestedCount);
            }

            return query.List<TestGroup>();
        }

        public async Task<Tuple<IList<TestGroup>, int>> GetAvailableEntitiesAsync(Tester tester, DateTime availableTo, int startIndex = 0, int requestedCount = 0, string searchTerm = null)
        {
            TestGroup testGroupAlias = null;

            var query = Session.QueryOver<TestGroup>(() => testGroupAlias)
                .WithSubquery.WhereNotExists(
                    QueryOver.Of<TestGroups>()
                        .Where(sq => sq.TestGroup.Id == testGroupAlias.Id)
                        .And(sq => sq.Tester == tester)
                        .Select(sq => sq.Id))
                .Where(() => testGroupAlias.Name.IsInsensitiveLike(searchTerm, MatchMode.Anywhere))
                .And(() => testGroupAlias.AvailableTo >= availableTo);

            var totalCount = query.List<TestGroup>().Count;

            if (requestedCount > 0)
            {
                query.Skip(startIndex).Take(requestedCount);
            }

            var entitiesTask = await query.ListAsync<TestGroup>();

            return new Tuple<IList<TestGroup>, int>(entitiesTask, totalCount);
        }
    }
}