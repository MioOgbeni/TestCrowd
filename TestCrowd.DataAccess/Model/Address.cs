using System;
using TestCrowd.DataAccess.Interface;

namespace TestCrowd.DataAccess.Model
{
    public class Address :IAddress
    {
        public virtual Guid Id { get; set; }
        public virtual string Street { get; set; }
        public virtual string City { get; set; }
        public virtual string PostCode { get; set; }
        public virtual Country Country { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}