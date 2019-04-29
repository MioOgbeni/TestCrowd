using System;
using System.Collections.Generic;
using NHibernate.Criterion;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Interface.Repository.Tests
{
    public interface ISoftwareTypeRepository :IRepositoryBase<SoftwareType>
    {
        bool IsSwTypeExist(Guid id);

        IList<SoftwareType> GetAllValid();
    }
}