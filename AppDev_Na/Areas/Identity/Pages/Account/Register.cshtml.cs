using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AppDev_Na.Data;
using AppDev_Na.Models;
using AppDev_Na.Utility.Enum;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace AppDev_Na.Areas.Identity.Pages.Account
{
    [Authorize (Roles = Constant.Role_Admin + "," + Constant.Role_Staff)]
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<RegisterModel> logger,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _roleManager = roleManager;
            _emailSender = emailSender;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
            
            [Required]
            public string Name { get; set; }
            [Required]
            public int Age { get; set; }
            [Required]
            public string Role { get; set; }
            public  IEnumerable<SelectListItem> RoleList { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            GetRole();
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ApplicationUser();
                Trainee trainee = new Trainee();
                Trainer trainer = new Trainer();
                IdentityResult result = new IdentityResult();
                if (Input.Role == Constant.Role_Trainee)
                {
                    trainee = new Trainee()
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        Role = Input.Role,
                        FullName = Input.Name,
                    };
                    result = await _userManager.CreateAsync(trainee, Input.Password);
                    
                } else if (Input.Role == Constant.Role_Trainer)
                {
                    trainer = new Trainer()
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        Role = Input.Role,
                        FullName = Input.Name,
                    };
                    result = await _userManager.CreateAsync(trainer, Input.Password);
                }
                else
                {
                    applicationUser = new ApplicationUser()
                    {
                        UserName = Input.Email,
                        Email = Input.Email,
                        Role = Input.Role,
                        FullName = Input.Name,
                    };
                    result = await _userManager.CreateAsync(applicationUser, Input.Password);
                }

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    if (Input.Role == Constant.Role_Trainee)
                    {
                        await _userManager.AddToRoleAsync(trainee, trainee.Role); 
                        
                    } else if (Input.Role == Constant.Role_Trainer)
                    {
                        await _userManager.AddToRoleAsync(trainer, trainer.Role); 
                        
                    }
                    else 
                    {
                        await _userManager.AddToRoleAsync(applicationUser, applicationUser.Role); 
                        
                    }
                    
                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        return RedirectToPage("RegisterConfirmation",
                                new {email = Input.Email, returnUrl = returnUrl});

                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            GetRole();
            // If we got this far, something failed, redisplay form
            return Page();
        }

        private void GetRole()
        {
            Input = new InputModel()
            {
                RoleList = _roleManager.Roles.Where(u => u.Name != Constant.Role_Trainee).Select(x => x.Name).Select(
                    i => new SelectListItem
                    {
                        Text = i,
                        Value = i
                    })
            };
            if (User.IsInRole(Constant.Role_Staff))
            {
                Input = new InputModel()
                {
                    RoleList = _roleManager.Roles.Where(u => u.Name == Constant.Role_Trainee).Select(x => x.Name)
                        .Select(i => new SelectListItem
                        {
                            Text = i,
                            Value = i
                        })
                };
            }
        }

        private async Task SendEmail(ApplicationUser user, string returnUrl)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

        }
    }
}
