using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Repository;

namespace TestCrowd.Areas.Testing.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        // GET: Testing/Profile
        [Authorize(Roles = "company, tester, admin")]
        public ActionResult Index()
        {
            if (User.IsInRole("company"))
            {
                var company = new ApplicationUserRepository<Company>().GetByUserName(User.Identity.Name);

                if (Request.IsAjaxRequest())
                {
                    return PartialView("CompanyProfile", company);
                }

                return View("CompanyProfile", company);
            }

            if (User.IsInRole("admin"))
            {
                var admin = new ApplicationUserRepository<Admin>().GetByUserName(User.Identity.Name);

                if (Request.IsAjaxRequest())
                {
                    return PartialView("AdminProfile", admin);
                }

                return View("AdminProfile", admin);
            }

            var tester = new ApplicationUserRepository<Tester>().GetByUserName(User.Identity.Name);

            if (Request.IsAjaxRequest())
            {
                return PartialView("TesterProfile", tester);
            }

            return View("TesterProfile", tester);
        }
    }
}