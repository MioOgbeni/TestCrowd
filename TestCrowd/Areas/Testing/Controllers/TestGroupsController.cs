using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CsQuery.ExtensionMethods;
using CsQuery.ExtensionMethods.Internal;
using DataTables.AspNet.Core;
using DataTables.AspNet.Mvc5;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Evidences;
using TestCrowd.DataAccess.Model.Tests;
using TestCrowd.DataAccess.Repository;
using TestCrowd.DataAccess.Repository.Evidences;
using TestCrowd.DataAccess.Repository.Reviews;
using TestCrowd.DataAccess.Repository.Tests;

namespace TestCrowd.Areas.Testing.Controllers
{
    [Authorize(Roles = "company, tester")]
    public class TestGroupsController : Controller
    {
        // GET: Testing/TestGroups
        public ActionResult Index()
        {
            if (User.IsInRole("company"))
            {
                return RedirectToAction("CompanyTestGroups");
            }

            return RedirectToAction("TesterTestGroups");
        }

        [Authorize(Roles = "company")]
        public ActionResult CompanyTestGroups()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "company")]
        public ActionResult CompanyTestGroupsDataTable(IDataTablesRequest request)
        {
            ApplicationUserRepository<Company> companyRepo = new ApplicationUserRepository<Company>();
            var company = companyRepo.GetByUserName(User.Identity.Name);
            var testGroupRepo = new TestGroupRepository();

            var orderColumn = request.Columns.Where(c => c.Sort != null).First();

            bool asc;
            if (orderColumn.Sort.Direction == 0)
            {
                asc = true;
            }
            else
            {
                asc = false;
            }

            var testGroups = testGroupRepo.GetEntitiesForCompany(out int totalCount, out int filteredCount, request.Start, request.Length, nameof(TestCase.Name), request.Search.Value, orderColumn.Name, asc, company);

            var resault = testGroups.Select(x => new
            {
                x.Id,
                x.Name,
                x.SkillDificulty,
                x.TimeDificulty,
                x.RewardMultiplier,
                x.Rating,
                x.Created,
                x.AvailableTo,
                testCasesCount = x.TestCases.Count
            });

            var response = DataTablesResponse.Create(request, totalCount, filteredCount, resault);

            return new DataTablesJsonResult(response, JsonRequestBehavior.DenyGet);
        }

        [Authorize(Roles = "company")]
        public ActionResult CreateTestGroup()
        {
            return View();
        }

        public JsonResult GetTestCases(string searchTerm)
        {
            ApplicationUserRepository<Company> companyRepo = new ApplicationUserRepository<Company>();
            Company currentCompany = companyRepo.GetByUserName(User.Identity.Name);

            TestCaseRepository testCaseRepo = new TestCaseRepository();

            IList<TestCase> testCases;
            testCases = !string.IsNullOrEmpty(searchTerm) ? testCaseRepo.GetFilteredForCompanyNotInGroup(currentCompany, nameof(TestCase.Name), searchTerm) : testCaseRepo.GetFilteredForCompanyNotInGroup(currentCompany);

            var modifiedData = testCases.Select(x => new
            {
                id = x.Id,
                text = x.Name + " (available to: " + x.AvailableTo.ToShortDateString() + ")"
            });
            return Json(modifiedData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Authorize(Roles = "company")]
        public ActionResult AddTestGroup(TestGroup testGroup, string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                TempData["error"] = "At least one test case must be selected!";
                return View("CreateTestGroup");
            }

            ModelState.Remove(nameof(TestGroup.TestCases));
            ModelState.Remove(nameof(TestGroup.Created));
            ModelState.Remove(nameof(TestGroup.Creator));
            ModelState.Remove(nameof(TestGroup.Rating));
            ModelState.Remove(nameof(TestGroup.TimeDificulty));
            ModelState.Remove(nameof(TestGroup.SkillDificulty));
            if (ModelState.IsValid)
            {
                ApplicationUserRepository<Company> userRepo = new ApplicationUserRepository<Company>();
                Company currentUser = userRepo.GetByUserName(User.Identity.Name);

                var testCasesId = data.Split(',');
                TestCaseRepository testCaseRepo = new TestCaseRepository();
                IList<TestCase> testCases = new List<TestCase>();

                foreach (var testCaseId in testCasesId)
                {
                    TestCase tempTestCase = testCaseRepo.GetByIdForCompanyNotInGroup(currentUser, Guid.Parse(testCaseId));
                    tempTestCase.IsInGroup = true;
                    testCases.Add(tempTestCase);
                }

                DateTime latestAvailability = testCases.OrderByDescending(x => x.AvailableTo).First().AvailableTo;
                if (testGroup.AvailableTo < latestAvailability)
                {
                    TempData["error"] = $"Available To must be equal or greater than availability of latest test case ({latestAvailability.ToShortDateString()})!";
                    return View("CreateTestGroup");
                }

                testGroup.TestCases = testCases;
                testGroup.SkillDificulty = testGroup.CountSkillDificulty();
                testGroup.TimeDificulty = testGroup.CountTimeDificulty();

                testGroup.RewardMultiplier = (testGroup.RewardMultiplier + 100) / 100;

                testGroup.Creator = currentUser;
                testGroup.Created = DateTime.Now;

                foreach (var testCase in testCases)
                {
                    testCaseRepo.Update(testCase);
                }

                TestGroupRepository testGroupRepo = new TestGroupRepository();
                testGroupRepo.Create(testGroup);

                TempData["success"] = "Test group was added";
            }
            else
            {
                return View("CreateTestGroup");
            }

            return RedirectToAction("CompanyTestGroups");
        }

        [Authorize(Roles = "company")]
        public ActionResult DetailTestGroupCompany(string id)
        {
            var testGroupRepo = new TestGroupRepository();
            var testGroup = testGroupRepo.GetById(Guid.Parse(id));

            return View(testGroup);
        }

        [Authorize(Roles = "company")]
        public ActionResult DeleteTestGroup(IList<TestGroup> rows)
        {        
            foreach (var row in rows)
            {
                var testGroupRepo = new TestGroupRepository();
                TestGroup testGroup = testGroupRepo.GetById(row.Id);
                foreach (var testCase in testGroup.TestCases)
                {
                    testCase.IsInGroup = false;
                }

                testGroupRepo.Update(testGroup);
                testGroupRepo.Delete(testGroup);
            }

            TempData["success"] = "Test group/s was deleted!";

            return View("CompanyTestGroups");
        }

        [Authorize(Roles = "company")]
        public ActionResult DeleteTestGroupAll(IList<TestGroup> rows)
        {
            IList<Guid> testCasesId = new List<Guid>();
            foreach (var row in rows)
            {
                var testGroupRepo = new TestGroupRepository();
                TestGroup testGroup = testGroupRepo.GetById(row.Id);
                foreach (var testCase in testGroup.TestCases)
                {
                    testCase.IsInGroup = false;
                    testCasesId.Add(testCase.Id);
                }

                testGroupRepo.Update(testGroup);
                testGroupRepo.Delete(testGroup);
            }

            TestCaseRepository testCaseRepo = new TestCaseRepository();
            foreach (var id in testCasesId)
            {
                TestCase testCase = testCaseRepo.GetById(id);

                if (testCase.Evidences != null && testCase.Evidences.Count > 0)
                {
                    var evidenceRepo = new EvidenceRepository();
                    foreach (var evidence in testCase.Evidences)
                    {
                        System.IO.File.Delete(Server.MapPath("~/Uploads/" + evidence.Name + evidence.Extension));
                        evidenceRepo.Delete(evidence);
                    }
                }

                if (testCase.Reviews != null && testCase.Reviews.Count > 0)
                {
                    var reviewsRepo = new ReviewRepository();
                    foreach (var review in testCase.Reviews)
                    {
                        reviewsRepo.Delete(review);
                    }
                }

                testCaseRepo.Delete(testCase);
            }

            TempData["success"] = "Test group/s and tests was deleted!";

            return View("CompanyTestGroups");
        }

        [Authorize(Roles = "company")]
        public ActionResult EditTestGroup(string id)
        {
            var testGroupRepo = new TestGroupRepository();
            var testGroup = testGroupRepo.GetById(Guid.Parse(id));

            return View(testGroup);
        }

        [Authorize(Roles = "company")]
        public ActionResult DeleteAttachedTestCase(string id, string testCaseId)
        {
            TestGroupRepository testGroupRepo = new TestGroupRepository();
            TestCaseRepository testCaseRepo = new TestCaseRepository();

            TestGroup testGroup = testGroupRepo.GetById(Guid.Parse(id));
            if (testGroup.TestCases != null && testGroup.TestCases.Count > 0)
            {
                foreach (var tempTestCase in testGroup.TestCases)
                {
                    if (tempTestCase.Id == Guid.Parse(testCaseId))
                    {
                        testGroup.TestCases.Remove(tempTestCase);
                        break;
                    }
                }
                
                testGroup.SkillDificulty = testGroup.CountSkillDificulty();
                testGroup.TimeDificulty = testGroup.CountTimeDificulty();
                testGroupRepo.Update(testGroup);

                TestCase testCase = testCaseRepo.GetById(Guid.Parse(testCaseId));

                testCase.IsInGroup = false;
                testCaseRepo.Update(testCase);
            }

            return RedirectToAction("EditTestGroup", "TestGroups", new { id = testGroup.Id.ToString() });
        }

        [HttpPost]
        [Authorize(Roles = "company")]
        public ActionResult UpdateTestGroup(TestGroup testGroup, string data)
        {
            TestGroupRepository testGroupRepo = new TestGroupRepository();

            TestGroup oldTestGroup = testGroupRepo.GetById(testGroup.Id);

            if (string.IsNullOrEmpty(data) && oldTestGroup.TestCases.Count <= 0)
            {
                TempData["error"] = "At least one test case must be selected!";
                return View("EditTestGroup", testGroup);
            }

            ModelState.Remove(nameof(TestGroup.TestCases));
            ModelState.Remove(nameof(TestGroup.Created));
            ModelState.Remove(nameof(TestGroup.Creator));
            ModelState.Remove(nameof(TestGroup.Rating));
            ModelState.Remove(nameof(TestGroup.TimeDificulty));
            ModelState.Remove(nameof(TestGroup.SkillDificulty));
            if (ModelState.IsValid)
            {
                var testCasesId = data.Split(',');
                TestCaseRepository testCaseRepo = new TestCaseRepository();

                IList<TestCase> testCases = new List<TestCase>();

                if (!testCasesId[0].IsNullOrEmpty())
                {
                    foreach (var testCaseId in testCasesId)
                    {
                        TestCase tempTestCase = testCaseRepo.GetByIdForCompanyNotInGroup(testGroup.Creator, Guid.Parse(testCaseId));
                        tempTestCase.IsInGroup = true;
                        oldTestGroup.TestCases.Add(tempTestCase);
                        testCases.Add(tempTestCase);
                    }
                }

                DateTime latestAvailability = oldTestGroup.TestCases.OrderByDescending(x => x.AvailableTo).First().AvailableTo;
                if (testGroup.AvailableTo < latestAvailability)
                {
                    testGroup.TestCases = oldTestGroup.TestCases;
                    testGroup.RewardMultiplier = oldTestGroup.RewardMultiplier;

                    TempData["error"] = $"Available To must be equal or greater than availability of latest test case ({latestAvailability.ToShortDateString()})!";
                    return View("EditTestGroup", testGroup);
                }

                oldTestGroup.SkillDificulty = oldTestGroup.CountSkillDificulty();
                oldTestGroup.TimeDificulty = oldTestGroup.CountTimeDificulty();

                oldTestGroup.RewardMultiplier = (testGroup.RewardMultiplier + 100) / 100;

                oldTestGroup.Name = testGroup.Name;
                oldTestGroup.AvailableTo = testGroup.AvailableTo;

                foreach (var testCase in testCases)
                {
                    testCaseRepo.Update(testCase);
                }

                testGroupRepo.Update(oldTestGroup);

                TempData["success"] = "Test group was edited";
            }
            else
            {
                return View("CreateTestGroup");
            }

            return RedirectToAction("CompanyTestGroups");
        }

        [Authorize(Roles = "tester")]
        public async Task<ActionResult> TesterTestGroups(int? page, string searchTerm)
        {
            TestGroupRepository testGroupsRepo = new TestGroupRepository();

            ApplicationUserRepository<Tester> userRepo = new ApplicationUserRepository<Tester>();
            var testerTask = userRepo.GetByUserNameAsync(User.Identity.Name);

            int itemsOnPage = 8;
            int pg = page ?? 1;
            int startIndex = (pg * itemsOnPage) - itemsOnPage;

            ViewBag.CurrentSearch = searchTerm;
            IList<TestGroup> testGroups = testGroupsRepo.GetAvailableEntities(out var totalTests, await testerTask, DateTime.Today, startIndex, itemsOnPage, searchTerm);

            ViewBag.Pages = (int)Math.Ceiling((double)totalTests / (double)itemsOnPage);
            ViewBag.CurrentPage = pg;

            if (Request.IsAjaxRequest())
            {
                return PartialView(testGroups);
            }

            return View(testGroups);
        }

        [Authorize(Roles = "tester")]
        public async Task<ActionResult> TestGroupDetails(Guid testGroupGuid)
        {
            TestGroupRepository testGroupRepo = new TestGroupRepository();
            var testGroupTask = testGroupRepo.GetByIdAsync(testGroupGuid);

            ApplicationUserRepository<Tester> userRepo = new ApplicationUserRepository<Tester>();
            var testerTask = userRepo.GetByUserNameAsync(User.Identity.Name);

            TestGroupsRepository testGroupsRepo = new TestGroupsRepository();

            ViewBag.Takened = testGroupsRepo.GetGroupStatus(await testGroupTask, await testerTask);
            ViewBag.TestsGuid = testGroupsRepo.GetByTestGroupForTester(await testerTask, await testGroupTask);
            return View(await testGroupTask);
        }

        [Authorize(Roles = "tester")]
        public async Task<ActionResult> TakeGroupFromGrid(int? page, string searchTerm, Guid testGroupGuid)
        {
            await TakeGroup(testGroupGuid);

            return RedirectToAction("TesterTestGroups", new { page, searchTerm });
        }

        [Authorize(Roles = "tester")]
        public async Task<ActionResult> TakeGroupFromDetails(Guid testGroupGuid)
        {
            await TakeGroup(testGroupGuid);

            return RedirectToAction("TestGroupDetails", new { testGroupGuid });
        }

        [Authorize(Roles = "tester")]
        public async Task TakeGroup(Guid groupId)
        {
            TestGroupRepository testGroupRepo = new TestGroupRepository();
            var testGroupTask = testGroupRepo.GetByIdAsync(groupId);

            ApplicationUserRepository<Tester> userRepo = new ApplicationUserRepository<Tester>();
            var testerTask = userRepo.GetByUserNameAsync(User.Identity.Name);

            TestGroups testGroups = new TestGroups();
            testGroups.TestGroup = await testGroupTask;
            testGroups.Status = GroupStatus.Takened;
            testGroups.Takened = DateTime.Now;
            testGroups.Tester = await testerTask;

            TestGroup testGroup = await testGroupTask;
            TestsRepository testsRepo = new TestsRepository();

            foreach (var testCase in testGroup.TestCases)
            {
                if (!testsRepo.IsTestTakened(testCase, await testerTask))
                {
                    DataAccess.Model.Tests.Tests tests = new DataAccess.Model.Tests.Tests();
                    tests.Test = testCase;
                    tests.Status = TestsStatus.Takened;
                    tests.Takened = DateTime.Now;
                    tests.Tester = await testerTask;

                    testsRepo.Create(tests);
                }
            }

            TestGroupsRepository testGroupsRepo = new TestGroupsRepository();
            testGroupsRepo.Create(testGroups);
        }

        [Authorize(Roles = "tester")]
        public async Task<ActionResult> TakenedTestGroups(int? page, string searchTerm)
        {
            TestGroupsRepository testGroupsRepo = new TestGroupsRepository();

            ApplicationUserRepository<Tester> userRepo = new ApplicationUserRepository<Tester>();
            var testerTask = userRepo.GetByUserNameAsync(User.Identity.Name);

            int itemsOnPage = 8;
            int pg = page ?? 1;
            int startIndex = (pg * itemsOnPage) - itemsOnPage;

            ViewBag.CurrentSearch = searchTerm;
            IList<TestGroups> testGroups = testGroupsRepo.GetAvailableEntities(out var totalTests, await testerTask, GroupStatus.Takened, startIndex, itemsOnPage, searchTerm);

            ViewBag.Pages = (int)Math.Ceiling((double)totalTests / (double)itemsOnPage);
            ViewBag.CurrentPage = pg;

            if (Request.IsAjaxRequest())
            {
                return PartialView(testGroups);
            }

            return View(testGroups);
        }

        [Authorize(Roles = "tester")]
        public async Task<ActionResult> FinishedTestGroups(int? page, string searchTerm)
        {
            TestGroupsRepository testGroupsRepo = new TestGroupsRepository();

            ApplicationUserRepository<Tester> userRepo = new ApplicationUserRepository<Tester>();
            var testerTask = userRepo.GetByUserNameAsync(User.Identity.Name);

            int itemsOnPage = 8;
            int pg = page ?? 1;
            int startIndex = (pg * itemsOnPage) - itemsOnPage;

            ViewBag.CurrentSearch = searchTerm;
            IList<TestGroups> testGroups = testGroupsRepo.GetAvailableEntities(out var totalTests, await testerTask, GroupStatus.Finished, startIndex, itemsOnPage, searchTerm);

            ViewBag.Pages = (int)Math.Ceiling((double)totalTests / (double)itemsOnPage);
            ViewBag.CurrentPage = pg;

            if (Request.IsAjaxRequest())
            {
                return PartialView(testGroups);
            }

            return View(testGroups);
        }

        [Authorize(Roles = "tester")]
        public async Task<ActionResult> ResolveGroup(Guid testGroupGuid)
        {

            TestGroupsRepository testGroupsRepo = new TestGroupsRepository();
            var testGroups = testGroupsRepo.GetById(testGroupGuid);

            ApplicationUserRepository<Tester> userRepo = new ApplicationUserRepository<Tester>();
            var testerTask = userRepo.GetByUserNameAsync(User.Identity.Name);

            TestsRepository testsRepo = new TestsRepository();
            IList<DataAccess.Model.Tests.Tests> tests = new List<DataAccess.Model.Tests.Tests>();
            foreach (var testCase in testGroups.TestGroup.TestCases)
            {
                DataAccess.Model.Tests.Tests testsRecord = testsRepo.GetByTestCaseForTesterByStatus(await testerTask, testCase, TestsStatus.Takened);
                if (testsRecord != null)
                {
                    tests.Add(testsRecord);
                }               
            }

            if (tests.Count <= 0)
            {
                testGroups.Finished = DateTime.Now;
                testGroups.Status = GroupStatus.Finished;
                testGroups.TestGroup.Rating = testGroups.TestGroup.CountRating();

                testGroupsRepo.Update(testGroups);

                return RedirectToAction("FinishedTestGroups");
            }

            TestStatusRepository testStatusRepo = new TestStatusRepository();
            ViewBag.TestStatus = testStatusRepo.GetAll();
            ViewBag.TestGroup = testGroups.TestGroup.Name;
            ViewBag.TestGroupsGuid = testGroups.Id;

            return View(tests.First());
        }

        [HttpPost]
        [Authorize(Roles = "tester")]
        public async Task<ActionResult> Resolve(DataAccess.Model.Tests.Tests tests, HttpPostedFileBase[] files, Guid testGroupsGuid)
        {
            TestsRepository testsRepo = new TestsRepository();
            DataAccess.Model.Tests.Tests testsRecord = testsRepo.GetById(tests.Id);

            TestGroupsRepository testGroupsRepo = new TestGroupsRepository();
            TestGroups testGroups = testGroupsRepo.GetById(testGroupsGuid);

            TestStatusRepository testStatusRepo = new TestStatusRepository();

            ModelState.Remove("files");
            ModelState.Remove("testGroupsGuid");
            ModelState.Remove(nameof(DataAccess.Model.Tests.Tests.Tester));
            ModelState.Remove(nameof(DataAccess.Model.Tests.Tests.Status));
            ModelState.Remove(nameof(DataAccess.Model.Tests.Tests.Test));
            ModelState.Remove(nameof(DataAccess.Model.Tests.Tests.Evidences));
            ModelState.Remove(nameof(DataAccess.Model.Tests.Tests.Finished));
            ModelState.Remove(nameof(DataAccess.Model.Tests.Tests.Rejected));
            ModelState.Remove(nameof(DataAccess.Model.Tests.Tests.Takened));
            ModelState.Remove("TestStatus.Status");
            if (ModelState.IsValid)
            {
                if (testsRecord.Test.Creator.Credits < (int)Math.Ceiling(testsRecord.Test.Reward * testGroups.TestGroup.RewardMultiplier))
                {
                    ViewBag.TestStatus = testStatusRepo.GetAll();
                    ViewBag.TestGroup = testGroups.TestGroup.Name;
                    ViewBag.TestGroupsGuid = testGroups.Id;

                    TempData["error"] = "Createor of tests don't have required amount of coins. Please try resolve test later, or contact our support team.";
                    return RedirectToAction("TakenedTestGroups");
                }

                var testStatusTask = testStatusRepo.GetByIdAsync(tests.TestStatus.Id);

                IList<Evidence> evidences = new List<Evidence>();

                if (files[0] != null)
                {
                    var maxSizeInMb = 20;
                    var byteSize = 1048576;
                    var maxSizeInBytes = byteSize * maxSizeInMb;
                    foreach (var file in files)
                    {
                        if (file.ContentLength > maxSizeInBytes)
                        {
                            TempData["error"] = "File " + file.FileName + " is too big! (max size is " + maxSizeInMb + "MB)";

                            ViewBag.TestStatus = testStatusRepo.GetAll();
                            ViewBag.TestGroup = testGroups.TestGroup.Name;
                            ViewBag.TestGroupsGuid = testGroups.Id;
                            return View("ResolveGroup", testsRecord);
                        }
                    }

                    foreach (var file in files)
                    {
                        Evidence evidence = new Evidence();
                        evidence.Id = Guid.NewGuid();
                        evidence.Name = evidence.Id.ToString();
                        evidence.RealName = Path.GetFileNameWithoutExtension(file.FileName);
                        evidence.Attached = DateTime.Now;

                        var extension = Path.GetExtension(file.FileName);
                        evidence.Extension = extension;

                        var path = Path.Combine(Server.MapPath($"~/Uploads/{testsRecord.Id}"), evidence.Name + extension);
                        Directory.CreateDirectory(Server.MapPath($"~/Uploads/{testsRecord.Id}"));
                        file.SaveAs(path);

                        evidences.Add(evidence);
                    }

                    testsRecord.Evidences = evidences;
                }

                testsRecord.TestStatus = await testStatusTask;
                testsRecord.Finished = DateTime.Now;
                testsRecord.Status = TestsStatus.Finished;

                testsRecord.Tester.Credits = testsRecord.Tester.Credits + (int)Math.Ceiling(testsRecord.Test.Reward * testGroups.TestGroup.RewardMultiplier);
                testsRecord.Test.Creator.Credits = testsRecord.Test.Creator.Credits - (int)Math.Ceiling(testsRecord.Test.Reward * testGroups.TestGroup.RewardMultiplier);

                EvidenceRepository evidRepo = new EvidenceRepository();

                foreach (var evidence in evidences)
                {
                    evidRepo.Create(evidence);
                }

                testsRepo.Update(testsRecord);

                return RedirectToAction("ResolveGroup", new { testGroupGuid = testGroups.Id});
            }

            ViewBag.TestStatus = testStatusRepo.GetAll();
            ViewBag.TestGroup = testGroups.TestGroup.Name;
            ViewBag.TestGroupsGuid = testGroups.Id;

            return View("ResolveGroup", testsRecord);
        }
    }
}