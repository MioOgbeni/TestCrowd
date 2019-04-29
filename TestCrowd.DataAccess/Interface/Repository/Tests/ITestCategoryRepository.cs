using System;
using System.Collections.Generic;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Interface.Repository.Tests
{
    public interface ITestCategoryRepository : IRepositoryBase<TestCategory>
    {
        bool IsTestCatExist(Guid id);

        IList<TestCategory> GetAllValid();
    }
}