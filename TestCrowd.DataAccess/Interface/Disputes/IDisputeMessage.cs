using System;
using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Interface.Disputes
{
    public interface IDisputeMessage :IEntity
    {
        ApplicationUser User { get; set; }
        DateTime Created { get; set; }
        string Message { get; set; }
    }
}