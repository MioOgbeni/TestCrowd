using System.Collections.Generic;
using System.Linq;
using TestCrowd.DataAccess.Interface;
using TestCrowd.DataAccess.Model.Reviews;

namespace TestCrowd.DataAccess.Model
{
    public class Company : ApplicationUser, ICompany
    {
        public virtual string CompanyName { get; set; }

        public virtual IList<Review> Reviews { get; set; }
        public virtual int Rating { get; set; }

        public virtual void CountRating()
        {
            Rating = (int)(Reviews.Count == 0 ? 0 : Reviews.Average(x => x.Rating));
        }

        public virtual int Credits { get; set; }
        public virtual Address Address { get; set; }
    }
}