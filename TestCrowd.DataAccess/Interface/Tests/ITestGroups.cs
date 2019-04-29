using System;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Interface.Tests
{
    public interface ITestGroups:IEntity
    {
        TestGroup TestGroup { get; set; }
        Tester Tester { get; set; }
        GroupStatus Status { get; set; }
        DateTime Takened { get; set; }
        DateTime? Finished { get; set; }
    }
}