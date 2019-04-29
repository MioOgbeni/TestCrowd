using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DataTables.AspNet.Core;
using DataTables.AspNet.Mvc5;
using TestCrowd.Class;
using TestCrowd.DataAccess.Model;
using TestCrowd.DataAccess.Model.Tests;
using TestCrowd.DataAccess.Repository;
using TestCrowd.DataAccess.Repository.Tests;

namespace TestCrowd.Areas.Testing.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdministrationController : Controller
    {
        // GET: Testing/Administration
        public ActionResult Index()
        {
            return RedirectToAction("Admins");
        }

        public ActionResult Admins()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminsDataTable(IDataTablesRequest request)
        {
            var adminRepo = new ApplicationUserRepository<Admin>();

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

            string currentUserId = adminRepo.GetByUserName(User.Identity.Name).Id.ToString(); 

            var admins = adminRepo.GetEntities(out int totalCount,out int filteredCount, request.Start, request.Length, nameof(Admin.Email), request.Search.Value, orderColumn.Name, asc, currentUserId);

            var response = DataTablesResponse.Create(request, totalCount, filteredCount, admins);

            return new DataTablesJsonResult(response, JsonRequestBehavior.DenyGet);
        }

        public ActionResult Delete(IList<Admin> rows)
        {
            var adminRepo = new ApplicationUserRepository<Admin>();
            var roleRepo = new ApplicationRoleRepository();
            foreach (var row in rows)
            {             
                row.Role = roleRepo.GetByName("admin");
                adminRepo.Delete(row);
            }

            TempData["success"] = "User/s was deleted!";

            return View("Admins");
        }

        [HttpPost]
        public async Task<ActionResult> Add(Admin admin)
        {
            if (ModelState.IsValid)
            {
                var userRepository = new ApplicationUserRepository<Admin>();
                var roleRepository = new ApplicationRoleRepository();
                var role = roleRepository.GetByNameAsync("admin");
                var isUserExist = userRepository.IsUserExistAsync(admin.UserName);

                if (await isUserExist)
                {
                    TempData["error"] = "Account with this name already exist!";
                    return View("Create", admin);
                }

                Admin _admin = new Admin()
                {
                    UserName = admin.UserName,
                    Email = admin.Email,
                    Role = await role,
                    PasswordHash = ApplicationPasswordHasher.HashPasword(admin.PasswordHash),
                    FirstName = admin.FirstName,
                    LastName = admin.LastName
                };

                new ApplicationUserRepository<Admin>().Create(_admin);
                TempData["success"] = "Admin account was added";
            }
            else
            {
                return View("Create", admin);
            }

            return RedirectToAction("Admins");
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(string id)
        {
            var userRepository = new ApplicationUserRepository<Admin>();
            var admin = userRepository.GetById(Guid.Parse(id));
            return View(admin);
        }

        [HttpPost]
        public ActionResult Update(Admin admin)
        {
            ModelState.Remove(nameof(admin.PasswordConfirmationHash));
            if (ModelState.IsValid)
            {
                var userRepository = new ApplicationUserRepository<Admin>();

                var roleRepository = new ApplicationRoleRepository();
                var role = roleRepository.GetByName("admin");
                admin.Role = role;

                userRepository.Update(admin);
                TempData["success"] = "Admin account was edited";
            }
            else
            {
                return View("Edit", admin);
            }

            return RedirectToAction("Admins");
        }

        public ActionResult SoftwareTypes()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SoftwareTypesDataTable(IDataTablesRequest request)
        {
            var swTypeRepo = new SoftwareTypeRepository();

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

            var swTypes = swTypeRepo.GetEntities(out int totalCount, out int filteredCount, request.Start, request.Length, nameof(SoftwareType.Name), request.Search.Value, orderColumn.Name, asc).Select(x=> new
            {
                x.Id,
                x.Name,
                x.Description,
                x.Valid,
                x.Created,
                x.LastChange,
                x.ChangedBy,
                ChangedByName = x.ChangedBy.FirstName + " " + x.ChangedBy.LastName
            });

            var response = DataTablesResponse.Create(request, totalCount, filteredCount, swTypes);

            return new DataTablesJsonResult(response, JsonRequestBehavior.DenyGet);
        }

        public ActionResult DeleteSoftwareType(IList<SoftwareType> rows)
        {
            var swTypeRepo = new SoftwareTypeRepository();

            foreach (var row in rows)
            {
                var swType = swTypeRepo.GetById(row.Id);

                try
                {
                    swTypeRepo.Delete(swType);
                }
                catch (Exception e)
                {
                    TempData["error"] = "Software type/s can't be delete, because it used in some part of system!";

                    return View("SoftwareTypes");
                }
            }

            TempData["success"] = "Software type/s was deleted!";

            return View("SoftwareTypes");
        }

        [HttpPost]
        public async Task<ActionResult> AddSoftwareType(SoftwareType swType)
        {
            ModelState.Remove(nameof(swType.Id));
            ModelState.Remove(nameof(swType.Created));
            ModelState.Remove(nameof(swType.LastChange));
            ModelState.Remove(nameof(swType.ChangedBy));
            if (ModelState.IsValid)
            {
                var userRepository = new ApplicationUserRepository<Admin>();
                var currentUser = userRepository.GetByUserNameAsync(User.Identity.Name);

                SoftwareType _swType = new SoftwareType()
                {
                    Name = swType.Name,
                    Description = swType.Description,
                    Valid = swType.Valid,
                    Created = DateTime.Now,
                    LastChange = DateTime.Now,
                    ChangedBy = await currentUser
                };

                new SoftwareTypeRepository().Create(_swType);
                TempData["success"] = "Software type was added";
            }
            else
            {
                return View("CreateSoftwareType", swType);
            }

            return RedirectToAction("SoftwareTypes");
        }

        public ActionResult CreateSoftwareType()
        {
            return View();
        }

        public ActionResult EditSoftwareType(string id)
        {
            var swTypeRepo = new SoftwareTypeRepository();
            var swType = swTypeRepo.GetById(Guid.Parse(id));
            return View(swType);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateSoftwareType(SoftwareType swType)
        {
            ModelState.Remove(nameof(swType.Id));
            ModelState.Remove(nameof(swType.Created));
            ModelState.Remove(nameof(swType.LastChange));
            ModelState.Remove(nameof(swType.ChangedBy));
            if (ModelState.IsValid)
            {
                var userRepository = new ApplicationUserRepository<Admin>();
                var swTypeRepo = new SoftwareTypeRepository();
                var currentUser = userRepository.GetByUserNameAsync(User.Identity.Name);

                swType.LastChange = DateTime.Now;
                swType.ChangedBy = await currentUser;

                swTypeRepo.Update(swType);
                TempData["success"] = "Software type was edited";
            }
            else
            {
                return View("EditSoftwareType", swType);
            }

            return RedirectToAction("SoftwareTypes");
        }

        public ActionResult ToggleValidSoftwareType(IList<SoftwareType> rows)
        {
            var swTypeRepo = new SoftwareTypeRepository();

            foreach (var row in rows)
            {
                var swType = swTypeRepo.GetById(row.Id);

                if (swType.Valid)
                {
                    swType.Valid = false;
                }
                else if (!swType.Valid)
                {
                    swType.Valid = true;
                }

                swTypeRepo.Update(swType);
            }

            TempData["success"] = "Software type/s was updated!";

            return View("SoftwareTypes");
        }

        public ActionResult TestCategories()
        {
            return View();
        }

        [HttpPost]
        public ActionResult TestCategoriesDataTable(IDataTablesRequest request)
        {
            var testCatRepo = new TestCategoryRepository();

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

            var testCat = testCatRepo.GetEntities(out int totalCount, out int filteredCount, request.Start, request.Length, nameof(SoftwareType.Name), request.Search.Value, orderColumn.Name, asc).Select(x => new
            {
                x.Id,
                x.Name,
                x.Description,
                x.Valid,
                x.Created,
                x.LastChange,
                x.ChangedBy,
                ChangedByName = x.ChangedBy.FirstName + " " + x.ChangedBy.LastName
            });

            var response = DataTablesResponse.Create(request, totalCount, filteredCount, testCat);

            return new DataTablesJsonResult(response, JsonRequestBehavior.DenyGet);
        }

        public ActionResult DeleteTestCategory(IList<TestCategory> rows)
        {
            var testCatRepo = new TestCategoryRepository();

            foreach (var row in rows)
            {
                var testCat = testCatRepo.GetById(row.Id);

                try
                {
                    testCatRepo.Delete(testCat);
                }
                catch (Exception e)
                {
                    TempData["error"] = "Test category/ies can't be delete, because it used in some part of system!";

                    return View("TestCategories");
                }

            }

            TempData["success"] = "Test category/ies was deleted!";

            return View("TestCategories");
        }

        [HttpPost]
        public async Task<ActionResult> AddTestCategory(TestCategory testCat)
        {
            ModelState.Remove(nameof(testCat.Id));
            ModelState.Remove(nameof(testCat.Created));
            ModelState.Remove(nameof(testCat.LastChange));
            ModelState.Remove(nameof(testCat.ChangedBy));
            if (ModelState.IsValid)
            {
                var userRepository = new ApplicationUserRepository<Admin>();
                var currentUser = userRepository.GetByUserNameAsync(User.Identity.Name);

                TestCategory _testCat = new TestCategory()
                {
                    Name = testCat.Name,
                    Description = testCat.Description,
                    Valid = testCat.Valid,
                    Created = DateTime.Now,
                    LastChange = DateTime.Now,
                    ChangedBy = await currentUser
                };

                new TestCategoryRepository().Create(_testCat);
                TempData["success"] = "Test category was added";
            }
            else
            {
                return View("CreateTestCategory", testCat);
            }

            return RedirectToAction("TestCategories");
        }

        public ActionResult CreateTestCategory()
        {
            return View();
        }

        public ActionResult EditTestCategory(string id)
        {
            var testCatRepo = new TestCategoryRepository();
            var testCat = testCatRepo.GetById(Guid.Parse(id));
            return View(testCat);
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTestCategory(TestCategory testCat)
        {
            ModelState.Remove(nameof(testCat.Id));
            ModelState.Remove(nameof(testCat.Created));
            ModelState.Remove(nameof(testCat.LastChange));
            ModelState.Remove(nameof(testCat.ChangedBy));
            if (ModelState.IsValid)
            {
                var userRepository = new ApplicationUserRepository<Admin>();
                var testCatRepo = new TestCategoryRepository();
                var currentUser = userRepository.GetByUserNameAsync(User.Identity.Name);

                testCat.LastChange = DateTime.Now;
                testCat.ChangedBy = await currentUser;

                testCatRepo.Update(testCat);
                TempData["success"] = "Test category was edited";
            }
            else
            {
                return View("EditTestCategory", testCat);
            }

            return RedirectToAction("TestCategories");
        }

        public ActionResult ToggleValidTestCategory(IList<TestCategory> rows)
        {
            var testCatRepo = new TestCategoryRepository();

            foreach (var row in rows)
            {
                var testCat = testCatRepo.GetById(row.Id);

                if (testCat.Valid)
                {
                    testCat.Valid = false;
                }
                else if (!testCat.Valid)
                {
                    testCat.Valid = true;
                }

                testCatRepo.Update(testCat);
            }

            TempData["success"] = "Test category/ies was updated!";

            return View("TestCategories");
        }
    }
}