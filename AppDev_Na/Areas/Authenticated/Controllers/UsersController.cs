using System;
using System.Composition.Hosting;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppDev_Na.Data;
using AppDev_Na.Models;
using AppDev_Na.Utility.Enum;
using AppDev_Na.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace AppDev_Na.Areas.Authenticated.Controllers
{
    [Area("Authenticated")]
    [Authorize(Roles = Constant.Role_Admin + "," + Constant.Role_Staff)]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(ApplicationDbContext db, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userList = _db.ApplicationUsers.Where(u => u.Id != claims.Value);

            foreach (var user in userList)
            {
                var userTemp = await _userManager.FindByIdAsync(user.Id);
                var roleTemp = await _userManager.GetRolesAsync(userTemp);
                user.Role = roleTemp.FirstOrDefault();
            }

            if (User.IsInRole(Constant.Role_Admin))
            {
                var userListTemp = userList.ToList().Where(u => u.Role != Constant.Role_Trainee);
                ViewData["Message"] = TempData["Message"];
                return View(userListTemp);
            }

            var studentUser = userList.ToList()
                .Where(u => u.Role == Constant.Role_Trainee || u.Role == Constant.Role_Trainer);
            return View(studentUser);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = _db.ApplicationUsers.Find(id);
            var userTemp = await _userManager.FindByIdAsync(user.Id);
            var roleTemp = await _userManager.GetRolesAsync(userTemp);
            user.Role = roleTemp.FirstOrDefault();
            if (user.Role == Constant.Role_Admin || user.Role == Constant.Role_Staff)
            {
                return RedirectToAction(nameof(EditStaff), new {id = id});
            }
            else if (user.Role == Constant.Role_Trainee)
            {
                return RedirectToAction(nameof(EditTrainee), new {id = id});
            }
            else
            {
                return RedirectToAction(nameof(EditTrainer), new {id = id});
            }

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> EditStaff(string id)
        {
            var user = _db.ApplicationUsers.Find(id);
            if (user == null)
            {
                ViewData["Message"] = "Error: User not found!";
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStaff(ApplicationUser user)
        {
            var userdb = _db.ApplicationUsers.Find(user.Id);
            if (userdb == null)
            {
                ViewData["Message"] = "Error: Data null";
                return RedirectToAction(nameof(Index), "Users");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            userdb.FullName = user.FullName;
            userdb.Age = user.Age;
            userdb.Address = user.Address;
            _db.ApplicationUsers.Update(userdb);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index), "Users");
        }
        
        [HttpGet]
        [Authorize(Roles = Constant.Role_Staff)]
        public async Task<IActionResult> EditTrainee(string id)
        {
            var user = _db.Trainee.Find(id);
            if (user == null)
            {
                ViewData["Message"] = "Error: User not found!";
                return NotFound();
            }

            return View(user);
        }
        
        [HttpPost]
        [Authorize(Roles = Constant.Role_Staff)]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainee(Trainee user)
        {
            var userdb = _db.Trainee.Find(user.Id);
            if (userdb == null)
            {
                ViewData["Message"] = "Error: Data null";
                return RedirectToAction(nameof(Index), "Users");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            userdb.FullName = user.FullName;
            userdb.Age = user.Age;
            userdb.Address = user.Address;

            userdb.DateOfBirth = user.DateOfBirth;
            userdb.Education = user.Education;
            _db.Trainee.Update(userdb);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index), "Users");
        }

        [HttpGet]
        public async Task<IActionResult> EditTrainer(string id)
        {
            var user = _db.Trainer.Find(id);
            if (user == null)
            {
                ViewData["Message"] = "Error: User not found!";
                return NotFound();
            }

            return View(user);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTrainer(Trainer user)
        {
            var userdb = _db.Trainer.Find(user.Id);
            if (userdb == null)
            {
                ViewData["Message"] = "Error: Data null";
                return RedirectToAction(nameof(Index), "Users");
            }

            if (!ModelState.IsValid)
            {
                return View(user);
            }

            userdb.FullName = user.FullName;
            userdb.Age = user.Age;
            userdb.Address = user.Address;
            userdb.Specialty = user.Specialty;

            _db.Trainer.Update(userdb);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index), "Users");
        }
        [HttpGet]
        public async Task<IActionResult> Delete(string id)
        {
            var applicationUser = _db.ApplicationUsers.Find(id);
            if (applicationUser == null)
            {
                return RedirectToAction(nameof(Index), "Users");
            }
            await _userManager.DeleteAsync(applicationUser);
            return RedirectToAction(nameof(Index), "Users");
        }

        [Authorize(Roles = Constant.Role_Admin)]
        [HttpGet]
        public async Task<IActionResult> ForgotPassword(string id)
        {
            var user = _db.ApplicationUsers.Find(id);
            if (user ==null)
            {
                return View();
            }

            ForgotPasswordViewModel UserEmail = new ForgotPasswordViewModel()
            {
                Email = user.Email
            };
            return View(UserEmail);
        }

        [Authorize(Roles = Constant.Role_Admin)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var result = await _userManager.ResetPasswordAsync(user, token, "Password123@");
                    if (result.Succeeded)
                    {
                        return View("ResetPasswordConfimation");
                    }
                }
            }
            return View("ResetPasswordFail");
        }
        
        [Authorize(Roles = Constant.Role_Admin)]
        [HttpGet]
        public IActionResult LockUnlock(string id)
        {
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var claimUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == claims.Value);

            var applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            if (claimUser.Id == applicationUser.Id)
            {
                return NotFound();
            }

            if (applicationUser.LockoutEnd != null && applicationUser.LockoutEnd > DateTime.Now)
            {
                applicationUser.LockoutEnd = DateTime.Now;
            }
            else
            {
                applicationUser.LockoutEnd = DateTime.Now.AddYears(1000);
            }

            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}