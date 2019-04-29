using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TestCrowd.Class;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Repository;

namespace TestCrowd.Controllers
{
    public class LoginController : Controller
    {
        private ApplicationUserManager userManager;

        public ApplicationUserManager AppUserManager => userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        private IAuthenticationManager AuthManager => HttpContext.GetOwinContext().Authentication;

        // GET: Login
        public ActionResult Index()
        {
            if (AuthManager.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { area = "Testing" });
            }

            ViewBag.Title = "TestCrowd - Login";

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(ApplicationUser user, bool? remember)
        {
            bool boolRemember = remember ?? false;

            ModelState.Remove(nameof(user.PasswordConfirmationHash));
            if (ModelState.IsValid)
            {
                ApplicationUserRepository<ApplicationUser> userRepository = new ApplicationUserRepository<ApplicationUser>();
                var isUserExist = userRepository.IsUserExistAsync(user.UserName);               

                if (!await isUserExist)
                {
                    TempData["error"] = "This Account Name do not exist!";
                    return RedirectToAction("Index", "Login");
                }

                var isUserWithEmailExist = userRepository.IsUserWithEmailExistAcync(user.UserName, user.Email);

                if (!await isUserWithEmailExist)
                {
                    TempData["error"] = "Incorrect Email for this account!";
                    return RedirectToAction("Index", "Login");
                }

                var _user = await AppUserManager.FindAsync(user.UserName, user.PasswordHash);
                if (_user != null)
                {
                    ClaimsIdentity identity = await AppUserManager.CreateIdentityAsync(_user, DefaultAuthenticationTypes.ApplicationCookie);
                    identity.AddClaim(new Claim(ClaimTypes.Role, _user.Role.Name));

                    AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    AuthManager.SignIn(new AuthenticationProperties()
                    {
                        AllowRefresh = true,
                        IsPersistent = boolRemember,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(10)
                    },identity);

                    return RedirectToAction("Index", "Home", new { area = "Testing" });
                }
                
                TempData["error"] = "Wrong Password, Try it again!";
                return RedirectToAction("Index", "Login");
            }

            return View("Index");
        }

        public ActionResult Logout()
        {
            AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            TempData["success"] = "Logged out successfully!";
            return RedirectToAction("Index", "Login");
        }
    }
}