using System;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Interface.Reviews
{
    public interface IReview :IEntity
    {
        ApplicationUser Creator { get; set; }
        string Content { get; set; }
        int Rating { get; set; }
        DateTime Created { get; set; }
    }
}