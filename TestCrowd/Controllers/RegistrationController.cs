using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using TestCrowd.DataAccess.Model;
using TestCrowd.Class;
using TestCrowd.DataAccess.Repository;

namespace TestCrowd.Controllers
{
    public class RegistrationController : Controller
    {
        private ApplicationUserManager userManager;

        public ApplicationUserManager AppUserManager => userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

        private IAuthenticationManager AuthManager => HttpContext.GetOwinContext().Authentication;

        // GET: Registration
        public ActionResult Index(string id)
        {
            if (id.IsEmpty()) id = "tester";
            ViewBag.Title = "TestCrowd - Register " + id;

            if (id == "company")
            {
                return View("Company");
            }

            return View("Tester");
        }

        [HttpPost]
        public async Task<ActionResult> RegisterTester(Tester tester)
        {
            ModelState.Remove(nameof(tester.FirstName));
            ModelState.Remove(nameof(tester.LastName));

            if (ModelState.IsValid)
            {    
                var userRepository = new ApplicationUserRepository<ApplicationUser>();
                var roleRepository = new ApplicationRoleRepository();
                var role = roleRepository.GetByNameAsync("tester");
                var isUserExist = userRepository.IsUserExistAsync(tester.UserName);

                if (await isUserExist)
                {
                    TempData["error"] = "Account with this name already exist!";
                    return View("Tester");
                }

                Tester _tester = new Tester()
                {
                    UserName = tester.UserName,
                    Email = tester.Email,
                    Role = await role,
                    PasswordHash = ApplicationPasswordHasher.HashPasword(tester.PasswordHash)
                };

                _tester.Address = new Address(){User = _tester};
                new ApplicationUserRepository<Tester>().Create(_tester);

                ClaimsIdentity identity = await AppUserManager.CreateIdentityAsync(_tester, DefaultAuthenticationTypes.ApplicationCookie);
                identity.AddClaim(new Claim(ClaimTypes.Role, _tester.Role.Name));

                AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                AuthManager.SignIn(new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(10)
                }, identity);

                return RedirectToAction("Index", "Home", new { area = "Testing" });
            }

            return View("Tester");
        }

        [HttpPost]
        public async Task<ActionResult> RegisterCompany(Company company)
        {
            ModelState.Remove(nameof(company.Rating));
            ModelState.Remove(nameof(company.Reviews));
            ModelState.Remove(nameof(company.CompanyName));

            if (ModelState.IsValid)
            {
                var userRepository = new ApplicationUserRepository<ApplicationUser>();
                var roleRepository = new ApplicationRoleRepository();
                var role = roleRepository.GetByNameAsync("company");
                var isUserExist = userRepository.IsUserExistAsync(company.UserName);

                if (await isUserExist)
                {
                    TempData["error"] = "Account with this name already exist!";
                    return View("Company");
                }

                Company _company = new Company()
                {
                    UserName = company.UserName,
                    Email = company.Email,
                    Role = await role,
                    PasswordHash = ApplicationPasswordHasher.HashPasword(company.PasswordHash),
                    Address = new Address()
                };

                new ApplicationUserRepository<Company>().Create(_company);

                ClaimsIdentity identity = await AppUserManager.CreateIdentityAsync(_company, DefaultAuthenticationTypes.ApplicationCookie);

                AuthManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                AuthManager.SignIn(new AuthenticationProperties()
                {
                    AllowRefresh = true,
                    IsPersistent = false,
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(10)
                }, identity);

                return RedirectToAction("Index", "Home", new { area = "Testing" });
            }

            return View("Company");
        }
    }
}