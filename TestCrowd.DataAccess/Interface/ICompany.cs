using System.Collections.Generic;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Reviews;

namespace TestCrowd.DataAccess.Interface
{
    public interface ICompany:IApplicationUser
    {
        string CompanyName { get; set; }

        IList<Review> Reviews { get; set; }
        int Rating { get; set; }
        void CountRating();

        int Credits { get; set; }

        Address Address { get; set; }
    }
}