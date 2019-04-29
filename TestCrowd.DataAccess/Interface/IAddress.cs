using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Interface
{
    public interface IAddress :IEntity
    {
        string Street { get; set; }
        string City { get; set; }
        string PostCode { get; set; }

        Country Country { get; set; }

        ApplicationUser User { get; set; }
    }
}