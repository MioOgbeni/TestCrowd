using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Interface.Repository.Tests
{
    public interface ITestStatusRepository :IRepositoryBase<TestStatus>
    {
         TestStatus GetByName(string name);
         Task<TestStatus> GetByNameAsync(string name);
    }
}
