using System;
using TestCrowd.DataAccess.Interface;
using TestCrowd.DataAccess.Interface.Tests;

namespace TestCrowd.DataAccess.Model.Tests
{
    public enum GroupStatus { Takened, Finished, Null }

    public class TestGroups :ITestGroups
    {
        public virtual Guid Id { get; set; }

        public virtual TestGroup TestGroup { get; set; }
        public virtual Tester Tester { get; set; }
        public virtual GroupStatus Status { get; set; }
        public virtual DateTime Takened { get; set; }
        public virtual DateTime? Finished { get; set; }
    }
}