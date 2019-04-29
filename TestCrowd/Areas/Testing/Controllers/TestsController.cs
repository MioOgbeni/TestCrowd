using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataTables.AspNet.Core;
using DataTables.AspNet.Mvc5;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Evidences;
using TestCrowd.DataAccess.Model.Reviews;
using TestCrowd.DataAccess.Model.Tests;
using TestCrowd.DataAccess.Repository;
using TestCrowd.DataAccess.Repository.Evidences;
using TestCrowd.DataAccess.Repository.Reviews;
using TestCrowd.DataAccess.Repository.Tests;

namespace TestCrowd.Areas.Testing.Controllers
{
    [Authorize(Roles = "company, tester")]
    public class TestsController : Controller
    {
        // GET: Testing/Tests
        public ActionResult Index()
        {
            if (User.IsInRole("company"))
            {
                return RedirectToAction("CompanyTests");
            }

            return RedirectToAction("TesterTests");
        }

        [Authorize(Roles = "company")]
        public ActionResult CompanyTests()
        {
            return View();
        }

        [Authorize(Roles = "company")]
        public ActionResult TestsResaults(int? page, Guid? testCaseGuid, Guid? statusGuid)
        {
            TestsRepository testsRepo = new TestsRepository();

            ApplicationUserRepository<Company> userRepo = new ApplicationUserRepository<Company>();
            Company company = userRepo.GetByUserName(User.Identity.Name);

            TestCaseRepository testCaseRepo = new TestCaseRepository();
            ViewBag.TestCases = testCaseRepo.GetAllForCompany(company);

            TestStatusRepository statusRepo = new TestStatusRepository();
            ViewBag.Statuses = statusRepo.GetAll();

            int itemsOnPage = 20;
            int pg = page ?? 1;
            int startIndex = (pg * itemsOnPage) - itemsOnPage;

            TestCase testCase = null;
            if (testCaseGuid != null)
            {
                testCase = testCaseRepo.GetById((Guid)testCaseGuid);
                ViewBag.TestCase = testCase;
            }

            TestStatus testStatus = null;
            if (statusGuid != null)
            {
                testStatus = statusRepo.GetById((Guid)statusGuid);
                ViewBag.Status = testStatus;
            }

            IList<DataAccess.Model.Tests.Tests> testses = testsRepo.GetTestsForCompany(company, out int totalTests, out int filteredCount, testCase, testStatus, startIndex, itemsOnPage);

            ViewBag.Pages = (int)Math.Ceiling((double)totalTests / (double)itemsOnPage);
            ViewBag.CurrentPage = pg;

            if (Request.IsAjaxRequest())
            {
                return PartialView(testses);
            }

            return View(testses);
        }

        [HttpPost]
        [Authorize(Roles = "company")]
        public ActionResult CompanyTestsDataTable(IDataTablesRequest request)
        {
            ApplicationUserRepository<Company> companyRepo = new ApplicationUserRepository<Company>();
            var company = companyRepo.GetByUserName(User.Identity.Name);
            var testCaseRepo = new TestCaseRepository();

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

            var testCases = testCaseRepo.GetEntitiesForCompany(out int totalCount, out int filteredCount, request.Start, request.Length, nameof(TestCase.Name), request.Search.Value, orderColumn.Name, asc, company).Select(x => new
            {
                x.Id,
                x.Name,
                x.SkillDificulty,
                x.TimeDificulty,
                x.Reward,
                x.Rating,
                x.Created,
                x.AvailableTo,
                x.SoftwareType,
                x.Category
            });

            var response = DataTablesResponse.Create(request, totalCount, filteredCount, testCases);

            return new DataTablesJsonResult(response, JsonRequestBehavior.DenyGet);
        }

        [Authorize(Roles = "company")]
        public ActionResult CreateTestCase()
        {
            SoftwareTypeRepository swTypeRepo = new SoftwareTypeRepository();
            IList<SoftwareType> swTypes = swTypeRepo.GetAllValid();
            ViewBag.SoftwareTypes = swTypes;

            TestCategoryRepository testCatRepo = new TestCategoryRepository();
            IList<TestCategory> testCats = testCatRepo.GetAllValid();
            ViewBag.TestCategories = testCats;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "company")]
        public async Task<ActionResult> AddTest(TestCase testCase, HttpPostedFileBase[] files)
        {
            ModelState.Remove(nameof(testCase.Evidences));
            ModelState.Remove(nameof(testCase.Rating));
            ModelState.Remove(nameof(testCase.Created));
            ModelState.Remove(nameof(testCase.Creator));
            ModelState.Remove(nameof(testCase.Reviews));
            ModelState.Remove(nameof(testCase.IsInGroup));
            ModelState.Remove("SoftwareType.Name");
            ModelState.Remove("Category.Name");
            ModelState.Remove("files");
            if (ModelState.IsValid)
            {
                TestCategoryRepository testCatRepo = new TestCategoryRepository();
                SoftwareTypeRepository swTypeRepo = new SoftwareTypeRepository();
                ApplicationUserRepository<Company> userRepo = new ApplicationUserRepository<Company>();
                IList<Evidence> evidences = new List<Evidence>();

                if (testCase.AvailableTo <= DateTime.Now)
                {
                    IList<SoftwareType> swTypes = swTypeRepo.GetAllValid();
                    IList<TestCategory> testCats = testCatRepo.GetAllValid();
                    ViewBag.SoftwareTypes = swTypes;
                    ViewBag.TestCategories = testCats;

                    TempData["error"] = "Available To must be in future!";
                    return View("CreateTestCase", testCase);
                }

                if (files[0] != null)
                {
                    var maxSizeInMb = 20;
                    var byteSize = 1048576;
                    var maxSizeInBytes = byteSize * maxSizeInMb;
                    foreach (var file in files)
                    {
                        if (file.ContentLength > maxSizeInBytes)
                        {
                            IList<SoftwareType> swTypes = swTypeRepo.GetAllValid();
                            IList<TestCategory> testCats = testCatRepo.GetAllValid();
                            ViewBag.SoftwareTypes = swTypes;
                            ViewBag.TestCategories = testCats;

                            TempData["error"] = "File " + file.FileName + " is too big! (max size is " + maxSizeInMb + "MB)";
                            return View("CreateTestCase", testCase);
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

                        var path = Path.Combine(Server.MapPath("~/Uploads"), evidence.Name + extension);
                        Directory.CreateDirectory(Server.MapPath("~/Uploads"));
                        file.SaveAs(path);

                        evidences.Add(evidence);
                    }

                    testCase.Evidences = evidences;
                }

                var swType = swTypeRepo.GetByIdAsync(testCase.SoftwareType.Id);
                var testCat = testCatRepo.GetByIdAsync(testCase.Category.Id);
                var currentUser = userRepo.GetByUserNameAsync(User.Identity.Name);

                testCase.SoftwareType = await swType;
                testCase.Category = await testCat;
                testCase.Creator = await currentUser;
                testCase.Created = DateTime.Now;

                TestCaseRepository testRepo = new TestCaseRepository();
                EvidenceRepository evidRepo = new EvidenceRepository();

                foreach (var evidence in evidences)
                {
                    evidRepo.Create(evidence);
                }
                testRepo.Create(testCase);
                TempData["success"] = "Test case was added";
            }
            else
            {
                SoftwareTypeRepository swTypeRepo = new SoftwareTypeRepository();
                IList<SoftwareType> swTypes = swTypeRepo.GetAllValid();
                ViewBag.SoftwareTypes = swTypes;

                TestCategoryRepository testCatRepo = new TestCategoryRepository();
                IList<TestCategory> testCats = testCatRepo.GetAllValid();
                ViewBag.TestCategories = testCats;

                if (!swTypeRepo.IsSwTypeExist(testCase.SoftwareType.Id))
                {
                    TempData["error"] = "Please select Software type!";
                    return View("CreateTestCase", testCase);
                }

                if (!testCatRepo.IsTestCatExist(testCase.Category.Id))
                {
                    TempData["error"] = "Please select Category!";
                    return View("CreateTestCase", testCase);
                }

                return View("CreateTestCase", testCase);
            }

            return RedirectToAction("CompanyTests");
        }

        [Authorize(Roles = "company")]
        public ActionResult DeleteTestCase(IList<TestCase> rows)
        {
            var testCaseRepo = new TestCaseRepository();

            foreach (var row in rows)
            {
                var testCase = testCaseRepo.GetById(row.Id);

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

            TempData["success"] = "Test case/s was deleted!";

            return View("CompanyTests");
        }

        [Authorize(Roles = "company")]
        public ActionResult EditTestCase(string id)
        {
            SoftwareTypeRepository swTypeRepo = new SoftwareTypeRepository();
            IList<SoftwareType> swTypes = swTypeRepo.GetAllValid();
            ViewBag.SoftwareTypes = swTypes;

            TestCategoryRepository testCatRepo = new TestCategoryRepository();
            IList<TestCategory> testCats = testCatRepo.GetAllValid();
            ViewBag.TestCategories = testCats;

            var testCaseRepo = new TestCaseRepository();
            var testCase = testCaseRepo.GetById(Guid.Parse(id));
            return View(testCase);
        }

        [Authorize(Roles = "company")]
        public ActionResult DeleteAttachment(string id, string attachment)
        {
            TestCaseRepository testCaseRepo = new TestCaseRepository();
            EvidenceRepository evidenceRepo = new EvidenceRepository();

            TestCase testCase = testCaseRepo.GetById(Guid.Parse(id));

            if (testCase.Evidences != null && testCase.Evidences.Count > 0)
            {
                foreach (var evidence in testCase.Evidences)
                {
                    if (evidence.Id == Guid.Parse(attachment))
                    {
                        System.IO.File.Delete(Server.MapPath("~/Uploads/" + evidence.Name + evidence.Extension));
                        evidenceRepo.Delete(evidence);
                    }
                }
            }

            SoftwareTypeRepository swTypeRepo = new SoftwareTypeRepository();
            IList<SoftwareType> swTypes = swTypeRepo.GetAllValid();
            ViewBag.SoftwareTypes = swTypes;

            TestCategoryRepository testCatRepo = new TestCategoryRepository();
            IList<TestCategory> testCats = testCatRepo.GetAllValid();
            ViewBag.TestCategories = testCats;

            return RedirectToAction("EditTestCase", "Tests", new { id = testCase.Id.ToString()});
        }

        [HttpPost]
        [Authorize(Roles = "company")]
        public async Task<ActionResult> UpdateTestCase(TestCase testCase, HttpPostedFileBase[] files)
        {
            ModelState.Remove(nameof(testCase.Evidences));
            ModelState.Remove(nameof(testCase.Rating));
            ModelState.Remove(nameof(testCase.Created));
            ModelState.Remove(nameof(testCase.Creator));
            ModelState.Remove(nameof(testCase.Reviews));
            ModelState.Remove(nameof(testCase.IsInGroup));
            ModelState.Remove("SoftwareType.Name");
            ModelState.Remove("Category.Name");
            ModelState.Remove("files");
            if (ModelState.IsValid)
            {
                TestCaseRepository testCaseRepo = new TestCaseRepository();
                TestCategoryRepository testCatRepo = new TestCategoryRepository();
                SoftwareTypeRepository swTypeRepo = new SoftwareTypeRepository();

                TestCase oldTestCase = testCaseRepo.GetById(testCase.Id);

                IList<Evidence> evidences = new List<Evidence>();

                if (testCase.AvailableTo <= DateTime.Now)
                {
                    IList<SoftwareType> swTypes = swTypeRepo.GetAllValid();
                    IList<TestCategory> testCats = testCatRepo.GetAllValid();
                    ViewBag.SoftwareTypes = swTypes;
                    ViewBag.TestCategories = testCats;

                    TempData["error"] = "Available To must be in future!";
                    return View("EditTestCase", testCase);
                }

                if (files[0] != null)
                {
                    if (oldTestCase.Evidences == null || oldTestCase.Evidences.Count <= 0)
                    {
                        oldTestCase.Evidences = new List<Evidence>();
                    }

                    var maxSizeInMb = 20;
                    var byteSize = 1048576;
                    var maxSizeInBytes = byteSize * maxSizeInMb;
                    foreach (var file in files)
                    {
                        if (file.ContentLength > maxSizeInBytes)
                        {
                            IList<SoftwareType> swTypes = swTypeRepo.GetAllValid();
                            IList<TestCategory> testCats = testCatRepo.GetAllValid();
                            ViewBag.SoftwareTypes = swTypes;
                            ViewBag.TestCategories = testCats;

                            TempData["error"] = "File " + file.FileName + " is too big! (max size is " + maxSizeInMb + "MB)";
                            return View("EditTestCase", testCase);
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

                        var path = Path.Combine(Server.MapPath("~/Uploads"), evidence.Name + extension);
                        Directory.CreateDirectory(Server.MapPath("~/Uploads"));
                        file.SaveAs(path);

                        oldTestCase.Evidences.Add(evidence);
                        evidences.Add(evidence);
                    }
                }

                var swType = swTypeRepo.GetByIdAsync(testCase.SoftwareType.Id);
                var testCat = testCatRepo.GetByIdAsync(testCase.Category.Id);

                oldTestCase.SoftwareType = await swType;
                oldTestCase.Category = await testCat;
                oldTestCase.Name = testCase.Name;
                oldTestCase.SkillDificulty = testCase.SkillDificulty;
                oldTestCase.TimeDificulty = testCase.TimeDificulty;
                oldTestCase.AvailableTo = testCase.AvailableTo;
                oldTestCase.Description = testCase.Description;
                oldTestCase.Reward = testCase.Reward;

                EvidenceRepository evidRepo = new EvidenceRepository();

                foreach (var evidence in evidences)
                {
                    evidRepo.Create(evidence);
                }
                testCaseRepo.Update(oldTestCase);
                TempData["success"] = "Test case was edited";
            }
            else
            {
                SoftwareTypeRepository swTypeRepo = new SoftwareTypeRepository();
                IList<SoftwareType> swTypes = swTypeRepo.GetAllValid();
                ViewBag.SoftwareTypes = swTypes;

                TestCategoryRepository testCatRepo = new TestCategoryRepository();
                IList<TestCategory> testCats = testCatRepo.GetAllValid();
                ViewBag.TestCategories = testCats;

                if (!swTypeRepo.IsSwTypeExist(testCase.SoftwareType.Id))
                {
                    TempData["error"] = "Please select Software type!";
                    return View("EditTestCase", testCase);
                }

                if (!testCatRepo.IsTestCatExist(testCase.Category.Id))
                {
                    TempData["error"] = "Please select Category!";
                    return View("EditTestCase", testCase);
                }

                return View("EditTestCase", testCase);
            }

            return RedirectToAction("CompanyTests");
        }

        [Authorize(Roles = "company")]
        public ActionResult DetailTestCaseCompany(string id)
        {
            SoftwareTypeRepository swTypeRepo = new SoftwareTypeRepository();
            IList<SoftwareType> swTypes = swTypeRepo.GetAllValid();
            ViewBag.SoftwareTypes = swTypes;

            TestCategoryRepository testCatRepo = new TestCategoryRepository();
            IList<TestCategory> testCats = testCatRepo.GetAllValid();
            ViewBag.TestCategories = testCats;

            var testCaseRepo = new TestCaseRepository();
            var testCase = testCaseRepo.GetById(Guid.Parse(id));

            return View(testCase);
        }

        [Authorize(Roles = "tester")]
        public async Task<ActionResult> TesterTests(int? page, Guid? swTypeGuid, Guid? testCatGuid, string searchTerm)
        {
            TestCaseRepository testCaseRepo = new TestCaseRepository();

            ApplicationUserRepository<Tester> userRepo = new ApplicationUserRepository<Tester>();
            var testerTask = userRepo.GetByUserNameAsync(User.Identity.Name);

            SoftwareTypeRepository swTypeRepo = new SoftwareTypeRepository();
            ViewBag.SoftTypes = swTypeRepo.GetAllValid();

            TestCategoryRepository testCatRepo = new TestCategoryRepository();
            ViewBag.TestCategories = testCatRepo.GetAllValid();

            int itemsOnPage = 8;
            int pg = page ?? 1;
            int startIndex = (pg * itemsOnPage) - itemsOnPage;

            SoftwareType swType = null;
            if (swTypeGuid != null)
            {
                swType = swTypeRepo.GetById((Guid)swTypeGuid);
                ViewBag.SoftType = swType;
            }

            TestCategory testCat = null;
            if (testCatGuid != null)
            {
                testCat = testCatRepo.GetById((Guid)testCatGuid);
                ViewBag.TestCategory = testCat;
            }

            ViewBag.CurrentSearch = searchTerm;
            IList<TestCase> testCases = testCaseRepo.GetAvailableEntities(out var totalTests, DateTime.Today, await testerTask, swType, testCat, startIndex, itemsOnPage, searchTerm);

            ViewBag.Pages = (int)Math.Ceiling((double)totalTests / (double)itemsOnPage);
            ViewBag.CurrentPage = pg;

            if (Request.IsAjaxRequest())
            {
                return PartialView(testCases);
            }

            return View(testCases);
        }

        [Authorize(Roles = "tester")]
        public async Task<ActionResult> TestDetails(Guid testGuid)
        {
            TestCaseRepository testCaseRepo = new TestCaseRepository();
            var testCaseTask = testCaseRepo.GetByIdAsync(testGuid);

            ApplicationUserRepository<Tester> userRepo = new ApplicationUserRepository<Tester>();
            var testerTask = userRepo.GetByUserNameAsync(User.Identity.Name);

            TestsRepository testsRepo = new TestsRepository();

            ViewBag.Takened = testsRepo.GetTestStatus(await testCaseTask, await testerTask);
            ViewBag.TestsGuid = testsRepo.GetByTestCaseForTester(await testerTask, await testCaseTask);
            return View(await testCaseTask);
        }

        [Authorize(Roles = "tester")]
        public async Task<ActionResult> TakeTestFromGrid(int? page, Guid? swTypeGuid, Guid? testCatGuid, string searchTerm, Guid testGuid)
        {
            await TakeTest(testGuid);

            return RedirectToAction("TesterTests", new {page, swTypeGuid, testCatGuid, searchTerm});
        }

        [Authorize(Roles = "tester")]
        public async Task<ActionResult> TakeTestFromDetails(Guid testGuid)
        {
            await TakeTest(testGuid);

            return RedirectToAction("TestDetails", new{testGuid});
        }

        [Authorize(Roles = "tester")]
        public async Task TakeTest(Guid testGuid)
        {
            TestCaseRepository testCaseRepo = new TestCaseRepository();
            var testCaseTask = testCaseRepo.GetByIdAsync(testGuid);

            ApplicationUserRepository<Tester> userRepo = new ApplicationUserRepository<Tester>();
            var testerTask = userRepo.GetByUserNameAsync(User.Identity.Name);

            DataAccess.Model.Tests.Tests tests = new DataAccess.Model.Tests.Tests();
            tests.Test = await testCaseTask;
            tests.Status = TestsStatus.Takened;
            tests.Takened = DateTime.Now;
            tests.Tester = await testerTask;

            TestsRepository testsRepo = new TestsRepository();
            testsRepo.Create(tests);
        }

        [Authorize(Roles = "tester")]
        public ActionResult TakenedTests(int? page, Guid? swTypeGuid, Guid? testCatGuid, string searchTerm)
        {
            TestsRepository testsRepo = new TestsRepository();

            SoftwareTypeRepository swTypeRepo = new SoftwareTypeRepository();
            ViewBag.SoftTypes = swTypeRepo.GetAllValid();

            TestCategoryRepository testCatRepo = new TestCategoryRepository();
            ViewBag.TestCategories = testCatRepo.GetAllValid();

            ApplicationUserRepository<Tester> userRepo = new ApplicationUserRepository<Tester>();
            Tester tester = userRepo.GetByUserName(User.Identity.Name);

            int itemsOnPage = 8;
            int pg = page ?? 1;
            int startIndex = (pg * itemsOnPage) - itemsOnPage;

            SoftwareType swType = null;
            if (swTypeGuid != null)
            {
                swType = swTypeRepo.GetById((Guid)swTypeGuid);
                ViewBag.SoftType = swType;
            }

            TestCategory testCat = null;
            if (testCatGuid != null)
            {
                testCat = testCatRepo.GetById((Guid)testCatGuid);
                ViewBag.TestCategory = testCat;
            }

            ViewBag.CurrentSearch = searchTerm;
            IList<TestsStatus> statuses = new List<TestsStatus>();
            statuses.Add(TestsStatus.Takened);
            IList<DataAccess.Model.Tests.Tests> tests = testsRepo.GetAvailableEntities(out var totalTests, tester, statuses, swType, testCat, startIndex, itemsOnPage, searchTerm);

            ViewBag.Pages = (int)Math.Ceiling((double)totalTests / (double)itemsOnPage);
            ViewBag.CurrentPage = pg;

            if (Request.IsAjaxRequest())
            {
                return PartialView(tests);
            }

            return View(tests);
        }

        [Authorize(Roles = "tester")]
        public ActionResult FinishedTests(int? page, Guid? swTypeGuid, Guid? testCatGuid, string searchTerm)
        {
            TestsRepository testsRepo = new TestsRepository();

            SoftwareTypeRepository swTypeRepo = new SoftwareTypeRepository();
            ViewBag.SoftTypes = swTypeRepo.GetAllValid();

            TestCategoryRepository testCatRepo = new TestCategoryRepository();
            ViewBag.TestCategories = testCatRepo.GetAllValid();

            ApplicationUserRepository<Tester> userRepo = new ApplicationUserRepository<Tester>();
            Tester tester = userRepo.GetByUserName(User.Identity.Name);

            int itemsOnPage = 8;
            int pg = page ?? 1;
            int startIndex = (pg * itemsOnPage) - itemsOnPage;

            SoftwareType swType = null;
            if (swTypeGuid != null)
            {
                swType = swTypeRepo.GetById((Guid)swTypeGuid);
                ViewBag.SoftType = swType;
            }

            TestCategory testCat = null;
            if (testCatGuid != null)
            {
                testCat = testCatRepo.GetById((Guid)testCatGuid);
                ViewBag.TestCategory = testCat;
            }

            ViewBag.CurrentSearch = searchTerm;
            IList<TestsStatus> statuses = new List<TestsStatus>();
            statuses.Add(TestsStatus.Finished);
            statuses.Add(TestsStatus.Reviewed);
            IList<DataAccess.Model.Tests.Tests> tests = testsRepo.GetAvailableEntities(out var totalTests, tester, statuses, swType, testCat, startIndex, itemsOnPage, searchTerm);

            ViewBag.Pages = (int)Math.Ceiling((double)totalTests / (double)itemsOnPage);
            ViewBag.CurrentPage = pg;

            if (Request.IsAjaxRequest())
            {
                return PartialView(tests);
            }

            return View(tests);
        }

        [Authorize(Roles = "tester")]
        public async Task<ActionResult> ResolveTest(Guid testGuid)
        {
            TestCaseRepository testCaseRepo = new TestCaseRepository();
            var testCaseTask = testCaseRepo.GetByIdAsync(testGuid);

            ApplicationUserRepository<Tester> userRepo = new ApplicationUserRepository<Tester>();
            var testerTask = userRepo.GetByUserNameAsync(User.Identity.Name);

            TestsRepository testsRepo = new TestsRepository();
            var testsTask = testsRepo.GetByTestCaseForTesterAsync(await testerTask, await testCaseTask);

            TestStatusRepository testStatusRepo = new TestStatusRepository();
            ViewBag.TestStatus = testStatusRepo.GetAll();

            return View(await testsTask);
        }

        [HttpPost]
        [Authorize(Roles = "tester")]
        public async Task<ActionResult> Resolve(DataAccess.Model.Tests.Tests tests, HttpPostedFileBase[] files)
        {
            TestsRepository testsRepo = new TestsRepository();
            DataAccess.Model.Tests.Tests testsRecord = testsRepo.GetById(tests.Id);

            TestStatusRepository testStatusRepo = new TestStatusRepository();

            ModelState.Remove("files");
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
                if (testsRecord.Test.Creator.Credits < testsRecord.Test.Reward)
                {                   
                    ViewBag.TestStatus = testStatusRepo.GetAll();

                    TempData["error"] = "Createor of tests don't have required amount of coins. Please try resolve test later, or contact our support team.";
                    return View("TakenedTests");
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

                            return View("ResolveTest", testsRecord);
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

                testsRecord.Tester.Credits = testsRecord.Tester.Credits + testsRecord.Test.Reward;
                testsRecord.Test.Creator.Credits = testsRecord.Test.Creator.Credits - testsRecord.Test.Reward;

                EvidenceRepository evidRepo = new EvidenceRepository();

                foreach (var evidence in evidences)
                {
                    evidRepo.Create(evidence);
                }

                testsRepo.Update(testsRecord);

                return RedirectToAction("FinishedTests");
            }

            ViewBag.TestStatus = testStatusRepo.GetAll();

            return View("ResolveTest", testsRecord);
        }

        [Authorize(Roles = "tester")]
        public async Task<ActionResult> AddReview(Guid testsGuid)
        {
            ViewBag.TestsGuid = testsGuid;

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "tester")]
        public async Task<ActionResult> CreateReview(Review review, Guid testsGuid)
        {
            ModelState.Remove("testsGuid");
            ModelState.Remove(nameof(Review.Id));
            ModelState.Remove(nameof(Review.Created));
            ModelState.Remove(nameof(Review.Creator));
            if (ModelState.IsValid)
            {
                TestsRepository testsRepo = new TestsRepository();
                var testsTask = testsRepo.GetByIdAsync(testsGuid);

                ApplicationUserRepository<Tester> userRepo = new ApplicationUserRepository<Tester>();
                var testerTask = userRepo.GetByUserNameAsync(User.Identity.Name);

                review.Created = DateTime.Now;
                review.Creator = await testerTask;

                DataAccess.Model.Tests.Tests tests = await testsTask;

                tests.Test.Reviews.Add(review);

                tests.Test.CountRating();
                tests.Status = TestsStatus.Reviewed;

                ReviewRepository reviewRepo = new ReviewRepository();

                reviewRepo.Create(review);

                testsRepo.Update(tests);

                return RedirectToAction("FinishedTests");
            }

            ViewBag.TestsGuid = testsGuid;
            return View("AddReview", review);
        }
    }
}