using System;
using System.Collections.Generic;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Evidences;
using TestCrowd.DataAccess.Model.Reviews;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Interface.Tests
{
    public interface ITestCase :IEntity
    {
        TestCategory Category { get; set; }
        SoftwareType SoftwareType { get; set; }

        string Name { get; set; }
        int SkillDificulty { get; set; }
        int TimeDificulty { get; set; }

        string Description { get; set; }

        int Reward { get; set; }

        Company Creator { get; set; }

        IList<Evidence> Evidences { get; set; }

        DateTime Created { get; set; }
        DateTime AvailableTo { get; set; }

        IList<Review> Reviews { get; set; }
        int Rating { get; set; }

        bool IsInGroup { get; set; }

        void CountRating();
    }
}