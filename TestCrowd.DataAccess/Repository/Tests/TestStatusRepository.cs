using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate.Criterion;
using TestCrowd.DataAccess.Interface.Repository.Tests;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Repository.Tests
{
    public class TestStatusRepository : RepositoryBase<TestStatus>,ITestStatusRepository
    {
        public TestStatus GetByName(string name)
        {
            return Session.CreateCriteria<TestStatus>().Add(Restrictions.Eq(nameof(TestStatus.Status), name)).UniqueResult<TestStatus>();
        }

        public async Task<TestStatus> GetByNameAsync(string name)
        {
            return await Session.CreateCriteria<TestStatus>().Add(Restrictions.Eq(nameof(TestStatus.Status), name)).UniqueResultAsync<TestStatus>();
        }
    }
}
