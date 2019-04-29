using System.Collections.Generic;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Tests;

namespace TestCrowd.DataAccess.Interface
{
    public interface ITester:IApplicationUser
    {
        string FirstName { get; set; }
        string LastName { get; set; }

        int Credits { get; set; }

        Address Address { get; set; }
    }
}