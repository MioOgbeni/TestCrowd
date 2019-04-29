using System;
using TestCrowd.DataAccess.Interface.Tests;

namespace TestCrowd.DataAccess.Model.Tests
{
    public class TestStatus :ITestStatus
    {
        public virtual Guid Id { get; set; }
        public virtual string Status { get; set; }
    }
}