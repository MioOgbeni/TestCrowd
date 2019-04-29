using System;
using System.Collections.Generic;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Interface.Tests
{
    public interface ITestGroup:IEntity
    {
        string Name { get; set; }
        int SkillDificulty { get; set; }
        int TimeDificulty { get; set; }

        IList<TestCase> TestCases { get; set; }
        decimal RewardMultiplier { get; set; }

        Company Creator { get; set; }

        DateTime Created { get; set; }
        DateTime AvailableTo { get; set; }

        int Rating { get; set; }

        int CountRating();
    }
}