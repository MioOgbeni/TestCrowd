using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Criterion;
using TestCrowd.DataAccess.Interface;
using TestCrowd.DataAccess.Interface.Repository.Tests;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Repository.Tests
{
    public class TestCaseRepository:RepositoryBase<TestCase>, ITestCaseRepository
    {
        public IList<TestCase> GetEntitiesForCompany(out int totalCount, out int filteredCount, int startIndex = 0, int requestedCount = 0, string filterColumn = null, string filterValue = null, string orderPropertyName = null,
            bool asc = true, Company currentUser = null)
        {
            ICriteria criteria = Session.CreateCriteria<TestCase>();

            if (currentUser != null)
            {
                criteria.Add(Restrictions.Eq(nameof(TestCase.Creator), currentUser));
            }

            if (!string.IsNullOrEmpty(filterColumn))
            {
                criteria.Add(Restrictions.Like(filterColumn, $"%{filterValue}%"));
            }

            filteredCount = criteria.List<TestCase>().Count;

            if (requestedCount > 0)
            {
                criteria.SetFirstResult(startIndex).SetMaxResults(requestedCount);
            }

            if (!string.IsNullOrEmpty(orderPropertyName))
            {
                criteria.AddOrder(asc ? Order.Asc(orderPropertyName) : Order.Desc(orderPropertyName));
            }

            totalCount = GetCount();
            return criteria.List<TestCase>();
        }

        public async Task<Tuple<IList<TestCase>, int, int>> GetEntitiesForCompanyAsync(int startIndex = 0, int requestedCount = 0, string filterColumn = null, string filterValue = null, string orderPropertyName = null, bool asc = true, Company currentUser = null)
        {
            ICriteria criteria = Session.CreateCriteria<TestCase>();

            if (currentUser != null)
            {
                criteria.Add(Restrictions.Eq(nameof(TestCase.Creator), currentUser));
            }

            int filteredCount;

            if (!string.IsNullOrEmpty(filterColumn))
            {
                criteria.Add(Restrictions.Like(filterColumn, $"%{filterValue}%"));

                filteredCount = criteria.ListAsync<TestCase>().Result.Count;
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

            var entitiesTask = await criteria.ListAsync<TestCase>();
            var countTask = await GetCountAsync();

            return new Tuple<IList<TestCase>, int, int>(entitiesTask, countTask, filteredCount);
        }

        public IList<TestCase> GetAllForCompany(Company currentUser)
        {
            return Session.CreateCriteria<TestCase>().Add(Restrictions.Eq(nameof(TestCase.Creator), currentUser)).List<TestCase>();
        }

        public async Task<IList<TestCase>> GetAllForCompanyAsync(Company currentUser)
        {
            return await Session.CreateCriteria<TestCase>().Add(Restrictions.Eq(nameof(TestCase.Creator), currentUser)).ListAsync<TestCase>();
        }

        public int GetCountForCompany(Company currentUser = null)
        {
            if (currentUser != null)
            {
                return Session.CreateCriteria<TestCase>().Add(Restrictions.Eq(nameof(TestCase.Creator), currentUser)).SetProjection(Projections.RowCount()).UniqueResult<int>();
            }

            return Session.CreateCriteria<TestCase>().SetProjection(Projections.RowCount()).UniqueResult<int>();
        }

        public async Task<int> GetCountForCompanyAsync(Company currentUser = null)
        {
            if (currentUser != null)
            {
                return await Session.CreateCriteria<TestCase>().Add(Restrictions.Eq(nameof(TestCase.Creator), currentUser)).SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
            }

            return await Session.CreateCriteria<TestCase>().SetProjection(Projections.RowCount()).UniqueResultAsync<int>();
        }

        public IList<TestCase> GetFilteredForCompanyNotInGroup(Company currentUser, string filterColumn = null, string filterValue = null)
        {
            ICriteria criteria = Session.CreateCriteria<TestCase>();

            if (currentUser != null)
            {
                criteria.Add(Restrictions.Eq(nameof(TestCase.Creator), currentUser));
            }

            if (!string.IsNullOrEmpty(filterColumn))
            {
                criteria.Add(Restrictions.Like(filterColumn, $"%{filterValue}%"));
            }

            criteria.Add(Restrictions.Eq(nameof(TestCase.IsInGroup), false));

            return criteria.List<TestCase>();
        }

        public async Task<IList<TestCase>> GetFilteredForCompanyNotInGroupAsync(Company currentUser, string filterColumn = null, string filterValue = null)
        {
            ICriteria criteria = Session.CreateCriteria<TestCase>();

            if (currentUser != null)
            {
                criteria.Add(Restrictions.Eq(nameof(TestCase.Creator), currentUser));
            }

            if (!string.IsNullOrEmpty(filterColumn))
            {
                criteria.Add(Restrictions.Like(filterColumn, $"%{filterValue}%"));
            }

            criteria.Add(Restrictions.Eq(nameof(TestCase.IsInGroup), false));

            return await criteria.ListAsync<TestCase>();
        }

        public TestCase GetByIdForCompanyNotInGroup(Company currentUser, Guid id)
        {
            ICriteria criteria = Session.CreateCriteria<TestCase>();

            if (currentUser != null)
            {
                criteria.Add(Restrictions.Eq(nameof(TestCase.Creator), currentUser));
            }

            criteria.Add(Restrictions.Eq(nameof(TestCase.Id), id));

            criteria.Add(Restrictions.Eq(nameof(TestCase.IsInGroup), false));

            return criteria.UniqueResult<TestCase>();
        }

        public async Task<TestCase> GetByIdForCompanyNotInGroupAsync(Company currentUser, Guid id)
        {
            ICriteria criteria = Session.CreateCriteria<TestCase>();

            if (currentUser != null)
            {
                criteria.Add(Restrictions.Eq(nameof(TestCase.Creator), currentUser));
            }

            criteria.Add(Restrictions.Eq(nameof(TestCase.Id), id));

            criteria.Add(Restrictions.Eq(nameof(TestCase.IsInGroup), false));

            return await criteria.UniqueResultAsync<TestCase>();
        }

        public IList<TestCase> GetAvailableEntities(out int totalCount, DateTime availableTo, Tester tester, SoftwareType swType = null, TestCategory testCat = null, int startIndex = 0, int requestedCount = 0, string searchTerm = "")
        {
            TestCase testCaseAlias = null;

            var query = Session.QueryOver<TestCase>(() => testCaseAlias)
                .WithSubquery.WhereNotExists(
                    QueryOver.Of<Model.Tests.Tests>()
                    .Where(sq => sq.Test.Id == testCaseAlias.Id)
                    .And(sq => sq.Tester == tester)
                    .Select(sq => sq.Id))
                .Where(() => testCaseAlias.Name.IsInsensitiveLike(searchTerm, MatchMode.Anywhere))
                .And(() => testCaseAlias.AvailableTo >= availableTo);

            if (swType != null)
            {
                query.And(() => testCaseAlias.SoftwareType == swType);
            }

            if (testCat != null)
            {
                query.And(() => testCaseAlias.Category == testCat);
            }

            totalCount = query.List<TestCase>().Count;

            if (requestedCount > 0)
            {
                query.Skip(startIndex).Take(requestedCount);
            }

            return query.List<TestCase>();
        }

        public async Task<Tuple<IList<TestCase>, int>> GetAvailableEntitiesAsync(DateTime availableTo, Tester tester, SoftwareType swType = null, TestCategory testCat = null, int startIndex = 0, int requestedCount = 0, string searchTerm = "")
        {
            TestCase testCaseAlias = null;

            var query = Session.QueryOver<TestCase>(() => testCaseAlias)
                .WithSubquery.WhereNotExists(
                    QueryOver.Of<Model.Tests.Tests>()
                        .Where(sq => sq.Test.Id == testCaseAlias.Id)
                        .And(sq => sq.Tester == tester)
                        .Select(sq => sq.Id))
                .Where(() => testCaseAlias.Name.IsInsensitiveLike(searchTerm, MatchMode.Anywhere))
                .And(() => testCaseAlias.AvailableTo >= availableTo);

            if (swType != null)
            {
                query.And(() => testCaseAlias.SoftwareType == swType);
            }

            if (testCat != null)
            {
                query.And(() => testCaseAlias.Category == testCat);
            }

            var totalCount = query.List<TestCase>().Count;

            if (requestedCount > 0)
            {
                query.Skip(startIndex).Take(requestedCount);
            }

            var entitiesTask = await query.ListAsync<TestCase>();

            return new Tuple<IList<TestCase>, int>(entitiesTask, totalCount);
        }

    }
}