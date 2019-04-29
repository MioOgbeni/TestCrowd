using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Repository;
using TestCrowd.DataAccess.Repository.Tests;

namespace TestCrowd.Controllers
{
    public class PublicController : Controller
    {
        // GET: Public
        public ActionResult Index()
        {
            ViewBag.Title = "TestCrowd";

            ApplicationUserRepository<ApplicationUser> testCrowdUserDao = new ApplicationUserRepository<ApplicationUser>();
            int count = testCrowdUserDao.GetAll().Count;

            TestCaseRepository testCasesRepo = new TestCaseRepository();
            int testCount = testCasesRepo.GetCount();
            ViewBag.Clients = count.ToString();
            ViewBag.Tests = testCount.ToString();
            return View();
        }
    }
}