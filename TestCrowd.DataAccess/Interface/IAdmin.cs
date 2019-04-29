namespace TestCrowd.DataAccess.Interface
{
    public interface IAdmin : IApplicationUser
    {
        string FirstName { get; set; }
        string LastName { get; set; }
    }
}