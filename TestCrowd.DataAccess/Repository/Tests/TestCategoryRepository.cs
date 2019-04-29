using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate.Criterion;
using TestCrowd.DataAccess.Interface.Repository.Tests;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Repository.Tests
{
    public class TestCategoryRepository: RepositoryBase<TestCategory>, ITestCategoryRepository
    {
        public bool IsTestCatExist(Guid id)
        {
            var testCat = Session.CreateCriteria<TestCategory>().Add(Restrictions.Eq(nameof(TestCategory.Id), id)).UniqueResult<TestCategory>();
            return testCat != null;
        }

        public IList<TestCategory> GetAllValid()
        {
            return Session.CreateCriteria<TestCategory>().Add(Restrictions.Eq(nameof(TestCategory.Valid), true)).List<TestCategory>();
        }

        public async Task<IList<TestCategory>> GetAllValidAsync()
        {
            return await Session.CreateCriteria<TestCategory>().Add(Restrictions.Eq(nameof(TestCategory.Valid), true)).ListAsync<TestCategory>();
        }
    }
}