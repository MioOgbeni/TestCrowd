using System;
using TestCrowd.DataAccess.Interface;

namespace TestCrowd.DataAccess.Model
{
    public class Country :ICountry
    {
        public virtual Guid Id { get; set; }
        public virtual string IdentificationCode { get; set; }
        public virtual string Name { get; set; }
    }
}