using TestCrowd.DataAccess.Model;

namespace TestCrowd.DataAccess.Interface
{
    public interface IApplicationUser : IEntity
    {
        string UserName { get; set; }

        string Email { get; set; }
        string PasswordHash { get; set; }
        string PasswordConfirmationHash { get; set; }

        ApplicationRole Role { get; set; }
    }
}
