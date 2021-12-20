using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AppDev_Na.Data;
using AppDev_Na.Utility.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppDev_Na.Areas.Identity.Pages.Account.Manage
{
    public partial class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _db;

        public IndexModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public string Username { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }
            [Required]
            public string FullName { get; set; }
            [Required]
            public int Age { get; set; }
            [Required]
            public string Education { get; set; }
            [Required]
            public DateTime DateOfBirth { get; set; }
            [Required]
            public string Address { get; set; }
            [Required]
            public string Specialty { get; set; }
            
        }

        private async Task LoadAsync(IdentityUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var claimsIdentity = (ClaimsIdentity) User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var usertemp = _db.ApplicationUsers.FirstOrDefault(u => u.Id == claims.Value);
            var role = await _userManager.GetRolesAsync(usertemp);

            Username = userName;
            if (role.FirstOrDefault()==Constant.Role_Trainee)
            {
                var userList = _db.Trainee.Find(user.Id);
                Input = new InputModel
                {
                    PhoneNumber = phoneNumber,
                    FullName = userList.FullName,
                    Education = userList.Education,
                    Age = userList.Age,
                    DateOfBirth = userList.DateOfBirth
                };
            }else if (role.FirstOrDefault()==Constant.Role_Trainer)
            {
                var userList = _db.Trainer.Find(user.Id);
                Input = new InputModel
                {
                    PhoneNumber = phoneNumber,
                    FullName = userList.FullName,
                    Age = userList.Age,
                    Address = userList.Address,
                    Specialty = userList.Specialty
                };
            }
            else
            {
                var userList = _db.ApplicationUsers.Find(user.Id);
                Input = new InputModel
                {
                    PhoneNumber = phoneNumber,
                    FullName = userList.FullName,
                    Age = userList.Age,
                    Address = userList.Address,
                };
            }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (User.IsInRole(Constant.Role_Trainee))
            {
                var profile = _db.Trainee.Find(user.Id);
                if (Input.PhoneNumber != profile.PhoneNumber)
                {
                    profile.PhoneNumber = Input.PhoneNumber;
                }
                if (Input.FullName != profile.FullName)
                {
                    profile.FullName = Input.FullName;
                }
                if (Input.Education != profile.Education)
                {
                    profile.Education = Input.Education;
                }
                if (Input.Age != profile.Age)
                {
                    profile.Age = Input.Age;
                }
                if (Input.DateOfBirth != profile.DateOfBirth)
                {
                    profile.DateOfBirth = Input.DateOfBirth;
                }      
                _db.Trainee.Update(profile);
                _db.SaveChanges();                
            }
            if (User.IsInRole(Constant.Role_Trainer))
            {
                var profile = _db.Trainer.Find(user.Id);
                if (Input.PhoneNumber != profile.PhoneNumber)
                {
                    profile.PhoneNumber = Input.PhoneNumber;
                }
                if (Input.FullName != profile.FullName)
                {
                    profile.FullName = Input.FullName;
                }
                if (Input.Age != profile.Age)
                {
                    profile.Age = Input.Age;
                }
                if (Input.Address != profile.Address)
                {
                    profile.Address = Input.Address;
                }
                if (Input.Specialty != profile.Specialty)
                {
                    profile.Specialty = Input.Specialty;
                }  
                _db.Trainer.Update(profile);
                _db.SaveChanges();                
            }
            else
            {
                var profile = _db.ApplicationUsers.Find(user.Id);
                if (Input.PhoneNumber != profile.PhoneNumber)
                {
                    profile.PhoneNumber = Input.PhoneNumber;
                }
                if (Input.FullName != profile.FullName)
                {
                    profile.FullName = Input.FullName;
                }
                if (Input.Age != profile.Age)
                {
                    profile.Age = Input.Age;
                }
                if (Input.Address != profile.Address)
                {
                    profile.Address = Input.Address;
                }

                _db.ApplicationUsers.Update(profile);
                _db.SaveChanges();
            }
            
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
