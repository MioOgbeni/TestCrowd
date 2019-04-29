using TestCrowd.DataAccess.Interface;

namespace TestCrowd.DataAccess.Model
{
    public class Tester : ApplicationUser, ITester
    {
        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }

        public virtual int Credits { get; set; }
        public virtual Address Address { get; set; }
    }
}