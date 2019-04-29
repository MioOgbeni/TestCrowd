using System;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Interface.Tests
{
    public interface ISoftwareType :IEntity
    {
        string Name { get; set; }
        string Description { get; set; }
        bool Valid { get; set; }

        DateTime Created { get; set; }
        DateTime LastChange { get; set; }
        Admin ChangedBy { get; set; }
    }
}