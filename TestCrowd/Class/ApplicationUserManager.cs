using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using TestCrowd.DataAccess.Interface.Repository;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Repository;

namespace TestCrowd.Class
{
    public class ApplicationUserManager :UserManager<ApplicationUser, Guid>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser, Guid> userStore) : base(userStore)
        {

        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            return new ApplicationUserManager(new ApplicationUserRepository<ApplicationUser>());
        }

        public override Task<ApplicationUser> FindAsync(string userName, string password)
        {
            IApplicationUserRepository<ApplicationUser> userRepository = new ApplicationUserRepository<ApplicationUser>();
            var user = userRepository.GetByUserName(userName);

            if (user != null && ApplicationPasswordHasher.VerifyPassword(user.PasswordHash, password))
            {
                return Task.FromResult(user);
            }

            return Task.FromResult<ApplicationUser>(null);
        }

        public static Task<ApplicationUser> IsNameExist(string userName)
        {
            IApplicationUserRepository<ApplicationUser> userRepository = new ApplicationUserRepository<ApplicationUser>();
            var user = userRepository.GetByUserName(userName);

            return Task.FromResult<ApplicationUser>(null);
        }
    }
}