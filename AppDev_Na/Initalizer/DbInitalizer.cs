using System;
using System.Linq;
using AppDev_Na.Data;
using AppDev_Na.Models;
using AppDev_Na.Utility.Enum;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AppDev_Na.Initalizer
{
    public class DbInitalizer : IDbInitalizer
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DbInitalizer(ApplicationDbContext db, UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _roleManager = roleManager;
            _userManager = userManager;
        }
        
        public void Initalizer()
        {
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                
            }

            if (_db.Roles.Any(r => r.Name == Constant.Role_Admin)) return;
            if (_db.Roles.Any(r => r.Name == Constant.Role_Staff)) return;
            if (_db.Roles.Any(r => r.Name == Constant.Role_Trainee)) return;
            if (_db.Roles.Any(r => r.Name == Constant.Role_Trainer)) return;

            _roleManager.CreateAsync(new IdentityRole(Constant.Role_Admin)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Constant.Role_Staff)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Constant.Role_Trainee)).GetAwaiter().GetResult();
            _roleManager.CreateAsync(new IdentityRole(Constant.Role_Trainer)).GetAwaiter().GetResult();

            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true,
                FullName = "Admin"
            },"Admin123@").GetAwaiter().GetResult();
            
            ApplicationUser userAdmin =_db.ApplicationUsers.Where(u => u.Email == "admin@gmail.com").FirstOrDefault();
            _userManager.AddToRoleAsync(userAdmin, Constant.Role_Admin).GetAwaiter().GetResult();
            
            _userManager.CreateAsync(new ApplicationUser
            {
                UserName = "staff@gmail.com",
                Email = "staff@gmail.com",
                EmailConfirmed = true,
                FullName = "Staff"
            },"Staff123@").GetAwaiter().GetResult();
            
            ApplicationUser userStaff =_db.ApplicationUsers.Where(u => u.Email == "staff@gmail.com").FirstOrDefault();
            _userManager.AddToRoleAsync(userStaff, Constant.Role_Staff).GetAwaiter().GetResult();
            
            _userManager.CreateAsync(new Trainer()
            {
                UserName = "trainer@gmail.com",
                Email = "trainer@gmail.com",
                EmailConfirmed = true,
                FullName = "Trainer"
            },"Trainer123@").GetAwaiter().GetResult();
            
            Trainer userTrainer =_db.Trainer.Where(u => u.Email == "trainer@gmail.com").FirstOrDefault();
            _userManager.AddToRoleAsync(userTrainer, Constant.Role_Trainer).GetAwaiter().GetResult();
            
            _userManager.CreateAsync(new Trainee()
            {
                UserName = "trainee@gmail.com",
                Email = "trainee@gmail.com",
                EmailConfirmed = true,
                FullName = "Trainee"
            },"Trainee123@").GetAwaiter().GetResult();
            
            Trainee userTrainee =_db.Trainee.Where(u => u.Email == "trainee@gmail.com").FirstOrDefault();
            _userManager.AddToRoleAsync(userTrainee, Constant.Role_Trainee).GetAwaiter().GetResult();
        }
    }
}