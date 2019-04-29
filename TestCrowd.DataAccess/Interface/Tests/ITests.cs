using System;
using System.Collections.Generic;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Evidences;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Interface.Tests
{
    public interface ITests :IEntity
    {
        TestCase Test { get; set; }
        Tester Tester { get; set; }
        IList<Evidence> Evidences { get; set; }
        TestsStatus Status { get; set; }
        TestStatus TestStatus { get; set; }
        DateTime Takened { get; set; }
        DateTime? Finished { get; set; }
        DateTime? Rejected { get; set; }
    }
}