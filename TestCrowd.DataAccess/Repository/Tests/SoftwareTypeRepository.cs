using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NHibernate.Criterion;
using TestCrowd.DataAccess.Interface.Repository.Tests;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Repository.Tests
{
    public class SoftwareTypeRepository:RepositoryBase<SoftwareType>, ISoftwareTypeRepository
    {
        public bool IsSwTypeExist(Guid id)
        {
            var swType = Session.CreateCriteria<SoftwareType>().Add(Restrictions.Eq(nameof(SoftwareType.Id), id)).UniqueResult<SoftwareType>();
            return swType != null;
        }

        public IList<SoftwareType> GetAllValid()
        {
            return Session.CreateCriteria<SoftwareType>().Add(Restrictions.Eq(nameof(TestCategory.Valid), true)).List<SoftwareType>();
        }

        public async Task<IList<SoftwareType>> GetAllValidAsync()
        {
            return await Session.CreateCriteria<SoftwareType>().Add(Restrictions.Eq(nameof(TestCategory.Valid), true)).ListAsync<SoftwareType>();
        }
    }
}