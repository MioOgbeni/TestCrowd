using System;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Interface.Tests
{
    public interface ITestCategory :IEntity
    {
        string Name { get; set; }
        string Description { get; set; }
        bool Valid { get; set; }

        DateTime Created { get; set; }
        DateTime LastChange { get; set; }
        Admin ChangedBy { get; set; }
    }
}