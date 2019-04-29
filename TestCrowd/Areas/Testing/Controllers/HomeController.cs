using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CsQuery.ExtensionMethods.Internal;
using TestCrowd.Class;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Disputes;
using TestCrowd.DataAccess.Model.Tests;
using TestCrowd.DataAccess.Repository;
using TestCrowd.DataAccess.Repository.Disputes;
using TestCrowd.DataAccess.Repository.Tests;

namespace TestCrowd.Areas.Testing.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        // GET: Testing/Home
        public async Task<ActionResult> Index()
        {
            if (User.IsInRole("company"))
            {
                ViewBag.Role = "company";

                ApplicationUserRepository<Company> companyRepo = new ApplicationUserRepository<Company>();
                TestCaseRepository testCaseRepo = new TestCaseRepository();
                TestGroupRepository testGroupRepo = new TestGroupRepository();
                DisputeRepository disputeRepo = new DisputeRepository();
                Company company = companyRepo.GetByUserName(User.Identity.Name);

                var tests = testCaseRepo.GetCountForCompanyAsync(company);
                var testGroups = testGroupRepo.GetCountForCompanyAsync(company);
                var disputes = disputeRepo.GetCountWithCompanyAsync(company);

                ViewBag.Credits = company.Credits;
                ViewBag.Tests = await tests;
                ViewBag.TestGroups = await testGroups;
                ViewBag.Disputes = await disputes;
            }

            if (User.IsInRole("tester"))
            {
                ViewBag.Role = "tester";

                ApplicationUserRepository<Tester> testerRepo = new ApplicationUserRepository<Tester>();
                DisputeRepository disputeRepo = new DisputeRepository();
                TestsRepository testRepo = new TestsRepository();
                var tester = testerRepo.GetByUserName(User.Identity.Name);
                var dispute = disputeRepo.GetCountWithTesterAsync(tester);

                IList<TestsStatus> takened = new List<TestsStatus>();
                takened.Add(TestsStatus.Takened);

                IList<TestsStatus> finished = new List<TestsStatus>();
                finished.Add(TestsStatus.Finished);
                finished.Add(TestsStatus.Reviewed);

                ViewBag.Credits = tester.Credits;
                ViewBag.Disputes = await dispute;
                ViewBag.ResolvedTests = testRepo.GetCountByStatus(finished, tester);
                ViewBag.TakenedTests = testRepo.GetCountByStatus(takened, tester);
            }

            if (User.IsInRole("admin"))
            {
                ViewBag.Role = "admin";

                ApplicationUserRepository<Admin> adminRepo = new ApplicationUserRepository<Admin>();
                DisputeRepository disputeRepo1 = new DisputeRepository();
                DisputeRepository disputeRepo2 = new DisputeRepository();
                DisputeRepository disputeRepo3 = new DisputeRepository();
                SoftwareTypeRepository swTypeRepo = new SoftwareTypeRepository();
                TestCaseRepository testCatRepo = new TestCaseRepository();

                Admin admin = adminRepo.GetByUserName(User.Identity.Name);

                var disputes = disputeRepo1.GetCountPandingAsync();
                var inProgressDisputes = disputeRepo2.GetCountForAdminInProgressAsync(admin);
                var resolvedDisputes = disputeRepo3.GetCountForAdminResolvedAsync(admin);
                var admins = adminRepo.GetCountAsync();
                var softwareTypes = swTypeRepo.GetCountAsync();
                var testCategories = testCatRepo.GetCountAsync();

                ViewBag.Disputes = await disputes;
                ViewBag.InProgressDisputes = await inProgressDisputes;
                ViewBag.ResolvedDisputes = await resolvedDisputes;
                ViewBag.Admins = await admins;
                ViewBag.SoftwareTypes = await softwareTypes;
                ViewBag.TestCategories = await testCategories;
            }

            return View();
        }

        public ActionResult ProfileInfo()
        {
            if (User.IsInRole("company"))
            {
                var company = new ApplicationUserRepository<Company>().GetByUserName(User.Identity.Name);

                if (company.CompanyName.IsNullOrEmpty())
                {
                    ViewBag.ShowedName = company.UserName;
                }
                else
                {
                    ViewBag.ShowedName = company.CompanyName;
                }

                ViewBag.Role = company.Role.Name;
                ViewBag.EmailHash = Helpers.CalculateMd5Hash(company.Email).ToLower();
                return PartialView("ProfilePartials/ProfileBarCompany");
            }

            if (User.IsInRole("admin"))
            {
                var admin = new ApplicationUserRepository<Admin>().GetByUserName(User.Identity.Name);

                if (admin.FirstName.IsNullOrEmpty() || admin.LastName.IsNullOrEmpty())
                {
                    ViewBag.ShowedName = $"{admin.UserName} ({admin.Role.RoleName})";
                }
                else
                {
                    ViewBag.ShowedName = $"{admin.FirstName} {admin.LastName} ({admin.Role.RoleName})";
                }

                ViewBag.EmailHash = Helpers.CalculateMd5Hash(admin.Email).ToLower();
                return PartialView("ProfilePartials/ProfileBarAdmin");
            }

            var tester = new ApplicationUserRepository<Tester>().GetByUserName(User.Identity.Name);

            if (tester.FirstName.IsNullOrEmpty() || tester.LastName.IsNullOrEmpty())
            {
                ViewBag.ShowedName = tester.UserName;
            }
            else
            {
                ViewBag.ShowedName = $"{tester.FirstName} {tester.LastName}";
            }

            ViewBag.Role = tester.Role.Name;
            ViewBag.EmailHash = Helpers.CalculateMd5Hash(tester.Email).ToLower();
            return PartialView("ProfilePartials/ProfileBarTester");
        }

        public ActionResult Menu()
        {
            if (User.IsInRole("company"))
            {
                return PartialView("MenuPartials/MenuCompany");
            }

            if (User.IsInRole("admin"))
            {
                return PartialView("MenuPartials/MenuAdmin");
            }

            return PartialView("MenuPartials/MenuTester");
        }
    }
}